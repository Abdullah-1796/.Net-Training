using EF_Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;
using YourNamespace.Data;

namespace EF_Core.Data
{
    public class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            // Ensure database is created
            await dbContext.Database.EnsureCreatedAsync();

            // Seed roles
            string[] roles = new[] {"Admin", "Manager", "Receptionist"};
            string[] descriptions = new[] 
            { 
                "Administrator with full access", 
                "Manager with limited access", 
                "Receptionist with basic access" 
            };

            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    ApplicationRole applicationRole = new ApplicationRole()
                    {
                        Name = role,
                        Description = descriptions[Array.IndexOf(roles, role)]
                    };
                    await roleManager.CreateAsync(applicationRole);
                }
            }

            // Seed users
            string[] names = new[] { "Abdullah Zahid", "Default Manager", "Default Receptionist" };
            string[] cnics = new[] { "3520160501549", "3520160501548", "3520160501547" };
            string[] emails = new[] { "az@gmail.com", "dm@gmail.com", "dr@gmail.com" };
            string[] phones = new[] { "03227888444", "03004146694", "03214210710" };
            string defaultPassword = "Password@123";

            foreach(var cnic in cnics)
            {
                if(await userManager.FindByNameAsync(cnic) == null)
                {
                    int index = Array.IndexOf(cnics, cnic);
                    ApplicationUser user = new ApplicationUser()
                    {
                        UserName = cnic,
                        Name = names[index],
                        Email = emails[index],
                        PhoneNumber = phones[index],
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, defaultPassword);
                    if(!result.Succeeded)
                    {
                        Console.WriteLine("Users not seeded");
                        return;
                    }
                }
            }
            await userManager.AddToRoleAsync(await userManager.FindByNameAsync(cnics[0]), roles[0]);
            await userManager.AddToRoleAsync(await userManager.FindByNameAsync(cnics[1]), roles[1]);
            await userManager.AddToRoleAsync(await userManager.FindByNameAsync(cnics[2]), roles[2]);
        }
    }
}
