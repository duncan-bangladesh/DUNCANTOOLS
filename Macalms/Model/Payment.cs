using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalms.Model
{
    public class Payment
    {
        public long RecordId { get; set; }
        public int SL { get; set; }
        public int AssessmentYear { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? StudyMedium { get; set; }
        public string? ParentName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Age { get; set; }
        public int ScholarshipDuration { get; set; }
        public int AllowedMonths { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }
        public string? BankAccountNo { get; set; }
        public string? BankRoutingNo { get; set; }
        public double Amount { get; set; }
        public string? EntryBy { get; set; }
        public string? EntryDate { get; set; }
    }
}
