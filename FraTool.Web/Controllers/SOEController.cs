using dFinance.Biz;
using dShared.Biz;
using dShared.Model;
using FraTool.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FraTool.Web.Controllers
{
    public class SOEController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly WagesBiz wagesBiz;
        public SOEController(IConfiguration _configuration)
        {
            configuration = _configuration;
            wagesBiz = new WagesBiz(configuration);
        }
        public IActionResult WagesReport()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetWagesReportData(string CompanyCode, string? FromDate, string? ToDate)
        {
            try
            {
                DateTime fromDate = DateTime.ParseExact(FromDate!, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(ToDate!, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                ConHelper helper = new ConHelper();
                var estateCode = CompanyCode;
                string? conString = helper.DlrConStrings(estateCode);
                //var result = await wagesBiz.FilterScaleData(fromDate, toDate, conString);
                var result = (await wagesBiz.WagesReport(fromDate, toDate, conString, estateCode)).OrderBy(x => x.AccountsOrder);
                return Json(new { success = true, data = result });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
