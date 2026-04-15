using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetDriverPerformance;

public class GetDriverPerformanceQueryHandler : IRequestHandler<GetDriverPerformanceQuery, ApiResponse<DriverPerformanceDto>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IAssignmentRepository _assignmentRepository;

    public GetDriverPerformanceQueryHandler(IDriverRepository driverRepository, IAssignmentRepository assignmentRepository)
    {
        _driverRepository = driverRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<ApiResponse<DriverPerformanceDto>> Handle(GetDriverPerformanceQuery request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(request.DriverId)
            ?? throw new NotFoundException("Driver", request.DriverId);

        var (assignments, _) = await _assignmentRepository.GetPagedAsync(
            1, int.MaxValue, null, request.DriverId, null, null, request.DateFrom, request.DateTo);

        var assignmentList = assignments.ToList();
        var completed = assignmentList.Count(a => a.Status == AssignmentStatus.Completed);
        var cancelled = assignmentList.Count(a => a.Status == AssignmentStatus.Cancelled);
        var total = assignmentList.Count;

        var dto = new DriverPerformanceDto
        {
            DriverId = driver.Id,
            EmployeeId = driver.EmployeeId,
            TotalAssignments = total,
            CompletedDeliveries = completed,
            FailedDeliveries = cancelled,
            TotalDeliveries = completed,
            OnTimeRate = total > 0 ? Math.Round((double)completed / total * 100, 1) : 0,
            FailureRate = total > 0 ? Math.Round((double)cancelled / total * 100, 1) : 0,
            FromDate = request.DateFrom,
            ToDate = request.DateTo
        };

        return ApiResponse<DriverPerformanceDto>.Ok(dto);
    }
}
