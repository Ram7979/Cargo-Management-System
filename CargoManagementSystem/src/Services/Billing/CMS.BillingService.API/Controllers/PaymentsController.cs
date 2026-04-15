using System.Security.Claims;
using System.Text;
using CMS.BillingService.Application.Commands.RecordPayment;
using CMS.BillingService.Application.Commands.RefundPayment;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Application.Queries.GetPayment;
using CMS.BillingService.Application.Queries.GetPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.BillingService.API.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public PaymentsController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    /// <summary>Record a payment against an invoice.</summary>
    [HttpPost]
    [Authorize(Roles = "FinanceOfficer,SuperAdmin")]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentRequest request)
    {
        var result = await _mediator.Send(new RecordPaymentCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetPayment), new { paymentId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>List payments with filters.</summary>
    [HttpGet]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetPayments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? invoiceId = null,
        [FromQuery] string? method = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var result = await _mediator.Send(new GetPaymentsQuery(page, pageSize, invoiceId, method, dateFrom, dateTo));
        return Ok(result);
    }

    /// <summary>Get payment by ID.</summary>
    [HttpGet("{paymentId:guid}")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetPayment(Guid paymentId)
    {
        var result = await _mediator.Send(new GetPaymentQuery(paymentId));
        return Ok(result);
    }

    /// <summary>Download receipt PDF for a payment.</summary>
    [HttpGet("{paymentId:guid}/receipt")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,Customer,SuperAdmin")]
    public async Task<IActionResult> GetReceipt(Guid paymentId)
    {
        var result = await _mediator.Send(new GetPaymentQuery(paymentId));
        if (!result.Success || result.Data == null) return NotFound(result);

        if (string.IsNullOrWhiteSpace(result.Data.ReceiptBlobUrl))
            return NotFound(new { message = "Receipt not available for this payment." });

        return Ok(new { receiptUrl = result.Data.ReceiptBlobUrl });
    }

    /// <summary>Issue a refund against a payment.</summary>
    [HttpPost("refund")]
    [Authorize(Roles = "FinanceOfficer,SuperAdmin")]
    public async Task<IActionResult> RefundPayment([FromBody] RefundPaymentRequest request)
    {
        var result = await _mediator.Send(new RefundPaymentCommand(request));
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>Payment gateway webhook (Stripe/Razorpay). Signature-verified, no JWT required.</summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook()
    {
        // Read raw body for signature verification
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var payload = await reader.ReadToEndAsync();

        // Verify webhook signature
        var webhookSecret = _configuration["PaymentGateway:WebhookSecret"];
        if (!string.IsNullOrWhiteSpace(webhookSecret))
        {
            var signature = Request.Headers["X-Signature"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(signature) || !VerifySignature(payload, signature, webhookSecret))
                return Unauthorized(new { message = "Invalid webhook signature." });
        }

        // Parse event type and auto-record payment
        // For MVP: log the webhook and return 200
        // In production: parse Stripe/Razorpay event and call RecordPaymentCommand
        return Ok(new { received = true });
    }

    private static bool VerifySignature(string payload, string signature, string secret)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var expected = Convert.ToHexString(hash).ToLower();
        return signature.ToLower() == expected;
    }
}
