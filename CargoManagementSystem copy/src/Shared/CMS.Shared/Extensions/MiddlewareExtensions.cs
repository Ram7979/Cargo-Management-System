using CMS.Shared.Middleware;
using Microsoft.AspNetCore.Builder;

namespace CMS.Shared.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<GlobalExceptionMiddleware>();

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationIdMiddleware>();
}
