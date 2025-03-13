using System;
using System.Collections.Generic;
using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class BloodRequestDto
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public BloodType BloodType { get; set; }
        public double QuantityRequired { get; set; }
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string Notes { get; set; }
        public List<BloodUnitDto> AssignedUnits { get; set; }
        public bool IsUrgent => Priority == RequestPriority.Emergency || Priority == RequestPriority.Urgent;
    }
}
