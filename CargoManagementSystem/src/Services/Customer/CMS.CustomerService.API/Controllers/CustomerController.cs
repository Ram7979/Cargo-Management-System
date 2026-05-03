using System.Security.Claims;
using CMS.CustomerService.Application.DTOs;
using CMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.CustomerService.API.Controllers;

[ApiController]
[Route("api/v1/customer")]
[Authorize(Roles = "Customer")]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ILogger<CustomerController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets a summary of the customer's dashboard stats.
    /// </summary>
    [HttpGet("dashboard/summary")]
    public async Task<ActionResult<ApiResponse<CustomerDashboardSummaryDto>>> GetDashboardSummary()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // In a real app, you'd lookup the CustomerId from the UserId and then query shipments/invoices
            
            var summary = new CustomerDashboardSummaryDto
            {
                ActiveShipments = 2,
                DeliveredThisMonth = 5,
                TotalShipments = 12,
                PendingInvoicesCount = 1,
                PendingInvoicesAmount = 1500.00m,
                OverdueInvoicesCount = 0,
                RecentShipments = new List<ShipmentSummaryDto>
                {
                    new() { TrackingNumber = "CMS-2025-0001", Origin = "Mumbai", Destination = "Dubai", Status = "In Transit", ServiceType = "Express", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                    new() { TrackingNumber = "CMS-2025-0002", Origin = "London", Destination = "New York", Status = "Pending", ServiceType = "Standard", CreatedAt = DateTime.UtcNow.AddDays(-1) }
                },
                RecentNotifications = new List<NotificationDto>
                {
                    new() { Id = "1", Message = "Your shipment CMS-2025-0001 is in transit.", Type = "ShipmentUpdate", CreatedAt = DateTime.UtcNow.AddHours(-2), IsRead = false }
                }
            };

            return Ok(ApiResponse<CustomerDashboardSummaryDto>.Ok(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard summary");
            return StatusCode(500, ApiResponse<CustomerDashboardSummaryDto>.Fail("Internal server error."));
        }
    }

    /// <summary>
    /// Gets a paginated list of shipments for the current customer.
    /// </summary>
    [HttpGet("shipments")]
    public async Task<ActionResult<ApiResponse<object>>> GetShipments([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Logic to fetch shipments for this user/customer
        return Ok(ApiResponse<object>.Ok(new { items = new List<object>(), totalCount = 0, page, limit }));
    }

    /// <summary>
    /// Books a new shipment for the current customer.
    /// </summary>
    [HttpPost("shipments")]
    public async Task<ActionResult<ApiResponse<object>>> BookShipment([FromBody] object dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Logic to create shipment
        var trackingNumber = $"CMS-{DateTime.Now.Year}-{new Random().Next(100000, 999999)}";
        return StatusCode(201, ApiResponse<object>.Ok(new { trackingNumber, shipmentId = Guid.NewGuid(), estimatedDelivery = DateTime.UtcNow.AddDays(5), invoiceNumber = "INV-2025-0001" }));
    }

    /// <summary>
    /// Gets a list of invoices for the current customer.
    /// </summary>
    [HttpGet("invoices")]
    public async Task<ActionResult<ApiResponse<object>>> GetInvoices([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        return Ok(ApiResponse<object>.Ok(new { items = new List<object>(), totalCount = 0, page, limit }));
    }

    /// <summary>
    /// Gets the full profile of the current customer.
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<object>>> GetProfile()
    {
        return Ok(ApiResponse<object>.Ok(new { firstName = "John", lastName = "Doe", email = "john@example.com" }));
    }
}
