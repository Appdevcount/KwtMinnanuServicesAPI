using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;

namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/Notify")]
    public class NotifyController : ApiController
    {

        [Route("List")]
        [HttpPost]
        public HttpResponseMessage GetNotifications([FromBody] SecurityParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetNotifications(data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("Count")]
        [HttpPost]
        public HttpResponseMessage GetNotificationsCount([FromBody] SecurityParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetNotificationsCount(data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
