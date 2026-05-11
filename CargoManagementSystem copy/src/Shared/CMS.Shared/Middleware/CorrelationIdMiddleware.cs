using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace CMS.Shared.Middleware;

public class CorrelationIdMiddleware
{
    public const string CorrelationIdHeader = "X-Correlation-Id";
    public const string CorrelationIdItemKey = "CorrelationId";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrGenerateCorrelationId(context);

        context.Items[CorrelationIdItemKey] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetOrGenerateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue)
            && !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString();
        }

        return Guid.NewGuid().ToString();
    }
}
