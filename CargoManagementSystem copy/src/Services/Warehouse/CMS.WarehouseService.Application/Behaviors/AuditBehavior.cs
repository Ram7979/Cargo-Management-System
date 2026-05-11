using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CMS.WarehouseService.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs an audit trail for every command (write operation)
/// passing through the Warehouse Service's request pipeline.
///
/// <para><b>Workflow:</b></para>
/// <list type="number">
///   <item>Every MediatR request flows through this behavior before reaching its handler.</item>
///   <item>Requests ending with "Query" are read-only — no audit entry is written.</item>
///   <item>For commands (ReceiveCargo, ReleaseCargo, CreateWarehouse, etc.), the actor's
///         user ID is extracted from the JWT claims in the current HTTP context.</item>
///   <item>A structured log entry is written: "AUDIT: {ActorId} executed {CommandName}".</item>
///   <item>The request is forwarded to the next behavior or the actual command handler.</item>
/// </list>
///
/// <para><b>Actor resolution:</b> Reads <c>ClaimTypes.NameIdentifier</c> from the JWT.
/// Falls back to "system" for background jobs or internal calls without an HTTP context.</para>
/// </summary>
/// <typeparam name="TRequest">The MediatR request type (command or query).</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler.</typeparam>
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of <see cref="AuditBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="logger">Serilog-backed logger for structured audit entries.</param>
    /// <param name="httpContextAccessor">Provides access to the current HTTP context for actor resolution.</param>
    public AuditBehavior(
        ILogger<AuditBehavior<TRequest, TResponse>> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Intercepts the pipeline to write an audit log entry for commands.
    /// Queries are skipped — only write operations are audited.
    /// </summary>
    /// <param name="request">The incoming MediatR request.</param>
    /// <param name="next">Delegate to invoke the next behavior or the actual handler.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The response produced by the downstream handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Only audit commands — skip queries (read-only operations)
        if (!requestName.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
        {
            // Resolve actor from JWT; fall back to "system" for background/internal calls
            var actorId = _httpContextAccessor.HttpContext?.User?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system";

            _logger.LogInformation("AUDIT: {Actor} executed {Command}", actorId, requestName);
        }

        return await next(cancellationToken);
    }
}
