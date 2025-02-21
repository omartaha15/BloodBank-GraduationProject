using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class CreateDonationDto
    {
        public string DonorId { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; }
        public DateTime DonationDate { get; set; }
    }
}
