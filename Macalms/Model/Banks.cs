using dCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalms.Model
{
    public class Banks : DbBase
    {
        public long RecordId { get; set; }
        public string? BankName { get; set; }
        public string? ShortCode { get; set; }
    }
}
