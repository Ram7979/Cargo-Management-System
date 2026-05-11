using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetAssignments;

public class GetAssignmentsQueryHandler : IRequestHandler<GetAssignmentsQuery, PagedResponse<AssignmentDto>>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public GetAssignmentsQueryHandler(IAssignmentRepository assignmentRepository, IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<AssignmentDto>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _assignmentRepository.GetPagedAsync(
            request.Page, request.PageSize,
            request.Status, request.DriverId, request.VehicleId, request.ShipmentId,
            request.DateFrom, request.DateTo);

        var dtos = _mapper.Map<IEnumerable<AssignmentDto>>(items);
        return PagedResponse<AssignmentDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
    }
}
