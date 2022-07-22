using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ETradeAPI.Controllers
{
    
    public class ErrorController : ApiController
    {
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions, AcceptVerbs("PATCH")]
        public HttpResponseMessage Handle404()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
            responseMessage.ReasonPhrase = "The requested resource is not found, had its name changed, or is temporarily unavailable.";
            return responseMessage;
        }
        [Route("test")]
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions, AcceptVerbs("PATCH")]
        public HttpResponseMessage test()
        {
            throw (new UnauthorizedAccessException());
        }

    }
}
