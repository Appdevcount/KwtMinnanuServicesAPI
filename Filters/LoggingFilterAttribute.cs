using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Web.Http.Tracing;
using System.Web.Http;
using ETradeAPI.Helpers;

namespace ETradeAPI.Filters
{
    public class LoggingFilterAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Info(filterContext.Request, "Controller : " + controllerName + Environment.NewLine + "Action : " + actionName + Environment.NewLine, "JSON", filterContext.ActionArguments, actionName,controllerName);
        }
    }
}