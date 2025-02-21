using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class CreateBloodUnitDto
    {
        public int DonationId { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; }
        public string StorageLocation { get; set; }
    }
}
