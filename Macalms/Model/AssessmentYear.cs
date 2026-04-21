using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalms.Model
{
    public class AssessmentYear
    {
        public long RecordId { get; set; }
        public string? YearName { get; set; }
        public string? ShortCode { get; set; }
        public int IsActive { get; set; }
        public string? EntryBy { get; set; }
        public string? EntryDate { get; set; }
        public string? ModifyBy { get; set; }
        public string? ModifyDate { get; set; }
    }
}
