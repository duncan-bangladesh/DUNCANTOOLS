using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dCommon
{
    public class DbBase
    {
        public bool? IsActive { get; set; }
        public string? EntryBy { get; set; }
        public string? EntryDate { get; set; }
        public string? ModifyBy { get; set; }
        public string? ModifyDate { get; set; }
    }
}
