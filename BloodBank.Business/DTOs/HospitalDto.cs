namespace BloodBank.Business.DTOs
{
    public class HospitalDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public List<BloodRequestDto> BloodRequests { get; set; }
        public bool IsActive { get; set; }
    }
}
