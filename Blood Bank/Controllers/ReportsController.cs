using BloodBank.Core.Entities;
using BloodBank.Infrastructure.Data;
using Blood_Bank.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Blood_Bank.ViewModels.Blood_Bank.ViewModels;
using BloodBank.Core.Enums;

namespace BloodBank.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly BloodBankDbContext _context;

        public ReportsController ( BloodBankDbContext context )
        {
            _context = context;
        }

        public async Task<IActionResult> Index ( ReportFilterViewModel filter )
        {
            DateTime now = DateTime.Now;
            DateTime start, end;

            // Determine the date range based on the selected period
            switch ( filter.Period?.ToLower() )
            {
                case "daily":
                    start = now.Date;
                    end = start.AddDays( 1 );
                    break;
                case "weekly":
                    int diff = ( 7 + ( now.DayOfWeek - DayOfWeek.Monday ) ) % 7;
                    start = now.AddDays( -1 * diff ).Date;
                    end = start.AddDays( 7 );
                    break;
                case "monthly":
                    start = new DateTime( now.Year, now.Month, 1 );
                    end = start.AddMonths( 1 );
                    break;
                case "quarter":
                    int quarter = ( now.Month - 1 ) / 3 + 1;
                    start = new DateTime( now.Year, ( quarter - 1 ) * 3 + 1, 1 );
                    end = start.AddMonths( 3 );
                    break;
                case "yearly":
                    start = new DateTime( now.Year, 1, 1 );
                    end = start.AddYears( 1 );
                    break;
                case "custom":
                    start = filter.CustomFrom ?? now.Date;
                    end = ( filter.CustomTo?.AddDays( 1 ) ) ?? now.Date.AddDays( 1 );
                    break;
                default:
                    start = new DateTime( now.Year, now.Month, 1 );
                    end = start.AddMonths( 1 );
                    break;
            }

            // Fetch data from the database within the specified date range
            var donations = await _context.Donations
                .Where( d => d.DonationDate >= start && d.DonationDate < end )
                .Include( d => d.Donor )
                .Include( d => d.Hospital )
                .ToListAsync();

            var bloodRequests = await _context.BloodRequests
                .Where( r => r.RequestDate >= start && r.RequestDate < end )
                .Include( r => r.Hospital )
                .Include( r => r.AssignedUnits )
                .ToListAsync();

            var bloodTests = await _context.BloodTests
                .Where( t => t.TestDate >= start && t.TestDate < end )
                .Include( t => t.Donor )
                .Include( t => t.Hospital )
                .ToListAsync();

            var bloodUnits = await _context.BloodUnits
                .Where( u => u.CollectionDate >= start && u.CollectionDate < end )
                .Include( u => u.Donation )
                .ToListAsync();

            // Calculate summary metrics
            int totalDonations = donations.Count;
            int totalBloodUnits = bloodUnits.Count;
            int totalPassedTests = bloodTests.Count( t => t.IsTestPassed );
            int totalFailedTests = bloodTests.Count( t => !t.IsTestPassed );
            int totalRequests = bloodRequests.Count;
            int fulfilledRequests = bloodRequests.Count( r => r.Status == RequestStatus.Fulfilled ); // Adjust if enum differs

            // Prepare chart data for blood type distribution
            var bloodTypeLabels = donations.Select( d => d.BloodType.ToString() ).Distinct().ToList();
            var bloodTypeCounts = donations.GroupBy( d => d.BloodType )
                                           .Select( g => g.Count() )
                                           .ToList();

            // Serialize chart data to JSON strings
            string bloodTypeLabelsJson = JsonSerializer.Serialize( bloodTypeLabels );
            string bloodTypeCountsJson = JsonSerializer.Serialize( bloodTypeCounts );

            // Create and populate the ReportViewModel
            var report = new ReportViewModel
            {
                From = start,
                To = end,
                Period = filter.Period,
                TotalDonations = totalDonations,
                TotalBloodUnits = totalBloodUnits,
                TotalPassedTests = totalPassedTests,
                TotalFailedTests = totalFailedTests,
                TotalRequests = totalRequests,
                FulfilledRequests = fulfilledRequests,
                Donations = donations,
                BloodRequests = bloodRequests,
                BloodTests = bloodTests,
                BloodUnits = bloodUnits,
                BloodTypeLabels = bloodTypeLabelsJson,
                BloodTypeCounts = bloodTypeCountsJson
            };

            // Pass the populated model to the view
            return View( report );
        }
    }
}