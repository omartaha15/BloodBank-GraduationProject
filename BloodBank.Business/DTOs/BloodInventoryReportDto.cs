namespace BloodBank.Business.DTOs
{
    public class BloodInventoryReportDto
    {
        public DateTime ReportDate { get; set; }
        public List<BloodTypeStatDto> BloodTypeInventory { get; set; }
        public int TotalDonationsToday { get; set; }
        public int TotalRequestsToday { get; set; }
        public List<BloodUnitDto> ExpiringUnits { get; set; }
    }
}
