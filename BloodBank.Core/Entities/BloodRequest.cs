using BloodBank.Core.Enums;
using System;
using System.Collections.Generic;

namespace BloodBank.Core.Entities
{
    public class BloodRequest : BaseEntity
    {
        public BloodRequest ()
        {
            AssignedUnits = new List<BloodUnit>();
        }
        public string HospitalId { get; set; }
        public virtual User Hospital { get; set; }
        public BloodType BloodType { get; set; }
        public double QuantityRequired { get; set; }
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime RequestDate { get; set; } // NEW property to track when the request was made
        public string Notes { get; set; }
        public virtual ICollection<BloodUnit> AssignedUnits { get; set; }
    }
}
