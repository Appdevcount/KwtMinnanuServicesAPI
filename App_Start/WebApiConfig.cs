using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Web.Http.Routing;

using ETradeAPI.Filters;
using ETradeAPI.Selectors;
using System.Web.Http.Dispatcher;
using System.Web.Http.Controllers;

namespace ETradeAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();

            // Customised Selector To Handle UnWanted Controller Access
            config.Services.Replace(typeof(IHttpControllerSelector), new HttpNotFoundAwareDefaultHttpControllerSelector(config));
            config.Services.Replace(typeof(IHttpActionSelector), new HttpNotFoundAwareControllerActionSelector());
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            
            //To Handle Db Logging
            config.Filters.Add(new LoggingFilterAttribute());
            //To Handle Exception Attribute
            config.Filters.Add(new GlobalExceptionAttribute());

            config.Formatters.XmlFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));
            // config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

            // Tempory Allow Cross For Developent
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            

            // Web API routes Handling
            config.MapHttpAttributeRoutes();

            //
            var constraints = new { httpMethod = new HttpMethodConstraint(HttpMethod.Options) };
           // config.Routes.IgnoreRoute("OPTIONS", "*pathInfo", constraints);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{actionid}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute( name: "Error404",
            routeTemplate: "{*url}",    
            defaults: new { controller = "Error", action = "Handle404" }
            );
        }
    }
}
