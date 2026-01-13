using dCommon;

namespace Macalms.Model
{
    public class EmployeeProfile : DbBase 
    {
        public long RecordId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public long DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long WorkLocationId { get; set; }
        public string? WorkingIn { get; set; }
        public string? ContactNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? JobStatus { get; set; }
        public string? ApplicableFrom { get; set; }
        public string? ApplicableUpto { get; set; }
    }
}
