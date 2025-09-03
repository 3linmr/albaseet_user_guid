//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.EntityFrameworkCore;
//using Shared.Repository;
//using Shared.Service.Helpers.Identity;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

//namespace Shared.API.Helpers
//{
//    public class AuthorizedAction : ActionFilterAttribute
//    {
//        public override void OnResultExecuting(ResultExecutingContext filterContext)
//        {

//        }

//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            //base.OnActionExecuting(filterContext);


//            //var connection = "server=localhost;user id=root;password=ayman;database=albaseet_copy;";
//            //var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//            //optionsBuilder.UseMySql(connection, ServerVersion.AutoDetect(connection), b => b.MigrationsAssembly("Shared.Repository"));
//            //using (var context = new ApplicationDbContext(optionsBuilder.Options))
//            //{
//            //    // do stuff
//            //}






//            //var httpContextAccessor = new HttpContextAccessor();
//            //httpContextAccessor.InitializeSessions();

//            //if (httpContextAccessor?.HttpContext?.Session.GetString("UserName") != null)
//            //{
//            //    //if ( Helpers.Helpers.PharmacyStopped())
//            //    //{
//            //    //    filterContext.Result = new RedirectToRouteResult(
//            //    //        new RouteValueDictionary { { "controller", "Home" }, { "action", "SomethingWentWrong" } });
//            //    //    return;
//            //    //}

//            //    if (!Helpers.UserValid(httpContextAccessor?.HttpContext?.Session?.GetString("UserName")))
//            //    {
//            //        filterContext.Result = new RedirectToRouteResult(
//            //            new RouteValueDictionary { { "controller", "Home" }, { "action", "UserLocked" } });
//            //        return;
//            //    }
//            //}

//            //if (filterContext.HttpContext.Session.GetString("UserId") == null)
//            //{
//            //    filterContext.Result = new RedirectToRouteResult(
//            //        new RouteValueDictionary { { "controller", "Home" }, { "action", "AccessDenied" } });
//            //    return;
//            //}

//            //if (httpContextAccessor.HttpContext.Session.GetString("UserName") == "LoggedOut")
//            //{
//            //    filterContext.Result = new RedirectToRouteResult(
//            //        new RouteValueDictionary { { "controller", "Home" }, { "action", "LoggedOut" } });
//            //    return;
//            //}

//            //var menus = httpContextAccessor.GetUserMenus();
//            //var controllerName = filterContext.RouteData.Values["controller"];
//            //var actionName = filterContext.RouteData.Values["action"];
//            //string url = "/" + controllerName + "/" + actionName;

//            //if (menus.All(s => s.MenuUrl != url) && url != "/Home/Index")
//            //{
//            //    filterContext.Result = new RedirectToRouteResult(
//            //        new RouteValueDictionary { { "controller", "Home" }, { "action", "AccessDenied" } });
//            //    return;
//            //}
//        }
//    }
//}