using BloodBank.Business.DTOs;

namespace Blood_Bank.ViewModels
{
    public class HomeViewModel
    {
        public List<BloodTypeStatViewModel> BloodTypeStats { get; set; }
        public IEnumerable<BloodUnitDto> ExpiringUnits { get; set; }
    }
}
