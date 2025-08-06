using MassTransit;
using Messaging;
using Microsoft.AspNetCore.Identity;
using UserService.Data.Models;

namespace UserService.Helpers
{
    public static class RoleInitializer
    {
        private static readonly string[] Roles = ["Admin", "User", "Moderator"];

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task RegisterAdminAccount(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IPublishEndpoint publish)
        {
            var adminEmail = "admin@email.com";
            var adminPassword = "P@ssw0rd123";

            var user = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
            var result = await userManager.CreateAsync(user, adminPassword);

            if (result.Succeeded)
            {
                string roleName = "Admin";

                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                await userManager.AddToRoleAsync(user, roleName);

                var userAspNet = new UserRegistered
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };

                await publish.Publish(userAspNet);
            }

        }
    }
}
