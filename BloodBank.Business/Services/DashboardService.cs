using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Constants;
using BloodBank.Core.Entities;
using BloodBank.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBank.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<User> _userManager;
        private readonly IDonationRepository _donationRepository;
        private readonly IBloodInventoryService _inventoryService;

        public DashboardService (
            UserManager<User> userManager,
            IDonationRepository donationRepository,
            IBloodInventoryService inventoryService )
        {
            _userManager = userManager;
            _donationRepository = donationRepository;
            _inventoryService = inventoryService;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync ()
        {
            var users = await _userManager.Users.ToListAsync();
            var donors = await _userManager.GetUsersInRoleAsync( Roles.Donor );
            var donations = await _donationRepository.GetAllAsync();
            var pendingAppointments = donations.Where( d => d.AppointmentDate >= DateTime.UtcNow ).ToList();
            var bloodTypeStats = await _inventoryService.GetBloodTypeStatsAsync();

            var stats = new DashboardStatsDto
            {
                TotalUsers = users.Count,
                TotalDonors = donors.Count,
                TotalDonations = donations.Count(),
                PendingAppointments = pendingAppointments.Count,
                BloodTypeStats = bloodTypeStats.Select( s => new BloodTypeStatDto
                {
                    BloodType = s.Key,
                    Count = s.Value,
                    Percentage = bloodTypeStats.Sum( x => x.Value ) > 0 ? ( double ) s.Value / bloodTypeStats.Sum( x => x.Value ) * 100 : 0
                } ).ToList(),
                RecentActivities = await GetRecentActivitiesAsync()
            };

            return stats;
        }

        // Keep GetBloodTypeStatsAsync for compatibility if needed
        public async Task<List<BloodTypeStatDto>> GetBloodTypeStatsAsync ()
        {
            var stats = await _inventoryService.GetBloodTypeStatsAsync();
            return stats.Select( s => new BloodTypeStatDto
            {
                BloodType = s.Key,
                Count = s.Value,
                Percentage = stats.Sum( x => x.Value ) > 0 ? ( double ) s.Value / stats.Sum( x => x.Value ) * 100 : 0
            } ).ToList();
        }

        public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync ( int count = 10 )
        {
            var activities = new List<RecentActivityDto>();
            var recentDonations = await _donationRepository.GetRecentDonationsAsync( count );
            foreach ( var donation in recentDonations )
            {
                activities.Add( new RecentActivityDto
                {
                    ActivityType = "Donation",
                    Description = $"New donation by {donation.Donor.FirstName} {donation.Donor.LastName}",
                    Timestamp = donation.DonationDate,
                    PerformedBy = $"{donation.Donor.FirstName} {donation.Donor.LastName}"
                } );
            }

            var upcomingAppointments = ( await _donationRepository.GetAllAsync() )
                .Where( d => d.AppointmentDate >= DateTime.UtcNow )
                .OrderBy( d => d.AppointmentDate )
                .Take( count )
                .ToList();

            foreach ( var appointment in upcomingAppointments )
            {
                activities.Add( new RecentActivityDto
                {
                    ActivityType = "Appointment",
                    Description = $"Upcoming appointment for {appointment.Donor.FirstName} {appointment.Donor.LastName}",
                    Timestamp = appointment.AppointmentDate,
                    PerformedBy = $"{appointment.Donor.FirstName} {appointment.Donor.LastName}"
                } );
            }

            return activities.OrderByDescending( a => a.Timestamp ).Take( count ).ToList();
        }
    }
}