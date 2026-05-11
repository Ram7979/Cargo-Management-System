using System.Security.Claims;
using CMS.BillingService.Application.Commands.GenerateInvoice;
using CMS.BillingService.Application.Commands.MarkInvoicePaid;
using CMS.BillingService.Application.Commands.VoidInvoice;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Application.Queries.GetInvoice;
using CMS.BillingService.Application.Queries.GetInvoicePayments;
using CMS.BillingService.Application.Queries.GetInvoices;
using CMS.BillingService.Application.Queries.GetOverdueInvoices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.BillingService.API.Controllers;

[ApiController]
[Route("api/v1/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Revenue summary: totalInvoiced, totalCollected, outstanding for a date range.</summary>
    [HttpGet("revenue")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetRevenueSummary(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? customerId = null,
        [FromQuery] string? serviceType = null)
    {
        var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
        var to   = toDate   ?? DateTime.UtcNow;

        // 1. Get invoices in range for "Total Invoiced"
        var invoicesResult = await _mediator.Send(
            new CMS.BillingService.Application.Queries.GetInvoices.GetInvoicesQuery(
                1, int.MaxValue, customerId, null, from, to, null));
        
        var invoicesList = invoicesResult.Data?.ToList() ?? new List<InvoiceDto>();
        var totalInvoiced = invoicesList.Sum(i => i.TotalAmount);

        // 2. Get payments in range for "Total Collected"
        // This is the CRITICAL fix: Revenue = Payments made TODAY, not invoices from today.
        var paymentsResult = await _mediator.Send(
            new CMS.BillingService.Application.Queries.GetPayments.GetPaymentsQuery(
                1, int.MaxValue, null, null, from, to));
        
        var paymentsList = paymentsResult.Data?.ToList() ?? new List<PaymentDto>();
        var totalCollected = paymentsList.Sum(p => p.Amount);
        
        // 3. Outstanding = sum of all outstanding balances (not just for this period usually, but we'll follow previous logic)
        var outstanding = invoicesList.Sum(i => i.OutstandingBalance);

        // 4. Build trend data from payments
        var trend = paymentsList
            .GroupBy(p => p.PaidAt.Date)
            .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Value = g.Sum(p => p.Amount) })
            .OrderBy(x => x.Date)
            .ToList();

        return Ok(new
        {
            success = true,
            data = new
            {
                totalInvoiced,
                totalCollected,
                outstanding,
                revenue = totalCollected, // alias for reporting service
                revenueTrend = trend,
                fromDate = from,
                toDate   = to
            }
        });
    }

    /// <summary>Generate a new invoice with line item breakdown.</summary>

    [HttpPost]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GenerateInvoice([FromBody] GenerateInvoiceRequest request)
    {
        var result = await _mediator.Send(new GenerateInvoiceCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetInvoice), new { invoiceId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Internal endpoint to auto-generate invoice (bypasses role check).</summary>
    [HttpPost("internal/generate")]
    [AllowAnonymous]
    public async Task<IActionResult> InternalGenerateInvoice([FromBody] GenerateInvoiceRequest request)
    {
        var result = await _mediator.Send(new GenerateInvoiceCommand(request));
        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>Get invoices with filters.</summary>
    [HttpGet]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin,Customer")]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? customerId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? shipmentId = null)
    {
        // If customer, force their own ID
        if (User.IsInRole("Customer"))
        {
            customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        var result = await _mediator.Send(new GetInvoicesQuery(page, pageSize, customerId, status, dateFrom, dateTo, shipmentId));
        return Ok(result);
    }

    /// <summary>Get invoices for the calling user.</summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyInvoices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _mediator.Send(new GetInvoicesQuery(page, pageSize, userId, status, null, null, null));
        return Ok(result);
    }

    /// <summary>Get invoice by ID. Customers can only see their own invoices.</summary>
    [HttpGet("{invoiceId:guid}")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,Customer,SuperAdmin")]
    public async Task<IActionResult> GetInvoice(Guid invoiceId)
    {
        var result = await _mediator.Send(new GetInvoiceQuery(invoiceId));

        // Customer ownership check
        if (result.Success && User.IsInRole("Customer"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (result.Data?.CustomerId != userId)
                return Forbid();
        }

        return Ok(result);
    }

    /// <summary>Get all payments for a specific invoice.</summary>
    [HttpGet("{invoiceId:guid}/payments")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,Customer,SuperAdmin")]
    public async Task<IActionResult> GetInvoicePayments(Guid invoiceId)
    {
        var result = await _mediator.Send(new GetInvoicePaymentsQuery(invoiceId));
        return Ok(result);
    }

    /// <summary>Download invoice PDF (returns blob URL).</summary>
    [HttpGet("{invoiceId:guid}/pdf")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,Customer,SuperAdmin")]
    public async Task<IActionResult> GetInvoicePdf(Guid invoiceId)
    {
        var result = await _mediator.Send(new GetInvoiceQuery(invoiceId));
        if (!result.Success || result.Data == null) return NotFound(result);

        // Customer ownership check
        if (User.IsInRole("Customer"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (result.Data.CustomerId != userId)
                return Forbid();
        }

        return Ok(new { pdfUrl = $"/api/v1/invoices/{invoiceId}/document" });
    }

    /// <summary>Get Invoice Document (Generates PDF).</summary>
    [HttpGet("{invoiceId:guid}/document")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,Customer,SuperAdmin")]
    public async Task<IActionResult> GetInvoiceDocument(Guid invoiceId)
    {
        var result = await _mediator.Send(new GetInvoiceQuery(invoiceId));
        if (!result.Success || result.Data == null) return NotFound();

        var invoice = result.Data;

        // Customer ownership check
        if (User.IsInRole("Customer"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (invoice.CustomerId != userId)
                return Forbid();
        }

        using (var ms = new MemoryStream())
        {
            using (var writer = new StreamWriter(ms))
            {
                writer.WriteLine("%PDF-1.4");
                writer.WriteLine("1 0 obj <</Type /Catalog /Pages 2 0 R>> endobj");
                writer.WriteLine("2 0 obj <</Type /Pages /Kids [3 0 R] /Count 1>> endobj");
                writer.WriteLine("3 0 obj <</Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources <<>>>> endobj");
                
                var content = $"BT /F1 12 Tf 100 700 Td (INVOICE) Tj " +
                              $"0 -20 Td (Invoice #: {invoice.InvoiceNumber}) Tj " +
                              $"0 -20 Td (Customer: {invoice.CustomerId}) Tj " +
                              $"0 -20 Td (Shipment ID: {invoice.ShipmentId}) Tj " +
                              $"0 -20 Td (Total Amount: ${invoice.TotalAmount}) Tj " +
                              $"0 -20 Td (Outstanding: ${invoice.OutstandingBalance}) Tj " +
                              $"0 -20 Td (Status: {invoice.Status}) Tj " +
                              $"0 -20 Td (Date: {invoice.InvoiceDate:yyyy-MM-dd}) Tj ET";
                              
                writer.WriteLine($"4 0 obj <</Length {content.Length}>> stream");
                writer.WriteLine(content);
                writer.WriteLine("endstream endobj");
                writer.WriteLine("xref");
                writer.WriteLine("0 5");
                writer.WriteLine("0000000000 65535 f");
                writer.WriteLine("0000000010 00000 n");
                writer.WriteLine("0000000060 00000 n");
                writer.WriteLine("0000000115 00000 n");
                writer.WriteLine("0000000210 00000 n");
                writer.WriteLine("trailer <</Size 5 /Root 1 0 R>>");
                writer.WriteLine("startxref");
                writer.WriteLine("300");
                writer.WriteLine("%%EOF");
                writer.Flush();
            }
            
            return File(ms.ToArray(), "application/pdf", $"Invoice_{invoice.InvoiceNumber}.pdf");
        }
    }

    /// <summary>Void an invoice.</summary>
    [HttpPatch("{invoiceId:guid}/void")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> VoidInvoice(Guid invoiceId, [FromBody] VoidInvoiceRequest request)
    {
        var result = await _mediator.Send(new VoidInvoiceCommand(invoiceId, request));
        return Ok(result);
    }

    /// <summary>Manually mark invoice as paid (admin override).</summary>
    [HttpPatch("{invoiceId:guid}/mark-paid")]
    [Authorize(Roles = "FinanceOfficer,SuperAdmin")]
    public async Task<IActionResult> MarkPaid(Guid invoiceId, [FromBody] MarkPaidRequest request)
    {
        var result = await _mediator.Send(new MarkInvoicePaidCommand(invoiceId, request));
        return Ok(result);
    }

    /// <summary>Get all overdue invoices.</summary>
    [HttpGet("overdue")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetOverdueInvoices()
    {
        var result = await _mediator.Send(new GetOverdueInvoicesQuery());
        return Ok(result);
    }
}
