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

    /// <summary>Get invoices with filters.</summary>
    [HttpGet]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? customerId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? shipmentId = null)
    {
        var result = await _mediator.Send(new GetInvoicesQuery(page, pageSize, customerId, status, dateFrom, dateTo, shipmentId));
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
            if (result.Data?.CustomerId.ToString() != userId)
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
            if (result.Data.CustomerId.ToString() != userId)
                return Forbid();
        }

        if (string.IsNullOrWhiteSpace(result.Data.PdfBlobUrl))
            return NotFound(new { message = "PDF not available for this invoice." });

        return Ok(new { pdfUrl = result.Data.PdfBlobUrl });
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
