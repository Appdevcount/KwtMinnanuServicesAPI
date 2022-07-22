using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ETradeAPI.Models;
using System.Web.Http.Description;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;


namespace ETradeAPI.Controllers
{
    [RoutePrefix("api/OrgReq")]
    public class OrgReqController : ApiController
    {

        //[Route("Get/{tokenId}/{mUserid}/{sOrgReqId}")]
        [Route("Get")]
        [HttpPost]
        public HttpResponseMessage OrgReqResultInfo([FromBody] OrgReqResultInfoParams data)// string tokenId, string mUserid, string sOrgReqId)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetOrgReqResultInfo(data.sOrgReqId, data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("OrgRequestPrintHtml")]
        // POST: api/User
        [HttpPost]
        public HttpResponseMessage OrgRequestPrintHtml([FromBody] OrgReqResultInfoParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.OrgRequestPrintHtmlDS(data.sOrgReqId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }
        //pending
        // [Route("SubmitOrgReq/{tokenId}/{mUserid}/{sOrgReqId}")]
        [Route("SubmitOrgReq")]
        [HttpPost]
        public HttpResponseMessage SubmitOrgReq([FromBody] OrgReqResultInfoParams data)
        {
            
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.SubmitOrgReq(data.sOrgReqId, data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("UniqueTradeLicenseCheck")]
        [HttpPost]
        public HttpResponseMessage UniqueTradeLicenseCheck([FromBody] ReqObj data)
        {

            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UniqueTradeLicenseCheck(data.CommonData,data.CommonData1,data.CommonData2), System.Text.Encoding.UTF8, "application/json")
            };
        }
        //azhar
        [Route("GetuserOrgMap")]
        [HttpPost]
    
        public HttpResponseMessage GetuserOrgMap([FromBody] OrgRequest objOrgRequest)//[FromBody] OrgReqValues
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetuserOrgMap(objOrgRequest.tokenId, objOrgRequest.mUserid, objOrgRequest), System.Text.Encoding.UTF8, "application/json")
            };
        }


        [Route("CheckOrgEmailIsVerified")]
        [HttpPost]
        public HttpResponseMessage CheckOrgEmailIsVerified([FromBody] OrgReqResultInfoParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.CheckOrgEmailIsVerified(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("VerifyOrgEmail")]
        [HttpPost]
        public HttpResponseMessage VerifyOrgEmail([FromBody] VerifyOTPParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.VerifyOrgEmail(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [Route("SubmitOrgReqHtml")]
        [HttpPost]
        public HttpResponseMessage SubmitOrgReqHtml([FromBody] OrgReqResultInfoParams data)
        {
            var response = new HttpResponseMessage();
            string sHTML = MobileDataBase.SubmitOrgReqHtml(data.sOrgReqId, data.tokenId, data.mUserid);

            ResponseData RD = new ResponseData();
            RD.data = sHTML;

       //     response.Content = new StringContent(sHTML);
       //     response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
       //     return response;

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(RD, Formatting.None), System.Text.Encoding.UTF8, "application/json")
            };

        }

        //[Route("List/{tokenId}/{mUserid}")]
        [Route("List")]
        [HttpPost]
        public HttpResponseMessage ListOrgRequests([FromBody] SecurityParams data)// string tokenId, string mUserid)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ListOrgRequests(data.tokenId, data.mUserid), System.Text.Encoding.UTF8, "application/json")
            };
        }

        //[Route("CreateUpdate/{tokenId}/{mUserid}")]
        [Route("CreateUpdate")]
        [HttpPost]
        [ResponseType(typeof(OrgRequest))]
        public HttpResponseMessage CreateUpdateOrgReq( [FromBody] OrgRequest objOrgRequest)//[FromBody] OrgReqValues
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.CreateUpdateOrgReqBasicInfo(objOrgRequest.tokenId, objOrgRequest.mUserid, objOrgRequest), System.Text.Encoding.UTF8, "application/json")
            }; 
        }

        [Route("DeleteOrgReq")]
        [HttpPost]
        [ResponseType(typeof(OrgRequest))]
        public HttpResponseMessage DeleteOrgReq([FromBody] OrgRequestWithId objOrgRequest)//[FromBody] OrgReqValues
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.DeleteOrgReq(objOrgRequest), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [Route("ManageOrgAuthPerson")]
        [HttpPost]
        public HttpResponseMessage ManageOrgAuthPerson([FromBody] OrgAuthorizedSignatory OrgAuthPerson)//[FromBody] OrgReqValues
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.ManageOrgAuthPerson(OrgAuthPerson), System.Text.Encoding.UTF8, "application/json")
            };
        }
        //[Route("UpdateIndLic/{tokenId}/{mUserid}")]
        [Route("UpdateIndLic")]
        [HttpPost]
        public HttpResponseMessage UpdateIndLic([FromBody] OrgRequestInd objOrgRequest)//[FromBody] OrgReqValues
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UpdateOrgReqIndLicInfo(objOrgRequest), System.Text.Encoding.UTF8, "application/json")
            };
        }

        // [Route("UpdateCommLic/{tokenId}/{mUserid}")]
        [Route("UpdateCommLic")]
        [HttpPost]
        public HttpResponseMessage UpdateCommLic([FromBody]OrgRequestComm objOrgRequest)//[FromBody] OrgReqValues
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UpdateOrgReqCommLicInfo(objOrgRequest), System.Text.Encoding.UTF8, "application/json")
            };
        }

        //[Route("UpdateImpLic/{tokenId}/{mUserid}")]
        [Route("UpdateImpLic")]
        [HttpPost]
        public HttpResponseMessage UpdateImpLic([FromBody]OrgRequestImp objOrgRequest)//[FromBody] OrgReqValues
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.UpdateOrgReqImpLicInfo(objOrgRequest), System.Text.Encoding.UTF8, "application/json")
            };
        }



        //[Route("CommLicTypes")]
        //[HttpGet]
        //public HttpResponseMessage GetCommLicTypes(string lang)//[FromBody] OrgReqValues
        //{
        //    return new HttpResponseMessage()
        //    {
        //        Content = new StringContent(MobileDataBase.GetTypes("Usp_MApp_GetCommLicTypes", lang), System.Text.Encoding.UTF8, "application/json")
        //    };
        //}

        //[Route("CommLicSubTypes/{lang}")]
        [Route("CommLicSubTypes")]
        [HttpPost]
        public HttpResponseMessage GetCommLicSubTypes([FromBody] langParams data)//[FromBody] OrgReqValues
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.GetTypes("etrade.Usp_MApp_GetCommLicSubTypes", data.lang), System.Text.Encoding.UTF8, "application/json")
            };
        }




        #region CheckuserOrganizationMap Azhar
        //internal static string GetuserOrgMap(string tokenId, string mUserid, OrgRequest objOrgRequest)
        //{
        //    Result rslt = GetValidUserDetails(tokenId, mUserid);
        //    if (rslt.status == "0")
        //    {
        //        rslt.Data = GetuserOrgMapDs(rslt.mUserId, objOrgRequest);
        //    }
        //    return JsonConvert.SerializeObject(rslt, Formatting.None);
        //}

        //private static DataSet GetuserOrgMapDs(string mUserid, OrgRequest objOrgRequest)
        //{
        //    DataSet Ds = new DataSet();
        //    try
        //    {
        //        using (var sCon = new SqlConnection(connectionStr))
        //        {
        //            using (var sCmd = new SqlCommand("etrade.Sp_checkOrgUserMappingEservices", sCon))
        //            {
        //                sCmd.CommandType = CommandType.StoredProcedure;

        //                sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserid;
        //                sCmd.Parameters.Add("@TradeLicenseNumber", SqlDbType.VarChar).Value = objOrgRequest.TradeLicNumber;



        //                SqlDataAdapter da = new SqlDataAdapter(sCmd);
        //                da.Fill(Ds);

        //                for (int i = 0; i < Ds.Tables.Count; i++)
        //                {
        //                    if (Ds.Tables[i].Columns.Contains("TableName"))
        //                    {
        //                        if (Ds.Tables[i].Rows.Count > 0)
        //                            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
        //                    }
        //                    else
        //                    {
        //                        Ds.Tables.RemoveAt(i);
        //                        i--;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return Ds;
        //}
        #endregion CreateUpdateOrgReq


        //[Route("ImpLicTypes")]
        //[HttpGet]
        //public HttpResponseMessage GetImpLicTypes(string lang)//[FromBody] OrgReqValues
        //{
        //    return new HttpResponseMessage()
        //    {
        //        Content = new StringContent(MobileDataBase.GetTypes("Usp_MApp_GetImpLicTypes", lang), System.Text.Encoding.UTF8, "application/json")
        //    };
        //}
    }
}
