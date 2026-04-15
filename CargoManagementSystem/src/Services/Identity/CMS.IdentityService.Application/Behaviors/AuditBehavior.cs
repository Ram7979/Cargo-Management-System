using System.Security.Claims;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CMS.IdentityService.Application.Behaviors;

public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditBehavior(IAuditLogRepository auditLogRepository, IHttpContextAccessor httpContextAccessor)
    {
        _auditLogRepository = auditLogRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only audit commands (not queries) — queries have "Query" in their name
        var requestName = typeof(TRequest).Name;
        var isQuery = requestName.EndsWith("Query", StringComparison.OrdinalIgnoreCase);

        if (isQuery)
            return await next(cancellationToken);

        var response = await next(cancellationToken);

        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var actorId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";

            var auditLog = AuditLog.Create(
                actorId: actorId,
                action: requestName,
                resourceType: "Command",
                resourceId: string.Empty,
                ipAddress: ipAddress,
                payload: string.Empty);

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch
        {
            // Audit logging should never break the main flow
        }

        return response;
    }
}
