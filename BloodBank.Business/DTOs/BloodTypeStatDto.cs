using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class BloodTypeStatDto
    {
        public BloodType BloodType { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
