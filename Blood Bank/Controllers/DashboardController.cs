using BloodBank.Business.Interfaces;
using BloodBank.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blood_Bank.Controllers
{
    //[Area( "Admin" )]
    [Authorize( Roles = Roles.Admin )]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController ( IDashboardService dashboardService )
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index ()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return View( stats );
        }
    }
}
