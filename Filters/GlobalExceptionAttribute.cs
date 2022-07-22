using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Web.Http.Tracing;
using ETradeAPI.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net;
using System.Web.Http.Controllers;
using System.Threading.Tasks;
using System.Threading;

namespace ETradeAPI.Filters
{
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Error(context.Request, "Controller : " + context.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + context.ActionContext.ActionDescriptor.ActionName, context.Exception);

            var exceptionType = context.Exception.GetType();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
            responseMessage.ReasonPhrase = "The requested resource is not found, had its name changed, or is temporarily unavailable.";
            throw new HttpResponseException(responseMessage);
            //if (exceptionType == typeof(ValidationException))
            //{
            //    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(context.Exception.Message), ReasonPhrase = "ValidationException", };
            //    throw new HttpResponseException(resp);

            //}
            //else if (exceptionType == typeof(UnauthorizedAccessException))
            //{
            //    throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.Unauthorized));
            //}
            //else
            //{
            //    throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.InternalServerError));
            //}
        }
        internal class ThrowModelStateErrorsActionInvoker : ApiControllerActionInvoker
        {
            public override async Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
            {
                foreach (var error in actionContext.ModelState.SelectMany(kvp => kvp.Value.Errors))
                {
                    var exception = error.Exception ?? new ArgumentException(error.ErrorMessage);
                    //invoke global exception handling
                }

                return await base.InvokeActionAsync(actionContext, cancellationToken);
            }
        }
    }
}