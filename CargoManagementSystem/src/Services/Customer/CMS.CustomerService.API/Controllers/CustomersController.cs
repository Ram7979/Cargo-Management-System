using System.Security.Claims;
using CMS.CustomerService.Application.Commands.AddKycDocument;
using CMS.CustomerService.Application.Commands.DeactivateCustomer;
using CMS.CustomerService.Application.Commands.DeleteKycDocument;
using CMS.CustomerService.Application.Commands.RegisterCustomer;
using CMS.CustomerService.Application.Commands.UpdateCustomer;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Application.Queries.GetCustomer;
using CMS.CustomerService.Application.Queries.GetCustomers;
using CMS.CustomerService.Application.Queries.GetCustomerShipments;
using CMS.CustomerService.Application.Queries.GetKycDocuments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.CustomerService.API.Controllers;

[ApiController]
[Route("api/v1/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Register a new customer (self-registration or by staff).</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest request)
    {
        var result = await _mediator.Send(new RegisterCustomerCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetCustomer), new { customerId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Get paginated list of customers with search and filters.</summary>
    [HttpGet]
    [Authorize(Roles = "OpsManager,Support,SuperAdmin,FinanceOfficer")]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? type = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetCustomersQuery(page, pageSize, search, type, isActive));
        return Ok(result);
    }

    /// <summary>Get customer by ID.</summary>
    [HttpGet("{customerId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCustomer(Guid customerId)
    {
        var result = await _mediator.Send(new GetCustomerQuery(customerId));
        return Ok(result);
    }

    /// <summary>Update customer details including type, credit limit, payment terms.</summary>
    [HttpPut("{customerId:guid}")]
    [Authorize(Roles = "OpsManager,Support,SuperAdmin,FinanceOfficer")]
    public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromBody] UpdateCustomerRequest request)
    {
        var result = await _mediator.Send(new UpdateCustomerCommand(customerId, request));
        return Ok(result);
    }

    /// <summary>Deactivate (soft-delete) a customer. GDPR right-to-erasure.</summary>
    [HttpDelete("{customerId:guid}")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> DeactivateCustomer(Guid customerId)
    {
        var result = await _mediator.Send(new DeactivateCustomerCommand(customerId));
        return Ok(result);
    }

    /// <summary>Get customer shipment history with filters.</summary>
    [HttpGet("{customerId:guid}/shipments")]
    [Authorize]
    public async Task<IActionResult> GetCustomerShipments(
        Guid customerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? serviceType = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] string? format = null)
    {
        // Customer role can only see their own shipments
        Guid? callerCustomerId = null;
        if (User.IsInRole("Customer"))
        {
            var customerIdClaim = User.FindFirst("customerId")?.Value;
            if (Guid.TryParse(customerIdClaim, out var parsedId))
                callerCustomerId = parsedId;
        }

        var result = await _mediator.Send(new GetCustomerShipmentsQuery(
            customerId, page, pageSize, callerCustomerId,
            status, serviceType, dateFrom, dateTo, format));
        return Ok(result);
    }

    /// <summary>List KYC documents for a customer.</summary>
    [HttpGet("{customerId:guid}/documents")]
    [Authorize(Roles = "OpsManager,Support,SuperAdmin")]
    public async Task<IActionResult> GetKycDocuments(Guid customerId)
    {
        var result = await _mediator.Send(new GetKycDocumentsQuery(customerId));
        return Ok(result);
    }

    /// <summary>Add a KYC document (blob reference) to a customer.</summary>
    [HttpPost("{customerId:guid}/documents")]
    [Authorize(Roles = "OpsManager,Support,SuperAdmin")]
    public async Task<IActionResult> AddKycDocument(Guid customerId, [FromBody] KycDocumentRequest request)
    {
        var result = await _mediator.Send(new AddKycDocumentCommand(
            customerId, request.DocumentType, request.BlobReference));
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>Remove a KYC document from a customer.</summary>
    [HttpDelete("{customerId:guid}/documents/{documentId:guid}")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> DeleteKycDocument(Guid customerId, Guid documentId)
    {
        var result = await _mediator.Send(new DeleteKycDocumentCommand(customerId, documentId));
        return Ok(result);
    }
}
