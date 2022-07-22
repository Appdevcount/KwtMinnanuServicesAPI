using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;


namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/Search")]
    public class SearchController : ApiController
    {

        [Route("HouseBill")]
        // POST: api/User
        [HttpPost]
        //public HttpResponseMessage HouseBill(string hbNumber, string tokenId, string mUserid, [FromBody]string value)
        public HttpResponseMessage HouseBill([FromBody] HBSearchParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.HouseBillSearch(data.hbNumber,data.DONumber,data.SecurityCode,data.HBSearch, data.tokenId, data.mUserid, data.lang), System.Text.Encoding.UTF8, "application/json")

            };
        }

        [Route("Declaration")]
        // POST: api/User
        [HttpPost]
        //public HttpResponseMessage Declaration(string tempDeclNumber, string tokenId, string mUserid, [FromBody]string value)
        public HttpResponseMessage Declaration([FromBody] DeclarationSearchParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.DeclarationSearch(data.tempDeclNumber, data.tokenId, data.mUserid, data.lang), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("HSCode")]
        // POST: api/User
        [HttpPost]
        // public HttpResponseMessage HSCode(string data, string paramType, string tokenId, string mUserid, [FromBody]string value)
        public HttpResponseMessage HSCode([FromBody] HSCodeSearchParams  data)
        {
            return new HttpResponseMessage()
            {
            Content = new StringContent(MobileDataBase.HSCode(data.data, data.paramType, data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("HSCode/Tree")]
        // POST: api/User
        [HttpPost]
        // public HttpResponseMessage HSCode(string data, string paramType, string tokenId, string mUserid, [FromBody]string value)
        public HttpResponseMessage HSCodeTreeView([FromBody] HSCodeTreeViewParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.HSCodeTreeView(data.hsCodeId, data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
