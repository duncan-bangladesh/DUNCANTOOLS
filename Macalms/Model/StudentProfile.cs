using dCommon;
using System.ComponentModel.DataAnnotations;

namespace Macalms.Model
{
    public class StudentProfile : DbBase
    {
        public long RecordId { get; set; }
        public long ParentId { get; set; }
        public string? ParentName { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public long BankId { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNo { get; set; }
        public string? BankBranch { get; set; }
        public string? BankRoutingNo { get; set; }
        public string? EmployeeRefCode { get; set; }
        public string? ScholarshipStatus { get; set; }
    }    
}
