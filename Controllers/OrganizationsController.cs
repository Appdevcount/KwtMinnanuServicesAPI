using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;

namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/Organizations")]
    public class OrganizationsController : ApiController
    {
        [Route("List")]
        //[Route("List/{tokenId}/{mUserid}")]
        [HttpPost]
        public HttpResponseMessage OrgListInfo([FromBody] SecurityParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetOrgsAssociated(data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }

        //[Route("ReqForUpdate/{tokenId}/{mUserid}/{OrganizationId}")]
        [Route("ReqForUpdate")]
        [HttpPost]
        public HttpResponseMessage ReqForUpdate([FromBody] DeclarationSearchByIdParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.OrgReqForUpdate(data.tokenId, data.mUserid, data.OrganizationId,data.ApprovedDetail), System.Text.Encoding.UTF8, "application/json")
            };
        }
        
    }
}
