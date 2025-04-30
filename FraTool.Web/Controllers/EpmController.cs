using dShared.Biz;
using dShared.Model;
using FraTool.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FraTool.Web.Controllers
{
    public class EpmController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly TransferBiz transferBiz;
        public EpmController(IConfiguration _configuration)
        {
            configuration = _configuration;
            transferBiz = new TransferBiz(configuration);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveTransferData(long companyId, List<TransferData> transferData)
        {
            try
            {
                ConHelper helper = new ConHelper();
                var estateCode = await transferBiz.CompanyCodeForTransection(companyId);
                string? conString = helper.EpmConStrings(estateCode);
                var result = await transferBiz.SaveTransectionData(transferData, conString);
                if (result > 0)
                {
                    return Json(new { success = true, message = "Data saved successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save data." });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetYears()
        {
            try
            {
                var years = await transferBiz.GetYears();
                var data = years.OrderByDescending(x => x.Year).ToList();
                return Json(data : data );
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetMonths()
        {
            try
            {
                var months = await transferBiz.GetMonths();
                return Json( data : months );
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> Search(long companyId, string year, string month)
        {
            try
            {
                ConHelper helper = new ConHelper();
                var estateCode = await transferBiz.CompanyCodeForTransection(companyId);
                string? conString = helper.EpmConStrings(estateCode);
                var result = await transferBiz.GetTransferData(year, month, conString);
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
