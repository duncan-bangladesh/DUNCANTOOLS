using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using dSecurity.Biz;

namespace FraTool.Web.Utilities
{
    public class Authentication : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            UserLoginBiz userLoginBiz = new UserLoginBiz(configuration);
            var userName = context.HttpContext.Session.GetString("UserName");
            string controller = context.RouteData.Values["controller"]!.ToString()!;
            string action = context.RouteData.Values["action"]!.ToString()!;
            string url = "/" + controller + "/" + action;
            
            if (userName == null)
            {
                context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "Controller", "Accounts" },
                    { "Action", "Login" }
                });
            }
            else
            {
                int isFound = await userLoginBiz.IsAuthUrl(userName.ToString(), url);
                if (isFound == 0)
                {
                    context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Home" },
                        { "Action", "UnAuthorized" }
                    });
                }
            }
            return;
        }        
    }
}
