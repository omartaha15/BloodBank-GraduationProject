namespace BloodBank.Business.DTOs
{
    public class InventoryStatsDto
    {
        public int TotalUnits { get; set; }
        public int AvailableUnits { get; set; }
        public int ExpiringUnits { get; set; }
        public List<BloodTypeStatDto> BloodTypeStats { get; set; }
    }
}
