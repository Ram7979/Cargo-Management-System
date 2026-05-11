using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetAssignments;

public record GetAssignmentsQuery(
    int Page, int PageSize,
    string? Status = null,
    Guid? DriverId = null,
    Guid? VehicleId = null,
    Guid? ShipmentId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResponse<AssignmentDto>>;
