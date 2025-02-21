namespace BloodBank.Business.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalDonors { get; set; }
        public int TotalDonations { get; set; }
        public int PendingAppointments { get; set; }
        public List<BloodTypeStatDto> BloodTypeStats { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; }
    }
}
