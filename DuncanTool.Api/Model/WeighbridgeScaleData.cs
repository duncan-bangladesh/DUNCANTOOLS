namespace DuncanTool.Api.Model
{
    public class WeighbridgeScaleData
    {
        public long RecordId { get; set; }
        public string? SerialNo { get; set; }
        public string? VehicleId { get; set; }
        public string? VehicleNumber { get; set; }
        public string? MaterialId { get; set; }
        public string? Material { get; set; }
        public string? CustomerId { get; set; }
        public string? Customer { get; set; }
        public string? Gross { get; set; }
        public string? Tare { get; set; }
        public string? Net { get; set; }
        public string? RealNet { get; set; }
        public string? RecordDateTime { get; set; }
        public string? SourceName { get; set; } //Tea Estate Name

        public string? EntryDate { get; set; }
    }
}
