using dSecurity.Biz;
using dSecurity.Model;
using FraTool.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FraTool.Web.Controllers
{
    [Authentication]
    public class RolesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly RoleBiz biz;
        private readonly MenusInRoleBiz menusInRoleBiz;
        private readonly UsersInRoleBiz usersInRoleBiz;
        public RolesController(IConfiguration configuration)
        {
            _configuration = configuration;
            biz = new RoleBiz(_configuration);
            menusInRoleBiz = new MenusInRoleBiz(_configuration);
            usersInRoleBiz = new UsersInRoleBiz(_configuration);            
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                return Json(data: await biz.GetRole());
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(Role model)
        {
            try
            {
                string response = "";
                int result = 0;
                if (model != null)
                {
                    if (model.RoleId == 0)
                    {
                        model.EntryBy = HttpContext.Session.GetString("UserName");
                        result = await biz.AddRole(model);
                        if (result > 0)
                        {
                            response = "Role added successfully.";
                        }
                    }
                    else
                    {
                        model.ModifyBy = HttpContext.Session.GetString("UserName");
                        result = await biz.UpdateRoles(model);
                        if (result > 0)
                        {
                            response = "Role updated successfully.";
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
        public async Task<IActionResult> CheckRole(string name)
        {
            try
            {
                int result = 0;
                if (!string.IsNullOrEmpty(name))
                {
                    result = await biz.CheckRole(name);
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public  async Task<IActionResult> EditView(long id)
        {
            try
            {
                var role = new Role();
                if (id > 0)
                {
                    var data = await biz.GetRole();
                    role = data.Where(u => u.RoleId == id).SingleOrDefault();
                }
                return Json(data: role);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(long RoleId)
        {
            try
            {
                var result = 0;
                if (RoleId > 0)
                {
                    var data = await biz.GetRole();
                    var role = data.Where(u => u.RoleId == RoleId).FirstOrDefault();
                    if (role != null)
                    {
                        if (role.IsActive == true)
                        {
                            role.IsActive = false;
                        }
                        else
                        {
                            role.IsActive = true;
                        }
                        role.ModifyBy = HttpContext.Session.GetString("UserName");
                        result = await biz.ChangeRoleStatus(role);
                    }
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult RoleWiseMenu()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ActiveRoles()
        {
            try
            {
                var data = await biz.GetRole();
                var roles = from x in data
                           .Where(r => r.IsActive == true) 
                           select new 
                           { 
                               x.RoleId, 
                               x.RoleName 
                           };
                return Json(data: roles);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveMenusInRole(long RoleId, string[] MenuList)
        {
            try
            {
                int result = 0;
                MenusInRole menu = new MenusInRole();
                menu.RoleId = RoleId;
                menu.EntryBy = HttpContext.Session.GetString("UserName");
                int x = await menusInRoleBiz.CheckMenusInRole(menu);
                for (int i = 0; i < MenuList.Length; i++)
                {
                    if (MenuList[i] != null)
                    {
                        menu = new MenusInRole();
                        menu.RoleId = RoleId;
                        menu.MenuId = Convert.ToInt64(MenuList[i]);
                        menu.EntryBy = HttpContext.Session.GetString("UserName");
                        result += await menusInRoleBiz.AddMenusInRole(menu);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> MenusByRoleId(long RoleId)
        {
            try
            {
                var list = new List<MenusInRole>();
                if (RoleId > 0)
                {
                    list = await menusInRoleBiz.MenusByRoleId(RoleId);
                }
                return Json(data: list);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult RoleWiseUser()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UsersByRoleId(long RoleId)
        {
            try
            {
                var list = new List<UsersInRole>();
                if (RoleId > 0)
                {
                    list = await usersInRoleBiz.UsersByRoleId(RoleId);
                }
                return Json(data: list);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveUsersInRole(long RoleId, string[] UserList)
        {
            try
            {
                int result = 0;
                if (RoleId > 0)
                {
                    UsersInRole model = new UsersInRole();
                    model.RoleId = RoleId;
                    model.EntryBy = HttpContext.Session.GetString("UserName");
                    int x = await usersInRoleBiz.CheckUsersInRole(model);
                    for (int i = 0; i < UserList.Length; i++)
                    {
                        if (UserList[i] != null)
                        {
                            model = new UsersInRole();
                            model.RoleId = RoleId;
                            model.UserId = Convert.ToInt64(UserList[i]);
                            model.EntryBy = HttpContext.Session.GetString("UserName");
                            result += await usersInRoleBiz.AddUsersInRole(model);
                        }
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
