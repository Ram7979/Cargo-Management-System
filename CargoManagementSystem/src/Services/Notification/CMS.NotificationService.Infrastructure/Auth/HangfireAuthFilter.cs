using Hangfire.Dashboard;

namespace CMS.NotificationService.Infrastructure.Auth;

/// <summary>
/// Restricts Hangfire dashboard access to authenticated users with SuperAdmin or OpsManager roles.
/// </summary>
public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (!httpContext.User.Identity?.IsAuthenticated == true)
            return false;

        return httpContext.User.IsInRole("SuperAdmin") ||
               httpContext.User.IsInRole("OpsManager");
    }
}
