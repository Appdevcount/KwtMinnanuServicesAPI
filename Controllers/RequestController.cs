using ETradeAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/Request")]
    public class RequestController : ApiController
    {
        [Route("RequestList")]
        [HttpPost]
        public HttpResponseMessage GETRequestListfortheUser([FromBody] EserviceRequest data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GETRequestListfortheUser(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("Detail")]
        [HttpPost]
        public HttpResponseMessage GETRequestDetailsfortheRequest([FromBody] EserviceRequest data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GETRequestDetailsfortheRequest(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        #region  Update Request
        [Route("updateRequest")]
        [HttpPost]
        public HttpResponseMessage updateRequest([FromBody] Dictionary<String, String> requestToUpdate)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.updateRequest(requestToUpdate), System.Text.Encoding.UTF8, "application/json")
            };
        }
        #endregion  Update Request

    }
}
