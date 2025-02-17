using BloodBank.Core.Enums;

namespace BloodBank.Core.Entities
{
    public class BloodUnit : BaseEntity
    {
        public string UnitNumber { get; set; } // Barcode/ISBT 128
        public int DonationId { get; set; }
        public virtual Donation Donation { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public BloodUnitStatus Status { get; set; }
        public string StorageLocation { get; set; }
    }
}
