using BloodBank.Core.Constants;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBank.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize ( IServiceProvider serviceProvider )
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var context = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
            var random = new Random();

            // 1. Create Roles
            foreach ( var role in Roles.All )
            {
                if ( !await roleManager.RoleExistsAsync( role ) )
                    await roleManager.CreateAsync( new IdentityRole( role ) );
            }

            // 2. Create Admin
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
                    DateOfBirth = DateTime.Now.AddYears( -30 ),
                    Address = "Admin Headquarters",
                    Gender = Gender.Male,
                    BloodType = BloodType.APositive
                };

                var result = await userManager.CreateAsync( admin, "Admin@123" );
                if ( result.Succeeded )
                    await userManager.AddToRoleAsync( admin, Roles.Admin );
            }

            // 3. Seed 10 Donors
            var donors = new List<User>();
            for ( int i = 1; i <= 10; i++ )
            {
                string email = $"donor{i}@mail.com";
                var donor = await userManager.FindByEmailAsync( email );
                if ( donor == null )
                {
                    donor = new User
                    {
                        UserName = email,
                        Email = email,
                        FirstName = $"Donor{i}",
                        LastName = "Test",
                        EmailConfirmed = true,
                        PhoneNumber = $"010000000{i:D2}",
                        DateOfBirth = DateTime.Now.AddYears( -random.Next( 20, 40 ) ),
                        Address = $"Donor Street {i}",
                        Gender = i % 2 == 0 ? Gender.Male : Gender.Female,
                        BloodType = ( BloodType ) ( i % 8 )
                    };

                    var result = await userManager.CreateAsync( donor, "Donor@123" );
                    if ( result.Succeeded )
                        await userManager.AddToRoleAsync( donor, Roles.Donor );
                }

                donors.Add( donor );
            }

            // 4. Seed 5 Hospitals
            var hospitals = new List<User>();
            for ( int i = 1; i <= 5; i++ )
            {
                string email = $"hospital{i}@mail.com";
                var hospital = await userManager.FindByEmailAsync( email );
                if ( hospital == null )
                {
                    hospital = new User
                    {
                        UserName = email,
                        Email = email,
                        FirstName = $"Hospital{i}",
                        LastName = "Center",
                        EmailConfirmed = true,
                        PhoneNumber = $"020000000{i}",
                        DateOfBirth = DateTime.Now.AddYears( -random.Next( 30, 60 ) ),
                        Address = $"Hospital District {i}",
                        Gender = Gender.Female,
                        BloodType = ( BloodType ) ( i % 8 )
                    };

                    var result = await userManager.CreateAsync( hospital, "Hospital@123" );
                    if ( result.Succeeded )
                        await userManager.AddToRoleAsync( hospital, Roles.Hospital );
                }

                hospitals.Add( hospital );
            }

            // 5. Seed 70 Donations + BloodUnits
            var donations = new List<Donation>();
            var bloodUnits = new List<BloodUnit>();
            for ( int i = 1; i <= 70; i++ )
            {
                var donor = donors [ i % donors.Count ];
                var hospital = hospitals [ i % hospitals.Count ];
                var donationDate = DateTime.Now.AddDays( -i );

                var donation = new Donation
                {
                    DonorId = donor.Id,
                    HospitalId = hospital.Id,
                    DonationDate = donationDate,
                    AppointmentDate = donationDate.AddDays( -1 ),
                    AppointmentTime = TimeSpan.FromHours( 9 + ( i % 5 ) ),
                    AppointmentNotes = "Scheduled donation",
                    Quantity = 450,
                    BloodType = donor.BloodType,
                    Status = DonationStatus.Approved
                };

                var bloodUnit = new BloodUnit
                {
                    UnitNumber = $"UNIT-{i:D5}",
                    BloodType = donor.BloodType,
                    Quantity = 450,
                    CollectionDate = donationDate,
                    ExpiryDate = donationDate.AddDays( 42 ),
                    Status = BloodUnitStatus.Available,
                    StorageLocation = $"Fridge-{i % 3 + 1}",
                    Donation = donation
                };

                donation.BloodUnit = bloodUnit;

                donations.Add( donation );
                bloodUnits.Add( bloodUnit );
            }

            context.Donations.AddRange( donations );
            context.BloodUnits.AddRange( bloodUnits );

            // 6. Seed 10 BloodRequests
            var requests = new List<BloodRequest>();
            for ( int i = 1; i <= 10; i++ )
            {
                var hospital = hospitals [ i % hospitals.Count ];

                var request = new BloodRequest
                {
                    HospitalId = hospital.Id,
                    BloodType = ( BloodType ) ( i % 8 ),
                    QuantityRequired = 2,
                    Priority = ( RequestPriority ) ( i % 3 ),
                    Status = RequestStatus.Pending,
                    RequiredDate = DateTime.Now.AddDays( 3 + i ),
                    RequestDate = DateTime.Now,
                    Notes = $"Request for patient {i}"
                };

                requests.Add( request );
            }

            context.BloodRequests.AddRange( requests );

            // 7. Seed 10 BloodTests
            var tests = new List<BloodTest>();
            for ( int i = 1; i <= 10; i++ )
            {
                var donor = donors [ i % donors.Count ];
                var hospital = hospitals [ i % hospitals.Count ];

                var test = new BloodTest
                {
                    DonorId = donor.Id,
                    HospitalId = hospital.Id,
                    HivTest = false,
                    HepatitisB = false,
                    HepatitisC = false,
                    Syphilis = false,
                    Malaria = false,
                    OtherTestNotes = "All tests passed.",
                    IsTestPassed = true,
                    TestDate = DateTime.Now.AddDays( -random.Next( 10, 100 ) ),
                    HospitalApprovalStatus = HospitalApprovalStatus.Approved
                };

                tests.Add( test );
            }

            context.BloodTests.AddRange( tests );

            await context.SaveChangesAsync();
        }
    }
}
