using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;
using ETradeAPI.Helpers;
using System.Text.RegularExpressions;


namespace ETradeAPI.Controllers
{
    // [AllowAnonymous]
    // [RequireHttps]
    [RoutePrefix("api/DROrgUser")]
    public class DROrgUserController : ApiController
    {

        [Route("VerifyDROrgDetails")]
        [HttpPost]
        public HttpResponseMessage GetDROrgDetails([FromBody] ReqObj data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetDROrgDetails(data.CommonData,data.CommonData1,data.CommonData2), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("GetDROrgUsereqDetails")]
        [HttpPost]
        public HttpResponseMessage GetDROrgUsereqDetails([FromBody] ReqObj data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetDROrgUsereqDetails(data.CommonData), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("ResubmitDROrgUserRegistrationRequest")]
        [HttpPost]
        public HttpResponseMessage ResubmitDROrgUserRegistrationRequest([FromBody] ReqObj data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ResubmitDROrgUserRegistrationRequest(data.CommonData,data.File), System.Text.Encoding.UTF8, "application/json")
            };
        }


        [Route("CancelDROrgUserRegistration")]
        [HttpPost]
        public HttpResponseMessage CancelDROrgUserRegistration([FromBody] ReqObj data)
        {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(MobileDataBase.CancelDROrgUserRegistration(data.CommonData), System.Text.Encoding.UTF8, "application/json")
                };
        }
      
           
        

      

    }
}
