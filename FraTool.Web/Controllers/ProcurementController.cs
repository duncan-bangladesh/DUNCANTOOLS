using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Procurement.Biz;
using Procurement.Model;

namespace FraTool.Web.Controllers
{
    public class ProcurementController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SupplierProfileBiz supplierProfileBiz;
        public ProcurementController(IConfiguration configuration)
        {
            _configuration = configuration;
            supplierProfileBiz = new SupplierProfileBiz(_configuration);
        }
        [Authorize]
        public IActionResult Supplier()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetNewSupplierCode(string SupplierName)
        {
            try
            {
                var result = await supplierProfileBiz.NewSupplierCode(SupplierName);
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckSupplierName(string SupplierName)
        {
            try
            {
                var result = await supplierProfileBiz.CheckDuplicateSupplierName(SupplierName);
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> IsBillExistForThisFinancialYear(string SupplierName)
        {
            try
            {
                var result = await supplierProfileBiz.IsBillExistForThisFinancialYear(SupplierName);
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSupplierProfileList()
        {
            try
            {
                var result = (await supplierProfileBiz.GetSupplierProfiles()).OrderBy(x=> x.Description).ToList();
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSupplierProfileBySlNo(int SlNo)
        {
            try
            {
                var result = (await supplierProfileBiz.GetSupplierProfiles()).Where(x => x.SLNo == SlNo).ToList();
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveSupplierProfile(SupplierProfile model)
        {
            try
            {
                int result = 0;
                if (model != null)
                {
                    model.CreateUser = HttpContext.Session.GetString("UserName");
                    result = await supplierProfileBiz.SaveSupplierProfile(model);
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSupplierProfile(SupplierProfile model)
        {
            try
            {
                int result = 0;
                if (model != null)
                {
                    model.UpdateUser = HttpContext.Session.GetString("UserName");
                    result = await supplierProfileBiz.UpdateSupplierProfile(model);
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
