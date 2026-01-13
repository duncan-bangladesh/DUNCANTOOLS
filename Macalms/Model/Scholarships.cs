using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalms.Model
{
    public class Scholarships
    {
        public string? StudentName { get; set; }
        public string? ParentName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? StudyMedium { get; set; }
        public int StAgeYears { get; set; }
        public int StAgeMonths { get; set; }
        public int StAgeDays { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }
        public string? BankAccountNo { get; set; }
        public string? BankRoutingNo { get; set; }
        public int EmpEligibleMonths { get; set; }
        public int EmpEligibleDays { get; set; }        
    }
    public class  ScholarshipData
    {
        public string? StudentName { get; set; }
        public string? ParentName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }
        public string? BankAccountNo { get; set; }
        public string? BankRoutingNo { get; set; }
        public double Amount { get; set; }
    }
}
