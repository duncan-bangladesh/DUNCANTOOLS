using dShared.Biz;
using dShared.Model;
using FraTool.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FraTool.Web.Controllers
{
    public class TransferController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly TransferBiz transferBiz;
        public TransferController(IConfiguration _configuration)
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
                string? conString = helper.ConStrings(estateCode);
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
    }
}
