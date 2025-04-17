using BloodBank.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BloodBank.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBloodInventoryService _inventoryService;

        public HomeController ( IBloodInventoryService inventoryService )
        {
            _inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index ()
        {
            var stats = await _inventoryService.GetBloodTypeStatsAsync();
            return View( stats );
        }

        public async Task<IActionResult> BloodPanel ()
        {
            return View();
        }

        public async Task<IActionResult> About ()
        {
            return View();
        }
    }
}