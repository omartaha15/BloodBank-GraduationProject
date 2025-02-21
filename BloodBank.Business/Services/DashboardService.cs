using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Constants;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<User> _userManager;
        private readonly IDonationRepository _donationRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IMapper _mapper;

        public DashboardService (
            UserManager<User> userManager,
            IDonationRepository donationRepository,
            IAppointmentRepository appointmentRepository,
            IBloodUnitRepository bloodUnitRepository,
            IMapper mapper )
        {
            _userManager = userManager;
            _donationRepository = donationRepository;
            _appointmentRepository = appointmentRepository;
            _bloodUnitRepository = bloodUnitRepository;
            _mapper = mapper;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync ()
        {
            var users = await _userManager.Users.ToListAsync();
            var donors = await _userManager.GetUsersInRoleAsync( Roles.Donor );
            var donations = await _donationRepository.GetAllAsync();
            var pendingAppointments = await _appointmentRepository.GetUpcomingAppointmentsAsync( DateTime.UtcNow );
            var bloodUnits = await _bloodUnitRepository.GetAllAsync();

            var stats = new DashboardStatsDto
            {
                TotalUsers = users.Count,
                TotalDonors = donors.Count,
                TotalDonations = donations.Count(),
                PendingAppointments = pendingAppointments.Count(),
                BloodTypeStats = await GetBloodTypeStatsAsync(),
                RecentActivities = await GetRecentActivitiesAsync()
            };

            return stats;
        }

        public async Task<List<BloodTypeStatDto>> GetBloodTypeStatsAsync ()
        {
            var bloodUnits = await _bloodUnitRepository.GetAllAsync();
            var stats = new List<BloodTypeStatDto>();

            foreach ( BloodType bloodType in Enum.GetValues( typeof( BloodType ) ) )
            {
                var count = bloodUnits.Count( u => u.BloodType == bloodType &&
                                                u.Status == BloodUnitStatus.Available );
                stats.Add( new BloodTypeStatDto
                {
                    BloodType = bloodType,
                    Count = count,
                    Percentage = bloodUnits.Any() ?
                        ( double ) count / bloodUnits.Count() * 100 : 0
                } );
            }

            return stats;
        }

        public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync ( int count = 10 )
        {
            var activities = new List<RecentActivityDto>();

            // Get recent donations
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

            // Get upcoming appointments
            var upcomingAppointments = await _appointmentRepository.GetUpcomingAppointmentsAsync( DateTime.UtcNow );
            foreach ( var appointment in upcomingAppointments.Take( count ) )
            {
                activities.Add( new RecentActivityDto
                {
                    ActivityType = "Appointment",
                    Description = $"Upcoming appointment for {appointment.Donor.FirstName} {appointment.Donor.LastName}",
                    Timestamp = appointment.AppointmentDate,
                    PerformedBy = $"{appointment.Donor.FirstName} {appointment.Donor.LastName}"
                } );
            }

            // Order by most recent and take specified count
            return activities
                .OrderByDescending( a => a.Timestamp )
                .Take( count )
                .ToList();
        }
    }
}
