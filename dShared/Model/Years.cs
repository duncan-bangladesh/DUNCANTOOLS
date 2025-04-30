using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dShared.Model
{
    public class Years
    {
        public long RecordId { get; set; }
        public string? Year { get; set; }
        public int IsActive { get; set; }
    }
    public class Months
    {
        public long RecordId { get; set; }
        public string? MonthCode { get; set; }
        public string? MonthName { get; set; }
        public int IsActive { get; set; }
    }
}
