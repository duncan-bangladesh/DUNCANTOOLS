using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVoucher.Model
{
    public class GL
    {
        public long GlId { get; set; }
        public string? GlCode { get; set; }
    }
    public class TaskCodeRef
    {
        public int RecordID { get; set; }
        public int ObjectID { get; set; }
    }
}
