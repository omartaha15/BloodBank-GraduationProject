namespace BloodBank.Business.DTOs
{
    public class CreateBloodTestDto
    {
        public int DonationId { get; set; }
        public bool HivTest { get; set; }
        public bool HepatitisB { get; set; }
        public bool HepatitisC { get; set; }
        public bool Syphilis { get; set; }
        public bool Malaria { get; set; }
        public string OtherTestNotes { get; set; }
    }
}
