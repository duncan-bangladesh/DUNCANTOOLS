using dSecurity.Biz;
using dSecurity.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraTool.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private IConfiguration _configuration;
        private readonly UsersBiz biz;
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
            biz = new UsersBiz(_configuration);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Users user)
        {
            try
            {
                int result = 0;
                if (ModelState.IsValid)
                {
                    if (user != null)
                    {
                        user.PasswordHash = await biz.ShaEncrypt(user.Password);
                        user.EntryBy = HttpContext.Session.GetString("UserName");
                        result = await biz.AddUser(user);
                    }
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(Users user)
        {
            try
            {
                int result = 0;
                if (user != null)
                {
                    user.ModifyBy = HttpContext.Session.GetString("UserName");
                    if (user.Password == null || user.Password == "")
                    {
                        result = await biz.UpdateUserInfo(user);
                    }
                    else
                    {
                        user.OldPassword = "";
                        user.PasswordHash = await biz.ShaEncrypt(user.Password);
                        user.EntryBy = HttpContext.Session.GetString("UserName");
                        result = await biz.ChangePassword(user);
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
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var data = await biz.GetUsers();
                return Json(data: (data.OrderBy(u => u.UserName).ToList()));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> ChangeStatus(long UserId)
        {
            try
            {
                var result = 0;
                if (UserId > 0)
                {
                    var data = await biz.GetUsers();
                    var user = data.Where(u => u.UserId == UserId).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.IsActive == true)
                        {
                            user.IsActive = false;
                        }
                        else
                        {
                            user.IsActive = true;
                        }
                        user.ModifyBy = HttpContext.Session.GetString("UserName");
                        result = await biz.ChangeUserStatus(user);
                    }
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(Users user)
        {
            try
            {
                int result = 0;
                if (user != null)
                {
                    user.PasswordHash = await biz.ShaEncrypt(user.Password);
                    user.EntryBy = HttpContext.Session.GetString("UserName");
                    result = await biz.ChangePassword(user);
                }
                if (result > 0)
                {
                    HttpContext.Session.Clear();
                    HttpContext.Session.Remove("UserName");
                    HttpContext.Session.Remove("FullName");
                    HttpContext.Session.Remove("CompanyId");
                    HttpContext.Session.Remove("RoleId");
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<JsonResult> ActiveUsers()
        {
            try
            {
                var data = await biz.GetUsers();
                var users = from u in data
                            .Where(x => x.IsActive == true)
                            .OrderBy(x => x.UserName)
                            select new
                            {
                                u.UserId,
                                u.UserName
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
                Users? user = new Users();
                if (id > 0)
                {
                    var data = await biz.GetUsers();
                    user = data.Where(u => u.UserId == id).SingleOrDefault() ?? new Users();
                    user.Password = "";
                    user.OldPassword = "";
                    user.PasswordHash = "";
                }
                return Json(data: user);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
