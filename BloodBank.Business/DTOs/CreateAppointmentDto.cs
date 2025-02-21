namespace BloodBank.Business.DTOs
{
    public class CreateAppointmentDto
    {
        public string DonorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Notes { get; set; }
    }
}
