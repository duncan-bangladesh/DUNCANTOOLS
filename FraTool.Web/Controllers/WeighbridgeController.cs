using dWeighbridge.Biz;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FraTool.Web.Controllers
{
    public class WeighbridgeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ScaleDataBiz scaleDataBiz;
        public WeighbridgeController(IConfiguration configuration)
        {
            _configuration = configuration;
            scaleDataBiz = new ScaleDataBiz(_configuration);
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SearchScaleData(string? EstateCode, string? FromDate, string? ToDate)
        {
            try
            {
                if(FromDate != null)
                {
                    DateTime fdate = DateTime.ParseExact(FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    FromDate = fdate.ToString("yyyy-MM-dd");
                }
                else
                {
                    FromDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                if(ToDate != null)
                {
                    DateTime tdate = DateTime.ParseExact(ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    ToDate = tdate.ToString("yyyy-MM-dd");
                }
                else
                {
                    ToDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                if(EstateCode == "0")
                {
                    EstateCode = null;
                }

                var dataset = await scaleDataBiz.FilterScaleData(EstateCode, FromDate, ToDate);
                return Json(data: dataset);
            }
            catch
            {
                throw;
            }
        }
    }
}
