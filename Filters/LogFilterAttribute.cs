using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EarthSkyTime.Controllers
{
    public class LogFilterAttribute : ActionFilterAttribute 
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["UserID"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Authenticate", action = "Index" }));
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
            }
        }
    }
}