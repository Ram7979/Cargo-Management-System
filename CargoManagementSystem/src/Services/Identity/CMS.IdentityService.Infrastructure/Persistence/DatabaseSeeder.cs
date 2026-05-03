using CMS.IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        string[] roles = { "SuperAdmin", "Admin", "OperationsManager", "WarehouseManager", "Driver", "Customer", "Finance", "Dispatcher", "SupportAgent" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create a default SuperAdmin if not exists
        var adminEmail = "superadmin@cms.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
            }
        }
    }
}
