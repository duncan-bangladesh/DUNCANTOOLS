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
        public int is_valid { get; set; } = 0;
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



    /////////////////////////////////
    /// Temporary Data Model ///////
    public class TempErrorLog
    {
        public long LogId { get; set; }
        public string? GlCode { get; set; }
        public string? GlDescription { get; set; }
        public string? AccCode { get; set; }
        public string? AccDescription { get; set; }
        public double Amount { get; set; }
        public string? VoucherDate { get; set; }
        public string? Estate { get; set; }
        public string? DivisionCode { get; set; }
        public string? VoucherType { get; set; }
        public string? EntryBy { get; set; }
        public string? EntryDate { get; set; }
    }
    public class TempVoucherData
    {
        public string? AccountCode { get; set; }
        public string? HeadName { get; set; }
        public string? Description { get; set; }
        public string? Amount { get; set; }
        //public string? TransectionType { get; set; }
    }
    public class TempVoucherDetail
    {
        public string? GlCode { get; set; }
        public string? AccountCode { get; set; }
        public string? HeadName { get; set; }
        public string? Description { get; set; }
        public string? Amount { get; set; }
    }

    ///////////////////////////////////////////////////////
    ////////////////Error Log Model////////////////////////
    public class FraGetVoucherErrorLog
    {
        public long RecordId { get; set; }
        public long LogId { get; set; }
        public string? GlCode { get; set; }
        public string? GlDescription { get; set; }
        public string? AccCode { get; set; }
        public string? AccDescription { get; set; }
        public double Amount { get; set; }
        public string? VoucherDate { get; set; }
        public string? Estate { get; set; }
        public string? DivisionCode { get; set; }
        public string? VoucherType { get; set; }
        public string? EntryBy { get; set; }
        public string? EntryDate { get; set; }
    }
}
