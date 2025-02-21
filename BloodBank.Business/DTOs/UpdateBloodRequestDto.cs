using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class UpdateBloodRequestDto
    {
        public RequestStatus Status { get; set; }
        public string Notes { get; set; }
    }
}
