using System;
using BloodBank.Core.Entities;
using BloodBank.Core.Entities.BloodBank.Core.Entities;
using BloodBank.Core.Enums; // Ensure this is added to get HospitalApprovalStatus

namespace BloodBank.Business.DTOs
{
    public class BloodTestDto
    {
        public int Id { get; set; }
        public string DonorId { get; set; }
        public string DonorName { get; set; } // NEW property to show donor's full name
        public string HospitalId { get; set; }
        public bool HivTest { get; set; }
        public bool HepatitisB { get; set; }
        public bool HepatitisC { get; set; }
        public bool Syphilis { get; set; }
        public bool Malaria { get; set; }
        public string OtherTestNotes { get; set; }
        public bool IsTestPassed { get; set; }
        public DateTime TestDate { get; set; }
        public HospitalApprovalStatus HospitalApprovalStatus { get; set; }
    }
}
