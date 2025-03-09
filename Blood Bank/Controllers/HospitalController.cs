using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Blood_Bank.Controllers
{
    [Authorize( Roles = "Hospital" )]
    public class HospitalController : Controller
    {
        private readonly IBloodTestService _bloodTestService;
        private readonly UserManager<User> _userManager;

        public HospitalController ( IBloodTestService bloodTestService, UserManager<User> userManager )
        {
            _bloodTestService = bloodTestService;
            _userManager = userManager;
        }

        // GET: Hospital/BloodTestsPending
        public async Task<IActionResult> BloodTestsPending ()
        {
            var pendingTests = await _bloodTestService.GetPendingTestsAsync();
            return View( pendingTests );
        }

        // POST: Hospital/ApproveBloodTest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBloodTest ( int id )
        {
            await _bloodTestService.ApproveBloodTestAsync( id );
            TempData [ "Success" ] = "Blood test approved successfully.";
            return RedirectToAction( nameof( BloodTestsPending ) );
        }

        // POST: Hospital/RejectBloodTest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBloodTest ( int id )
        {
            await _bloodTestService.RejectBloodTestAsync( id );
            TempData [ "Success" ] = "Blood test rejected.";
            return RedirectToAction( nameof( BloodTestsPending ) );
        }
    }
}
