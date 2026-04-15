using CMS.IdentityService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Queries.GetAuditLogs;

public class GetAuditLogsQuery : IRequest<PagedResponse<AuditLogDto>>
{
    public string ActorId { get; }
    public int Page { get; }
    public int PageSize { get; }

    public GetAuditLogsQuery(string actorId, int page, int pageSize)
    {
        ActorId = actorId;
        Page = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? 1 : pageSize > 100 ? 100 : pageSize;
    }
}
