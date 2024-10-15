using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Identity_final_attempt.Data;
namespace Identity_final_attempt
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Define roles to be added
            string[] roleNames = { "Admin", "User", "Manager" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                // Check if the role already exists
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    // Create the role if it doesn't exist
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Optionally: Assign an Admin role to a specific user
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");  // Replace with your admin user email
            if (adminUser != null)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

}
