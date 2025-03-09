namespace BloodBank.Business.DTOs
{
    public class CreateBloodTestDto
    {
        public string DonorId { get; set; }
        // New: The hospital the donor chooses for the blood test.
        public string HospitalId { get; set; }

        public bool HivTest { get; set; }
        public bool HepatitisB { get; set; }
        public bool HepatitisC { get; set; }
        public bool Syphilis { get; set; }
        public bool Malaria { get; set; }
        public string OtherTestNotes { get; set; }
    }
}
