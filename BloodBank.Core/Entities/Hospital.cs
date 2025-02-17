namespace BloodBank.Core.Entities
{
    public class Hospital : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public virtual ICollection<BloodRequest> BloodRequests { get; set; }
    }
}
