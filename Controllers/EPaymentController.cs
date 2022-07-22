using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;

namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/EPayment")]
    public class EPaymentController : ApiController
    {

        [Route("EPaymentRequestDetails")]
        // POST: api/User
        [HttpPost]
        public HttpResponseMessage EPaymentRequestDetails([FromBody] EPaymentRequestDetailsParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.EPaymentRequestDetails(data.tokenId, data.mUserid, data.lang, data.pagenumber, data.searchCriteria, data.searchDropdown), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("GetEpaymentSearchDropdown")]
        // POST: api/User
        [HttpPost]
        public HttpResponseMessage GetEpaymentSearchDropdown([FromBody] langParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetEpaymentSearchDropdown(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("EPaymentReceiptPrintHtml")]
        // POST: api/User
        [HttpPost]
        public HttpResponseMessage EPaymentReceiptPrintHtml([FromBody] EPaymentReceipt data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.EPaymentReceiptPrintHtml(data), System.Text.Encoding.UTF8, "application/json")
            };
        }


[Route("ERequestPrintHtml")]
        // POST: api/User
        [HttpPost]
        public HttpResponseMessage ERequestPrintHtml([FromBody] BrokerUpdateModel data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ERequestPrintHtml(data), System.Text.Encoding.UTF8, "application/json")
            };
        }
      
        [Route("EPaymentRequestInfo")]
        // POST: api/User
        [HttpPost]
        public HttpResponseMessage EPaymentRequestInfo([FromBody] EPaymentRequestInfoParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.EPaymentRequestInfo(data.tokenId, data.mUserid, data.RequestNo, data.lang), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("DenyPayRequest")]
        // POST: api/User
        [HttpPost]
        public HttpResponseMessage DenyPayRequest([FromBody] EPaymentRequestInfoParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.DenyPayRequest(data.tokenId, data.mUserid, data.RequestNo, data.lang), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("EPaymentOnCallbackRedirect")]
        // POST: api/User
        [HttpPost]
        public HttpResponseMessage EPaymentOnCallbackRedirect([FromBody] CallbackRedirectInfo data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UpdateCallbackRedirectInfo(data.tokenId, data.mUserid, data.OPTokenId, data.RedirectUrl, data.EpayReqNo), System.Text.Encoding.UTF8, "application/json")
            };
        }


    }
}
