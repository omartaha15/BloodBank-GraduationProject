using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class UpdateDonationDto
    {
        public DonationStatus Status { get; set; }
        public string Notes { get; set; }
    }
}
