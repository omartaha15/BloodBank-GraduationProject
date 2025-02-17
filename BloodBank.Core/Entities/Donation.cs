using BloodBank.Core.Enums;

namespace BloodBank.Core.Entities
{
    public class Donation : BaseEntity
    {
        public string DonorId { get; set; }
        public virtual User Donor { get; set; }
        public DateTime DonationDate { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; } // in milliliters
        public DonationStatus Status { get; set; }
        public virtual BloodTest BloodTest { get; set; }
        public virtual BloodUnit BloodUnit { get; set; }
    }
}
