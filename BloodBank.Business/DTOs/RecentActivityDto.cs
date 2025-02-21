namespace BloodBank.Business.DTOs
{
    public class RecentActivityDto
    {
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string PerformedBy { get; set; }
    }
}
