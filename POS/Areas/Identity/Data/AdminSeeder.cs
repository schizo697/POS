using Microsoft.AspNetCore.Identity;

namespace POS.Areas.Identity.Data
{
    public class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string adminEmail = "squareone@gmail.com";
                string adminPassword = "@SquareOne1";
                string adminRole = "Admin";

                // Ensure roles exist
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(adminRole));
                }

                // Check if the admin user already exists
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "Square",
                        LastName = "One",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, adminRole);
                    }
                }
            }
        }
    }
}
