using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class BloodUnitDto
    {
        public int Id { get; set; }
        public string UnitNumber { get; set; }
        public int DonationId { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public BloodUnitStatus Status { get; set; }
        public string StorageLocation { get; set; }
    }
}