using dShared.Biz;
using dShared.Model;
using FraTool.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
        //This Save TransferData method is not using now, we are using UploadExcelData method for saving data from excel file.
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
        [HttpPost]
        public async Task<IActionResult> UploadExcelData([FromBody] ExcelUploadRequest request)
        {
            List<TransferData> transferData = new List<TransferData>();
            int companyId = request.CompanyId;
            var excelRows = request.ExcelRows;
            if (excelRows == null || excelRows.Count == 0)
                return BadRequest("No data received");
            else
            {
                for(int i = 0;  i < excelRows.Count; i++)
                {
                    var rowValues = excelRows[i].Values.ToList();
                    
                    TransferData data = new TransferData();
                    data.Year = rowValues[0]?.ToString();
                    data.Month = rowValues[1]?.ToString();
                    data.AccountNo = rowValues[2]?.ToString();
                    data.Description = rowValues[3]?.ToString();
                    data.Crop = rowValues[4]?.ToString();
                    if (double.TryParse(rowValues[5]?.ToString(), out double amount))
                    {
                        data.Amount = amount;
                    }
                    else
                    {
                        data.Amount = 0; // or handle the error as needed
                    }                  
                    if (!string.IsNullOrEmpty(data.Year) && !string.IsNullOrEmpty(data.Month) && !string.IsNullOrEmpty(data.AccountNo) && !string.IsNullOrEmpty(data.Description))
                    {
                        transferData.Add(data);
                    }
                }
            }
            if(transferData.Count > 0)
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
            else {
                return Json(new { success = false, message = "No valid data to save." });
            }
        }
    }
}
