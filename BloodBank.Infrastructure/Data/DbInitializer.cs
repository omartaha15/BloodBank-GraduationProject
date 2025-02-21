using BloodBank.Core.Constants;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBank.Infrastructure.Data
{

    public static class DbInitializer
    {
        public static async Task Initialize ( IServiceProvider serviceProvider )
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Create roles
            foreach ( var role in Roles.All )
            {
                if ( !await roleManager.RoleExistsAsync( role ) )
                {
                    await roleManager.CreateAsync( new IdentityRole( role ) );
                }
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@bloodbank.com";
            if ( await userManager.FindByEmailAsync( adminEmail ) == null )
            {
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    DateOfBirth = DateTime.Now.AddYears( -25 ),
                    Address = "Admin Address",
                    Gender = Gender.Male,
                    BloodType = BloodType.APositive
                };

                var result = await userManager.CreateAsync( admin, "Admin@123" );
                if ( result.Succeeded )
                {
                    await userManager.AddToRoleAsync( admin, Roles.Admin );
                }
            }
        }
    }
}
