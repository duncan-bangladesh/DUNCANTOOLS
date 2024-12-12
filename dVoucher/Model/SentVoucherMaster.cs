using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVoucher.Model
{
    public class SentVoucherMaster
    {
        public string? TranDate { get; set; }
        public int TranType { get; set; }
        public string? Narration { get; set; }
        public string? CompanyCode { get; set; }
        public string? EstateCode { get; set; }
        public int SerialNo { get; set; }
        public string? BatchNo { get; set; }
        public int VoucherSerialNo { get; set; }
        public string? VoucherNo { get; set; }

    }
}
