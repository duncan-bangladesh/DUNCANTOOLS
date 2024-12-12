using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVoucher.Model
{
    public class VoucherMaster
    {
        public string? date { get; set; }
        public string? company { get; set; }
        public string? estate { get; set; }
        public List<VoucherData>? data { get; set; }
    }
    public class VMasterViewModel
    {
        public long RecordId { get; set; }
        public string? date { get; set; }
        public string? company { get; set; }
        public string? estate { get; set; }
        public string? division { get; set; }
        public string? description { get; set; }
        public string? voucher_type { get; set; }
        public int IsSent { get; set; }
    }
}
