using BloodBank.Business.DTOs;

namespace BloodBank.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync ();
        Task<List<BloodTypeStatDto>> GetBloodTypeStatsAsync ();
        Task<List<RecentActivityDto>> GetRecentActivitiesAsync ( int count = 10 );
    }
}
