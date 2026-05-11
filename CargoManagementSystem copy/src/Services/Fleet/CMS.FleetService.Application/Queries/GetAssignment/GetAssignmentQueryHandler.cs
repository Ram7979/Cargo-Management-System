using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Exceptions;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetAssignment;

public class GetAssignmentQueryHandler : IRequestHandler<GetAssignmentQuery, ApiResponse<AssignmentDto>>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public GetAssignmentQueryHandler(IAssignmentRepository assignmentRepository, IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AssignmentDto>> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId)
            ?? throw new AssignmentNotFoundException($"Assignment '{request.AssignmentId}' not found.");

        var dto = _mapper.Map<AssignmentDto>(assignment);
        return ApiResponse<AssignmentDto>.Ok(dto);
    }
}
