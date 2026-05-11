using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PagedResponse<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository, IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<AuditLogDto>> Handle(GetAuditLogsQuery query, CancellationToken cancellationToken)
    {
        var logs = await _auditLogRepository.GetByActorAsync(query.ActorId, query.Page, query.PageSize);
        var logDtos = _mapper.Map<IEnumerable<AuditLogDto>>(logs);

        // Note: total count would ideally come from a count query; using enumerated count as approximation
        var logList = logDtos.ToList();
        return PagedResponse<AuditLogDto>.Ok(logList, query.Page, query.PageSize, logList.Count);
    }
}
