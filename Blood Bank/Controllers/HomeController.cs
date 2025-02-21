using System.Diagnostics;
using Blood_Bank.ViewModels;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Blood_Bank.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBloodUnitService _bloodUnitService;

        public HomeController ( IBloodUnitService bloodUnitService )
        {
            _bloodUnitService = bloodUnitService;
        }

        public async Task<IActionResult> Index ()
        {
            var bloodTypes = Enum.GetValues( typeof( BloodType ) )
                                .Cast<BloodType>()
                                .ToList();

            var bloodTypeStats = new List<BloodTypeStatViewModel>();

            foreach ( var bloodType in bloodTypes )
            {
                var count = await _bloodUnitService.GetAvailableUnitsCountByBloodTypeAsync( bloodType );
                bloodTypeStats.Add( new BloodTypeStatViewModel
                {
                    BloodType = bloodType,
                    AvailableUnits = count
                } );
            }

            var viewModel = new HomeViewModel
            {
                BloodTypeStats = bloodTypeStats,
                ExpiringUnits = await _bloodUnitService.GetExpiringUnitsAsync( 7 )
            };

            return View( viewModel );
        }
    }
}
