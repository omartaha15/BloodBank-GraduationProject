using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class CreateBloodRequestDto
    {
        public int HospitalId { get; set; }
        public BloodType BloodType { get; set; }
        public double QuantityRequired { get; set; }
        public RequestPriority Priority { get; set; }
        public DateTime RequiredDate { get; set; }
        public string Notes { get; set; }
    }
}
