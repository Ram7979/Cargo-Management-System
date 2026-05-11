using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetDriverActiveAssignment;

public class GetDriverActiveAssignmentQueryHandler : IRequestHandler<GetDriverActiveAssignmentQuery, ApiResponse<AssignmentDto>>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public GetDriverActiveAssignmentQueryHandler(IAssignmentRepository assignmentRepository, IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AssignmentDto>> Handle(GetDriverActiveAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetActiveByDriverIdAsync(request.DriverId)
            ?? throw new NotFoundException("Active assignment for driver", request.DriverId);

        var dto = _mapper.Map<AssignmentDto>(assignment);
        return ApiResponse<AssignmentDto>.Ok(dto);
    }
}
