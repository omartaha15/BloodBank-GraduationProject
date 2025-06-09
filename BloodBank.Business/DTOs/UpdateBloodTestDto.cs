using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class UpdateBloodTestDto
    {
        // Allow updating the test result and notes.
        public bool IsTestPassed { get; set; }
        public string OtherTestNotes { get; set; }
        // New: The hospital can update the approval status.
        public HospitalApprovalStatus HospitalApprovalStatus { get; set; }
    }
}
