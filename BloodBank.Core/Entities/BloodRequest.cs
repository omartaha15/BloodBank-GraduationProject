using BloodBank.Core.Enums;

namespace BloodBank.Core.Entities
{
    public class BloodRequest : BaseEntity
    {
        public int HospitalId { get; set; }
        public virtual User Hospital { get; set; }
        public BloodType BloodType { get; set; }
        public double QuantityRequired { get; set; }
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequiredDate { get; set; }
        public string Notes { get; set; }
        public virtual ICollection<BloodUnit> AssignedUnits { get; set; }
    }
}
