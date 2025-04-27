using dVoucher.Biz;
using dVoucher.Model;
using FraTool.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Text;
using System.Text.Json;

namespace FraTool.Web.Controllers
{
    public class VoucherController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly  VoucherBiz voucherBiz;
        private readonly  SentVoucherBiz sentVoucherBiz;
        public VoucherController(IConfiguration _configuration)
        {
            configuration = _configuration;
            voucherBiz = new VoucherBiz(configuration);
            sentVoucherBiz = new SentVoucherBiz(configuration);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpData(string VoucherDate)
        {
            try
            {
                var param = new VoucherApiParams()
                {
                    date = VoucherDate,
                    companycode = HttpContext.Session.GetString("FraCompanyCode"),
                    estatecode = HttpContext.Session.GetString("FraDivisionCode")
                };
                int result = 0;
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://172.17.5.125:1234/api/accountinfo/");
                var json = JsonSerializer.Serialize(param);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync("accountjson", content).Result;
                if (response != null)
                {
                    var divisionCode = HttpContext.Session.GetString("FraDivisionCode");
                    var userName = HttpContext.Session.GetString("UserName");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var postResponse = JsonSerializer.Deserialize<VoucherMaster>(responseContent);
                    VoucherMaster vouchers = postResponse!;
                    result = voucherBiz.InsertVoucherToFRATool(vouchers, divisionCode!,userName!);
                }
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult GetVoucherMaster()
        {
            try
            {
                var divisionCode = HttpContext.Session.GetString("FraDivisionCode");
                var voucher = voucherBiz.GetVoucherMaster(divisionCode!);
                return Json(data: voucher);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult GetVoucher(long MasterId)
        {
            try
            {
                var voucher = voucherBiz.GetVoucher(MasterId);
                return Json(data: voucher);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult SearchVoucher(string FromDate, string ToDate)
        {
            try
            {
                var voucher = new List<VMasterViewModel>();
                if (FromDate != "" && ToDate != "")
                {
                    var divisionCode = HttpContext.Session.GetString("FraDivisionCode");
                    voucher = voucherBiz.GetVoucherMaster(divisionCode!).Where(x => Convert.ToDateTime(x.date) >= Convert.ToDateTime(FromDate)).ToList();
                }
                return Json(data: voucher);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult ConfirmVoucher(long MasterId)
        {
            int result = 0;
            if (MasterId > 0)
            {
                ConHelper helper = new ConHelper();
                var FraDivisionCode = HttpContext.Session.GetString("FraDivisionCode");
                var EstateCode = HttpContext.Session.GetString("EstateCode");
                string? conString = helper.ConStrings(EstateCode);
                result = sentVoucherBiz.VoucherSentToCharms(MasterId, FraDivisionCode!, conString.ToString()!);
            }
            return Json(data: result);
        }
        public IActionResult ErrorLog()
        {
            return View();
        }
    }
}
