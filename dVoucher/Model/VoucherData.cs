using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVoucher.Model
{
    public class VoucherData
    {
        public string? description { get; set; }
        public string? division { get; set; }
        public string? voucher_type { get; set; }
        public List<VoucherDetail>? data { get; set; }
    }
    public class VoucherDetail
    {
        public string? account_code { get; set; }
        public string? head_name { get; set; }
        public string? description { get; set; }
        public string? amount { get; set; }
        public List<VoucherDetail>? data { get; set; }
    }
    public class VoucherApiParams
    {
        public string? date { get; set; }
        public string? companycode { get; set; }
        public string? estatecode { get; set; }
    }
    public class VoucherDetailModel
    {
        public int id { get; set; }
        public string? account_code { get; set; }
        public string? head_name { get; set; }
        public string? description { get; set; }
        public string? amount { get; set; }
        public int tranSide { get; set; }
    }
}
