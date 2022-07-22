using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;

namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/EServicesRequests")]
    public class EServicesRequestsController : ApiController//Controller
    {
        [Route("SubmitEserviceRequest")]
        [HttpPost]
        public HttpResponseMessage SubmitEserviceRequest([FromBody] EservicesRequests eServicesRequest)
        {

            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.SubmitEServiceRequest(eServicesRequest), System.Text.Encoding.UTF8, "application/json")
            };
        }


        [Route("InitiateExamRequest")]
        [HttpPost]
        public HttpResponseMessage InitiateExamRequest([FromBody] examRequestViewModel examRequestViewMD)
        {

            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.InitiateExamRequest(examRequestViewMD), System.Text.Encoding.UTF8, "application/json")
            };
        }


        [Route("getExamDetailsByEservicesRequestId")]
        [HttpPost]
        public HttpResponseMessage getExamDetailsByEservicesRequestId([FromBody] EservicesRequests EserviceRequest)
        {

            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.getExamDetailsByEservicesRequestId(EserviceRequest), System.Text.Encoding.UTF8, "application/json")
            };
        }


        [Route("GetRequestIfExists")]
        [HttpPost]
        public HttpResponseMessage GetRequestIfExists([FromBody] RequestExists RequestExist)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetRequestIfExists(RequestExist.RequestForUserType, RequestExist.RequesterUserId, RequestExist.ServiceId), System.Text.Encoding.UTF8, "application/json")
            };
        }



        [Route("GetRequestIfExistsByCivilId")]
        [HttpPost]
        public HttpResponseMessage GetRequestIfExistsByCivilId([FromBody] RequestExistsByCivilId RequestExistCivilId)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetRequestIfExistsByCivilId(RequestExistCivilId.CivilId , RequestExistCivilId.ServiceId, RequestExistCivilId.StateId), System.Text.Encoding.UTF8, "application/json")
            };
        }


        [Route("updateExamCandidateInfo")]
        [HttpPost]
        public HttpResponseMessage updateExamCandidateInfo([FromBody] ExamCandidateInfo examCandidateInfo)
        {

            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.updateExamCandidateInfo(examCandidateInfo), System.Text.Encoding.UTF8, "application/json")
            };
        }



        [Route("getUserDashBoard")]
        [HttpPost]
        public HttpResponseMessage getUserDashBoard([FromBody] User user)
        {

            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.getUserDashBoard(user), System.Text.Encoding.UTF8, "application/json")
            };
        }


    }
}