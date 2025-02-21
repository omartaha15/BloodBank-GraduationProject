namespace BloodBank.Business.DTOs
{
    public class BloodTestDto
    {
        public int Id { get; set; }
        public int DonationId { get; set; }
        public bool HivTest { get; set; }
        public bool HepatitisB { get; set; }
        public bool HepatitisC { get; set; }
        public bool Syphilis { get; set; }
        public bool Malaria { get; set; }
        public string OtherTestNotes { get; set; }
        public bool IsTestPassed { get; set; }
        public DateTime TestDate { get; set; }
    }
}
