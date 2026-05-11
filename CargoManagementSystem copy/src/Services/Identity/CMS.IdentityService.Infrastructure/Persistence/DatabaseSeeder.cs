using CMS.IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedRolesAndAdminAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        // Seed roles
        string[] roles = { "SuperAdmin", "OpsManager", "Dispatcher", "Driver", "FleetManager",
                           "WarehouseManager", "FinanceOfficer", "Support", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed SuperAdmin user
        var adminEmail = "superadmin@cms.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
            }
        }

        // Seed a test customer
        var customerEmail = "customer@cms.com";
        var customerUser = await userManager.FindByEmailAsync(customerEmail);

        if (customerUser == null)
        {
            customerUser = new ApplicationUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                FirstName = "Test",
                LastName = "Customer",
                IsActive = true,
                EmailConfirmed = true,
                CompanyName = "Test Corp",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(customerUser, "Customer@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customerUser, "Customer");
            }
        }
    }
}
