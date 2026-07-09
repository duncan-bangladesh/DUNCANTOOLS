using dCommon;
using Microsoft.AspNetCore.Http;

namespace dVehicle.Model
{
    public class VehicleDocument : DbBase
    {
        public long RecordId { get; set; }
        public long DocumentTypeId { get; set; }
        public string? DocumentTypeName { get; set; }
        public long VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public long IssueLocationId { get; set; } = 0;
        public int CompanyId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string? FilePath { get; set; }
        public string? PhysicalPath { get; set; }
        public string? FileUrl { get; set; }
        public string? Remarks { get; set; }        
        public IFormFile? DocumentAttachment { get; set; }
    }
}
