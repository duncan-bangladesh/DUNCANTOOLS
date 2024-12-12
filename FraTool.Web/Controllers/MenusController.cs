using dSecurity.Biz;
using dSecurity.Model;
using FraTool.Web.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FraTool.Web.Controllers
{
    [Authentication]
    public class MenusController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly MenuBiz biz;
        public MenusController(IConfiguration configuration)
        {
            _configuration = configuration;
            biz = new MenuBiz(_configuration);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMenu()
        {
            try
            {
                var list = new List<MenuViewModel>();
                var data = await biz.GetMenu();
                foreach (var m in data)
                {
                    MenuViewModel model = new MenuViewModel();
                    model.MenuId = m.MenuId;
                    model.DisplayName = m.DisplayName;
                    model.ControllerName = m.ControllerName;
                    model.ActionName = m.ActionName;
                    model.MenuUrl = m.MenuUrl;
                    if (m.IsParentMenu == 0)
                    {
                        model.IsParentMenu = "Child";
                    }
                    else
                    {
                        model.IsParentMenu = "Parent";
                    }
                    model.ParentMenuId = await MenuName(m.ParentMenuId);
                    model.IconTag = m.IconTag;
                    model.IsActive = m.IsActive;
                    model.EntryBy = m.EntryBy;
                    model.EntryDate = m.EntryDate;
                    model.ModifyBy = m.ModifyBy;
                    model.ModifyDate = m.ModifyDate;
                    list.Add(model);
                }
                return Json(data: list);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(Menu model)
        {
            try
            {
                string response = "";
                if (model != null)
                {
                    int result = 0;
                    if (model.ControllerName != null && model.ActionName != null)
                    {
                        model.MenuUrl = "/" + model.ControllerName + "/" + model.ActionName;
                    }
                    else
                    {
                        model.ControllerName = "";
                        model.ActionName = "";
                    }
                    if (model.IsParentMenu == 1)
                    {
                        model.ParentMenuId = 0;
                    }
                    //Add Menu
                    if (model.MenuId == 0)
                    {
                        model.EntryBy = HttpContext.Session.GetString("UserName");
                        result = await biz.AddMenu(model);
                        if (result > 0)
                        {
                            response = "Menu added successfully.";
                        }
                    }
                    //Update Menu
                    else
                    {
                        model.ModifyBy = HttpContext.Session.GetString("UserName");
                        result = await biz.UpdateMenus(model);
                        if (result > 0)
                        {
                            response = "Menu updated successfully.";
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
        public async Task<IActionResult> EditView(long id)
        {
            try
            {
                var menu = new Menu();
                if (id > 0)
                {
                    var data = await biz.GetMenu();
                    menu = data.Where(x => x.MenuId == id).FirstOrDefault();
                }
                return Json(data: menu);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckMenu(string name)
        {
            try
            {
                int result = 0;
                if (!string.IsNullOrEmpty(name))
                {
                    result = await biz.CheckMenuDisplayName(name);
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetParentMenu()
        {
            try
            {
                var parent = await biz.GetParentMenu();
                var data = (from m in parent select new { m.MenuId, m.DisplayName });
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(long MenuId)
        {
            try
            {
                var result = 0;
                if (MenuId > 0)
                {
                    var data = await biz.GetMenu();
                    var menu = data.Where(u => u.MenuId == MenuId).FirstOrDefault();
                    if (menu != null)
                    {
                        if (menu.IsActive == true)
                        {
                            menu.IsActive = false;
                        }
                        else
                        {
                            menu.IsActive = true;
                        }
                        menu.ModifyBy = HttpContext.Session.GetString("UserName");
                        result = await biz.ChangeMenuStatus(menu);
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
        public async Task<IActionResult> ActiveMenus()
        {
            try
            {
                var data = await biz.GetMenu();
                var menus = from x in data
                           .Where(r => r.IsActive == true) 
                           select new 
                           { 
                               x.MenuId, 
                               x.DisplayName 
                           };
                return Json(data: menus);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> MenusByUser()
        {
            try
            {
                string? userName = HttpContext.Session.GetString("UserName");
                var menus = MenusList(userName);
                return Json(data: await Task.Run(() => menus));
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<List<PMenus>> MenusList(string? userName)
        {
            try
            {
                var menus = await biz.GetMenusByUser(userName);
                var list = new List<PMenus>();
                List<Menu> parent = menus.Where(x => x.IsParentMenu == 1).ToList();
                List<Menu> child = menus.Where(x => x.IsParentMenu == 0).ToList();
                var pCount = parent.Count();
                while (pCount > 0)
                {
                    PMenus menu = new PMenus();
                    var pMenuId = parent.First().MenuId;
                    menu.MenuId = pMenuId;
                    menu.DisplayName = parent.First().DisplayName;
                    menu.ControllerName = parent.First().ControllerName;
                    menu.ActionName = parent.First().ActionName;
                    menu.MenuUrl = parent.First().MenuUrl;
                    menu.IsParentMenu = parent.First().IsParentMenu;
                    menu.ParentMenuId = parent.First().ParentMenuId;
                    menu.IconTag = parent.First().IconTag;
                    var pChild = child.Where(x => x.ParentMenuId == pMenuId).ToList();
                    if (pChild.Count() > 0)
                    {
                        List<CMenus> cData = new List<CMenus>();
                        foreach (var c in pChild)
                        {
                            CMenus m = new CMenus();
                            m.MenuId = c.MenuId;
                            m.DisplayName = c.DisplayName;
                            m.ControllerName = c.ControllerName;
                            m.ActionName = c.ActionName;
                            m.MenuUrl = c.MenuUrl;
                            m.IsParentMenu = c.IsParentMenu;
                            m.ParentMenuId = c.ParentMenuId;
                            m.IconTag = c.IconTag;
                            //menu.CMenus.Add(m);
                            cData.Add(m);
                        }
                        menu.CMenus = cData;
                        child.RemoveAll(x => x.ParentMenuId == pMenuId);
                    }
                    else
                    {
                        menu.CMenus = new List<CMenus>();
                    }
                    list.Add(menu);

                    parent.RemoveAll(x => x.MenuId == pMenuId);
                    pCount = pCount - 1;
                }
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<string> MenuName(long MenuId)
        {
            try
            {
                string name = "";
                if (MenuId > 0)
                {
                    var data = await biz.GetMenu();
                    name = data.Where(m => m.MenuId == MenuId).Single().DisplayName ?? "";
                }
                return await Task.Run(() => name);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
