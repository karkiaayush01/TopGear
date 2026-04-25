using Microsoft.AspNetCore.Identity;

namespace TopGear.Infrastructure.Data;

public class AppDbSeeder(RoleManager<IdentityRole<Guid>> roleManager)
{
    public async Task SeedRolesAsync()
    {
        string[] roles = { "Admin", "Staff", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
