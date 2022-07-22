using ETradeAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;

using ETradeAPI.Models;

namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/GCSKnet")]
    public class GCSKnetController : ApiController
    {
        [Route("IsReceiptValid")]
        [HttpPost]
        public IHttpActionResult IsReceiptValid([FromBody] GCSReqObj data)
        {

            ReceiptAction ReceiptAction = GCSKnetDatabase.IsReceiptValid(data);
            return Ok(ReceiptAction);
        }
        [Route("VerifyReceiptDetailsforGCSSite")]
        [HttpPost]
        public IHttpActionResult VerifyReceiptDetailsforGCSSite([FromBody] GCSReqObj data)
        {
          
                VerifyReceiptDetailsforGCSSite VerifyReceiptDetailsforGCSSite=GCSKnetDatabase.VerifyReceiptDetailsforGCSSite(data.CommonData,data.CommonData1,data.CommonData2,data.CommonData3,data.CommonData4);
            return Ok(VerifyReceiptDetailsforGCSSite);
        }
        [Route("GetPaymentDetailsforGCSSite")]
        [HttpPost]
        public IHttpActionResult GetPaymentDetailsforGCSSite([FromBody] GCSReqObj data)
        {
         
            ReceiptDetailsMinified rd = null;
            DataSet ds =GCSKnetDatabase.GetPaymentDetailsforGCSSite(data.CommonData,data.CommonData1,data.CommonData2,data.CommonData3,data.CommonData5,true);
           ds.Tables[0].Rows[0]["Amount"] = Convert.ToDecimal(ds.Tables[0].Rows[0]["Amount"]);// + Convert.ToDecimal(0.210);

            rd = GCSKnetDatabase.BindData<ReceiptDetailsMinified>(ds.Tables[0]);

            return Ok(rd);
        }
        [Route("ExplicitDecryptTokenCall")]
        [HttpPost]
        public IHttpActionResult ExplicitDecryptTokenCall([FromBody] GCSReqObj data)
        {
            string DecryptedToken = GCSKnetDatabase.ExplicitDecryptTokenCall(data.CommonData);
            GCSResp GCSResp = new GCSResp { CommonData = DecryptedToken };
            return Ok(GCSResp);
        }
        [Route("Encrypt")]
        [HttpPost]
        public IHttpActionResult Encrypt([FromBody] GCSReqObj data)
        {
            string EncryptedToken = GCSKnetDatabase.Encrypt(data.CommonData);
            GCSResp GCSResp = new GCSResp { CommonData = EncryptedToken };
            return Ok(GCSResp);
        }



    }
}
