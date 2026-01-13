namespace dShared.Model
{
    public class TransferData
    {   
        public long Id { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }
        public string? AccountNo { get; set; }
        public string? Description { get; set; }
        public string? Crop { get; set; }
        public double? Amount { get; set; }
    }
    public class ExcelUploadRequest
    {
        public int CompanyId { get; set; }
        public List<Dictionary<string, object>>? ExcelRows { get; set; }
    }
}
