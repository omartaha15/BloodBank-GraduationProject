using BloodBank.Core.Entities.BloodBank.Core.Entities;
using System;

namespace BloodBank.Business.DTOs
{
    public class BloodTestDto
    {
        public int Id { get; set; }
        public string DonorId { get; set; }
        // New: Hospital chosen for the test.
        public string HospitalId { get; set; }

        public bool HivTest { get; set; }
        public bool HepatitisB { get; set; }
        public bool HepatitisC { get; set; }
        public bool Syphilis { get; set; }
        public bool Malaria { get; set; }
        public string OtherTestNotes { get; set; }
        public bool IsTestPassed { get; set; }
        public DateTime TestDate { get; set; }
        // New: Approval status provided by the hospital.
        public HospitalApprovalStatus HospitalApprovalStatus { get; set; }
    }
}
