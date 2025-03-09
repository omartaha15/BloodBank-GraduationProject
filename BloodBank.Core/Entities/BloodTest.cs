namespace BloodBank.Core.Entities
{
    namespace BloodBank.Core.Entities
    {
        public class BloodTest : BaseEntity
        {
            public string DonorId { get; set; }
            public virtual User Donor { get; set; }

            public bool HivTest { get; set; }
            public bool HepatitisB { get; set; }
            public bool HepatitisC { get; set; }
            public bool Syphilis { get; set; }
            public bool Malaria { get; set; }
            public string OtherTestNotes { get; set; }
            public bool IsTestPassed { get; set; }
            public DateTime TestDate { get; set; }

            // New: Hospital information
            public string HospitalId { get; set; }
            public virtual User Hospital { get; set; }

            // New: Hospital approval status (set by a hospital user)
            public HospitalApprovalStatus HospitalApprovalStatus { get; set; }
        }

        public enum HospitalApprovalStatus
        {
            Pending,
            Approved,
            Rejected
        }
    }


}
