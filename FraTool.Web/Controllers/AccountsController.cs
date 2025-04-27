using dSecurity.Biz;
using dSecurity.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FraTool.Web.Controllers
{
    public class AccountsController : Controller
    {
        private IConfiguration _configuration;
        private readonly UsersBiz biz;
        public AccountsController(IConfiguration configuration)
        {
            _configuration = configuration;
            biz = new UsersBiz(_configuration);
        }
        //[AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserLogin login, string ReturnUrl = "")
        {
            try
            {                
                if (login != null)
                {
                    UserLoginBiz loginBiz = new UserLoginBiz(_configuration);
                    UsersInRoleBiz usersInRole = new UsersInRoleBiz(_configuration);
                    Users user = new Users();
                    user.UserName = login.UserName;
                    user.Password = login.Password;
                    user.PasswordHash = await biz.ShaEncrypt(login.Password);
                    int result = await loginBiz.IsFound(user);
                    if (result > 0)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, login.UserName),
                            new Claim(ClaimTypes.Name, login.UserName)
                        };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principle = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle, new AuthenticationProperties() { AllowRefresh = true });
                        HttpContext.Session.SetString("UserName", user.UserName.ToString());
                        var currentUser = await biz.FraInfoForLoginByUserName(user.UserName);
                        HttpContext.Session.SetString("FullName", currentUser.UserFullName ?? "");
                        HttpContext.Session.SetString("FraCompanyCode", currentUser.FraCompanyCode ?? "");
                        HttpContext.Session.SetString("EstateCode", currentUser.EstateCode ?? "");
                        HttpContext.Session.SetString("FraDivisionCode", currentUser.FraDivisionCode ?? "");
                        HttpContext.Session.SetString("OnLocationId", currentUser.OnLocationId.ToString() ?? "0");
                        HttpContext.Session.SetString("CompanyId", currentUser.LoginCompanyId.ToString() ?? "0");
                        var roleId = await usersInRole.GetRoleByUser(login.UserName);
                        HttpContext.Session.SetString("RoleId", roleId.ToString());
                        if (ReturnUrl != "")
                        {
                            return RedirectToAction(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Failed";
                        return View(login);
                    }
                }
            }
            catch
            {
                ViewBag.Message = "Server Error.";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("FullName");
            HttpContext.Session.Remove("CompanyId");
            HttpContext.Session.Remove("RoleId");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Accounts");
        }
        [HttpGet]
        public async Task<IActionResult> CheckUser(string name)
        {
            int result = 0;
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    result = await biz.CheckUser(name);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(data: result);
        }
        [HttpGet]
        public async Task<IActionResult> CheckPassword(string password)
        {
            int result = 0;
            try
            {
                var username = HttpContext.Session.GetString("UserName");
                if (!string.IsNullOrEmpty(username))
                {
                    result = await biz.CheckPassword(username, password);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(data: result);
        }
        [HttpGet]
        public IActionResult IsAuthUrls()
        {
            try
            {
                string? isAuthUrl = HttpContext.Session.GetString("authorizedUrl");
                HttpContext.Session.Remove("authorizedUrl");
                return Json(isAuthUrl);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult AppName()
        {
            try
            {
                string? message = _configuration.GetSection("ApplicationConfig").GetSection("AppName").Value;
                return Json(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
