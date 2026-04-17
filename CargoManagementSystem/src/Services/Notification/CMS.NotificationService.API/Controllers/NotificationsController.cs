using System.Security.Claims;
using CMS.NotificationService.Application.Commands.DeleteNotification;
using CMS.NotificationService.Application.Commands.MarkAllNotificationsRead;
using CMS.NotificationService.Application.Commands.MarkNotificationRead;
using CMS.NotificationService.Application.Commands.QueueNotification;
using CMS.NotificationService.Application.Commands.UpdatePreference;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Application.Queries.GetFailedNotifications;
using CMS.NotificationService.Application.Queries.GetNotification;
using CMS.NotificationService.Application.Queries.GetNotifications;
using CMS.NotificationService.Application.Queries.GetPreference;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.NotificationService.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string CallerUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    // ─────────────────────────────────────────────────────────────────────────
    // Notification CRUD
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>List notifications with full filter set.</summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,OpsManager,Support")]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? channel = null,
        [FromQuery] string? recipientId = null,
        [FromQuery] string? eventType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = await _mediator.Send(
            new GetNotificationsQuery(page, pageSize, status, channel, recipientId, eventType, fromDate, toDate));
        return Ok(result);
    }

    /// <summary>Get a single notification by ID.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,OpsManager,Support")]
    public async Task<IActionResult> GetNotification(Guid id)
    {
        var result = await _mediator.Send(new GetNotificationQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Queue a notification (internal API — called by other services).
    /// Accepts Email, SMS, or Push channels.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> QueueNotification([FromBody] QueueNotificationRequest request)
    {
        var result = await _mediator.Send(new QueueNotificationCommand(request));
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>Mark a single notification as read.</summary>
    [HttpPatch("{id:guid}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _mediator.Send(new MarkNotificationReadCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Mark all notifications as read for the calling user.</summary>
    [HttpPatch("read-all")]
    [Authorize]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _mediator.Send(new MarkAllNotificationsReadCommand(CallerUserId));
        return Ok(result);
    }

    /// <summary>Soft-delete a notification (GDPR erasure).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        var result = await _mediator.Send(new DeleteNotificationCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Failed notifications — support queue
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>List all failed notifications for manual review.</summary>
    [HttpGet("failed")]
    [Authorize(Roles = "SuperAdmin,OpsManager,Support")]
    public async Task<IActionResult> GetFailedNotifications()
    {
        var result = await _mediator.Send(new GetFailedNotificationsQuery());
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Notification preferences
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Get notification preferences for the calling user.</summary>
    [HttpGet("preferences")]
    [Authorize]
    public async Task<IActionResult> GetPreferences()
    {
        var result = await _mediator.Send(new GetPreferenceQuery(CallerUserId));
        return Ok(result);
    }

    /// <summary>Update notification preferences for the calling user.</summary>
    [HttpPut("preferences")]
    [Authorize]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferenceRequest request)
    {
        var result = await _mediator.Send(new UpdatePreferenceCommand(CallerUserId, request));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Convenience endpoints called by other services
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Warehouse arrival notification (called by Warehouse Service).</summary>
    [HttpPost("warehouse-arrival")]
    [Authorize]
    public async Task<IActionResult> SendWarehouseArrival([FromBody] QueueNotificationRequest request)
    {
        request.EventType = "WarehouseArrival";
        var result = await _mediator.Send(new QueueNotificationCommand(request));
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>Report-ready notification (called by Reporting Service).</summary>
    [HttpPost("report-ready")]
    [Authorize]
    public async Task<IActionResult> SendReportReady([FromBody] QueueNotificationRequest request)
    {
        request.EventType = "ReportReady";
        var result = await _mediator.Send(new QueueNotificationCommand(request));
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }
}
