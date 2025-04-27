using dShared.Biz;
using dShared.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FraTool.Web.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly CompanyBiz biz;
        public CompanyController(IConfiguration configuration)
        {
            _configuration = configuration;
            biz = new CompanyBiz(_configuration);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AllCompanies()
        {
            try
            {
                var list = await biz.GetCompanies();
                return Json(data: list);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(Company model)
        {
            try
            {
                int result = 0;
                string response = "";
                if (model != null)
                {
                    if (model.CompanyId == 0)
                    {
                        model.EntryBy = HttpContext.Session.GetString("UserName");
                        result = await biz.AddCompany(model);
                        if (result > 0)
                        {
                            response = "Company added successfully.";
                        }
                    }
                    else
                    {
                        model.ModifyBy = HttpContext.Session.GetString("UserName");
                        result = await biz.UpdateCompany(model);
                        if (result > 0)
                        {
                            response = "Company updated successfully.";
                        }
                    }
                }
                return Json(data: response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckCompanyName(string name)
        {
            try
            {
                int result = 0;
                if (!string.IsNullOrEmpty(name))
                {
                    result = await biz.CheckCompanyName(name);
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(long CompanyId)
        {
            try
            {
                var result = 0;
                if (CompanyId > 0)
                {
                    var data = await biz.GetCompanies();
                    var model = data.Where(u => u.CompanyId == CompanyId).FirstOrDefault();
                    if (model != null)
                    {
                        if (model.IsActive == true)
                        {
                            model.IsActive = false;
                        }
                        else
                        {
                            model.IsActive = true;
                        }
                        model.ModifyBy = HttpContext.Session.GetString("UserName");
                        result = await biz.ChangeCompanyStatus(model);
                    }
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ActiveCompanies()
        {
            try
            {
                var companies = await biz.GetCompanies();
                var data = from c in companies
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.CompanyName)
                           select new
                           {
                               c.CompanyId,
                               c.CompanyName
                           };
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }            
        }
        [HttpGet]
        public async Task<IActionResult> EditView(long id)
        {
            try
            {
                var data = new Company();
                if (id > 0)
                {
                    var company = await biz.GetCompanies();
                    data = company.Where(c => c.CompanyId == id).SingleOrDefault();
                }
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetTranCompanies() 
        {
            try
            {
                var list = await biz.GetCompanies();
                var data = list.Where(x => x.IsTranCompany == 1).ToList(); ;
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
