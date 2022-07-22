using ETradeAPI.Models;
using MicroClear.EnterpriseSolutions.CryptographyServices;
using MicroClear.EnterpriseSolutions.ServiceFactories;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using WebApplication1.Models;
using System.Xml.Linq;
//using TokenGeneration;
using System.Diagnostics;
using System.Collections.Generic;

using ETradeAPI.Service_References.MociLicensesServiceControl;
using System.Web;

namespace ETradeAPI
{
    public class MobileDataBase
    {
        private static String connectionStr = String.Empty;

        private static readonly string FreeServiceredirecturl = ConfigurationManager.AppSettings["FreeServiceredirecturl"].ToString();
        internal static void SendSMSViaAPI(String SMSMessage, String ToMobileNo)
        {
            // return;
            //SMSBox.SendingSMSResult Result = SendSMS(SMSMessage, ToMobileNo);
            //String[] RejectedMSISDN = Result.RejectedNumbers;
            //int? Status = Result.Result ? 3 : (RejectedMSISDN.Count() > 0 ? 2 : 1);
        }



        private static SMSBox.SendingSMSResult SendSMS(String MessageBody, String RecipientNumbers)
        {
            String UserName, Password, SenderText;
            int CustomerID;
            bool IsBlink, IsFlash;

            SenderText = ConfigurationManager.AppSettings["SenderText"];  //ConfigurationSettings.AppSettings["SenderText"];
            UserName = ConfigurationManager.AppSettings["UserName"];
            Password = ConfigurationManager.AppSettings["Password"];
            CustomerID = Convert.ToInt16(ConfigurationManager.AppSettings["CustomerID"]);

            IsBlink = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBlink"]);
            IsFlash = Convert.ToBoolean(ConfigurationManager.AppSettings["IsFlash"]);

            SMSBox.SoapUser UserDet = new SMSBox.SoapUser { Username = UserName, Password = Password, CustomerId = CustomerID };
            SMSBox.SendingSMSRequest SMSReq = new SMSBox.SendingSMSRequest
            {
                User = UserDet,
                SenderText = SenderText,
                MessageBody = MessageBody,
                RecipientNumbers = RecipientNumbers,
                IsBlink = IsBlink,
                IsFlash = IsFlash
            };
            SMSBox.MessagingSoapClient SMS = new SMSBox.MessagingSoapClient("MessagingSoap");
            SMSBox.SendingSMSResult result = SMS.SendSMS(SMSReq);

            //     Task<SMSBox.SendingSMSResult> result1 = SMS.SendSMSAsync(SMSReq);

            return result;
        }

        public class Result
        {
            public String mUserId = "",
                emailId = "",
                status = "-1";
            [JsonIgnore]
            public String mC_UserId, mobileNumber, lang = "", personalId = "";

            public DataSet Data = null;

            public DataTable dataTable = null;
        }

        public MobileDataBase()
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private static Random random = new Random();
        public static String RandomToken(int length)
        {
            const String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new String(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static String RandomCaseToken(int length)
        {
            const String chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new String(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        internal static String GetNotifications(String tokenId, String mUserid)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetNotificationsDS(rslt.mUserId, rslt.lang);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }


        internal static String SubmitOrgReqHtml(String sOrgReqId, String tokenId, String mUserid)
        {
            String sHTML = String.Empty;
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                sHTML = GetOrganizationReqSubmitPrint(sOrgReqId);
            }
            return sHTML;
        }


        #region  user activty data 

        internal static String UpdateUserActivityDetails(LogUserActivity userActivityData)
        {
            //Result rslt = GetValidUserDetails(userData.tokenId, userData.mUserid);

            Result rslt = new Result();
            rslt.mUserId = userActivityData.LoginUserid;

            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = UpdateUserActivityDetailsDS(userActivityData);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }


        internal static DataSet UpdateUserActivityDetailsDS(LogUserActivity userActivityData)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();

            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.SP_InsertUserActivityAudit", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;


                        if (String.IsNullOrEmpty(userActivityData.LoginUserid))
                            sCmd.Parameters.Add("@LoginUserid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@LoginUserid", SqlDbType.VarChar).Value = userActivityData.LoginUserid;

                        if (String.IsNullOrEmpty(userActivityData.Serviceid))
                            sCmd.Parameters.Add("@Serviceid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Serviceid", SqlDbType.VarChar).Value = userActivityData.Serviceid;
                        if (String.IsNullOrEmpty(userActivityData.sessionId))
                            sCmd.Parameters.Add("@sessionId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@sessionId", SqlDbType.VarChar).Value = userActivityData.sessionId;
                        if (String.IsNullOrEmpty(userActivityData.LoginTime))
                            sCmd.Parameters.Add("@LoginTime", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@LoginTime", SqlDbType.VarChar).Value = userActivityData.LoginTime;
                        if (String.IsNullOrEmpty(userActivityData.LogOutTime))
                            sCmd.Parameters.Add("@LogOutTime", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@LogOutTime", SqlDbType.VarChar).Value = userActivityData.LogOutTime;
                        if (String.IsNullOrEmpty(userActivityData.McUserName))
                            sCmd.Parameters.Add("@McUserName", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@McUserName", SqlDbType.VarChar).Value = userActivityData.McUserName;


                        if (String.IsNullOrEmpty(userActivityData.McUserOrgId))
                            sCmd.Parameters.Add("@McUserOrgId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@McUserOrgId", SqlDbType.Int).Value = userActivityData.McUserOrgId;



                        if (String.IsNullOrEmpty(userActivityData.ActivityPerformed))
                            sCmd.Parameters.Add("@ActivityPerformed", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ActivityPerformed", SqlDbType.VarChar).Value = userActivityData.ActivityPerformed;


                        if (String.IsNullOrEmpty(userActivityData.IPAddress))
                            sCmd.Parameters.Add("@IPAddress", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IPAddress", SqlDbType.VarChar).Value = userActivityData.IPAddress;

                        if (String.IsNullOrEmpty(userActivityData.legalentity))
                            sCmd.Parameters.Add("@legalentity", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@legalentity", SqlDbType.VarChar).Value = userActivityData.legalentity;

                        if (String.IsNullOrEmpty(userActivityData.SignInSignOut))
                            sCmd.Parameters.Add("@SignInSignOut", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@SignInSignOut", SqlDbType.VarChar).Value = userActivityData.SignInSignOut;

                        sCmd.Parameters.Add("@OrganizationService", SqlDbType.Bit).Value = userActivityData.OrganizationService;//added newly

                        sCmd.Parameters.Add("@ClearingAgentservice", SqlDbType.Bit).Value = userActivityData.ClearingAgentservice;//added newly
                        if (String.IsNullOrEmpty(userActivityData.OtherAdditionalInfo))
                            sCmd.Parameters.Add("@OtherAdditionalInfo", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OtherAdditionalInfo", SqlDbType.NVarChar).Value = userActivityData.OtherAdditionalInfo;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);




                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }


        #endregion

        private static String GetOrganizationReqSubmitPrint(String sOrgReqId)
        {
            String RetHtml = String.Empty;
            DataSet sDataResult = new DataSet("DS");

            try
            {
                using (var sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_GetOrgReqDetailsForPrint", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(sOrgReqId))
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = Convert.ToInt32(sOrgReqId);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(sDataResult);

                        if (sDataResult != null && sDataResult.Tables.Count > 0)
                        {
                            //String sProjectPath = ConfigurationManager.AppSettings["ProjectPath"].ToString();
                            String BodyHtmlFile = "~/OrganizationRequestSubmit.html";
                            FileStream fsreader = new FileStream(System.Web.HttpContext.Current.Server.MapPath(BodyHtmlFile), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                            StreamReader reader = new StreamReader(fsreader);
                            String readFile = reader.ReadToEnd();

                            fsreader.Dispose();
                            reader.Dispose();

                            String myString = "";
                            myString = readFile;
                            for (int i = 30; i >= 0; i--)
                            {
                                if (sDataResult.Tables[0].Rows[0]["#" + Convert.ToString(i)].ToString() != null)
                                    myString = myString.Replace("#" + Convert.ToString(i), sDataResult.Tables[0].Rows[0]["#" + Convert.ToString(i)].ToString());
                            }

                            //var inlineLogo = new LinkedResource(System.Web.HttpContext.Current.Server.MapPath("~/eTrade.png"));
                            //inlineLogo.ContentId = "DFFD1A8F-5393-4A67-9531-CBA0854B00D2";

                            //var view = AlternateView.CreateAlternateViewFromString(objeto_mail.Body, null, "text/html");
                            //view.LinkedResources.Add(inlineLogo);
                            //objeto_mail.AlternateViews.Add(view);

                            String sImage = "~/KGACTransparentLogo.gif";
                            FileStream fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                            Byte[] imgByte = new byte[fsreader1.Length];

                            fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                            //byte[] imageArray = System.IO.File.ReadAllBytes(fsreader1.);
                            String LogInBase64 = Convert.ToBase64String(imgByte);
                            myString = myString.Replace("#KGACTransparentLogo", LogInBase64);
                            RetHtml = myString.ToString();


                            fsreader1.Dispose();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

            return RetHtml;
        }
        internal static String CheckOrgEmailIsVerified(OrgReqResultInfoParams data)
        {
            Result rslt = GetValidUserDetails(data.tokenId, data.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = CheckOrgEmailIsVerifiedDS(data.sOrgReqId, data.mUserid);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        internal static DataSet CheckOrgEmailIsVerifiedDS(String sOrgReqId, String sUserId)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            String IsOrgEmailVarified = String.Empty;

            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_CheckOrgEmailIsVerifiedDS", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (String.IsNullOrEmpty(sOrgReqId))
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.NVarChar).Value = sOrgReqId;
                        if (String.IsNullOrEmpty(sUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.NVarChar).Value = sUserId;

                        SqlParameter pm = new SqlParameter("@EmailKeyVal", SqlDbType.VarChar, 10);
                        pm.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(pm);
                        SqlParameter ParamEmailId = new SqlParameter("@EmailId", SqlDbType.VarChar, 500);
                        ParamEmailId.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(ParamEmailId);
                        SqlParameter ParamOrgName = new SqlParameter("@OrgName", SqlDbType.NVarChar, 500);
                        ParamOrgName.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(ParamOrgName);
                        SqlParameter ParamIsOrgEmailVarified = new SqlParameter("@IsOrgEmailVarified", SqlDbType.NVarChar, 5);
                        ParamIsOrgEmailVarified.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(ParamIsOrgEmailVarified);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        String EmailKeyVal = sCmd.Parameters["@EmailKeyVal"].Value.ToString();
                        String EmailId = sCmd.Parameters["@EmailId"].Value.ToString();
                        String OrgName = sCmd.Parameters["@OrgName"].Value.ToString();
                        IsOrgEmailVarified = sCmd.Parameters["@IsOrgEmailVarified"].Value.ToString();
                        if (EmailKeyVal != null && EmailKeyVal != "")
                        {
                            sendEmailViaWebApi(EmailId, EmailKeyVal, OrgName);
                        }

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ds;
        }

        internal static String SubmitOrgReq(String sOrgReqId, String tokenId, String mUserid)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = SubmitOrgReqDS(rslt.mUserId, sOrgReqId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }



        private static DataSet SubmitOrgReqDS(String mUserId, String sOrgReqId)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_SubmitOrgRequest", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserId);

                        if (String.IsNullOrEmpty(sOrgReqId))
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = Convert.ToInt32(sOrgReqId);


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                }
                //NotifyEmail("", sOrgReqId.ToString(), Convert.ToInt32(mUserId));// Org request submission handled at MC side itself
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String OrgReqForUpdate(String tokenId, String mUserid, String organizationId, bool ApprovedDetail)
        {
            //usp_MApp_GetNewFromOrganizationsDetails

            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = OrgReqForUpdateDS(rslt.mUserId, organizationId, ApprovedDetail);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }

        internal static String LogOutAction(String tokenId, String mUserid)
        {

            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = LogOutActionDS(rslt.mUserId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }

        internal static String UserDetails(String tokenId, String mUserid)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            rslt.mUserId = mUserid;
            if (rslt.status == "0")
            {
                rslt.Data = UserDetailsDS(rslt.mUserId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }

        private static DataSet UserDetailsDS(String mUserId)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_GetUserDetails", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserId);
                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        private static DataSet LogOutActionDS(String mUserId)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_LogOutAction", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserId);


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String DeleteOrgReqDocument(OrgReqResultDocInfoParams data)
        {
            //  Result rslt = new Result();

            Result rslt = GetValidUserDetails(data.tokenId, data.mUserid);
            rslt.status = "0";
            rslt.mUserId = data.mUserid;
            if (rslt.status == "0")
            {
                rslt.Data = DeleteOrgReqDocumentDS(rslt.mUserId, data.sOrgReqDocId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }

        #region EService



        #region CheckuserOrganizationMap Azhar
        internal static String GetuserOrgMap(String tokenId, String mUserid, OrgRequest objOrgRequest)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);

            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetuserOrgMapDs(rslt.mUserId, objOrgRequest);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet GetuserOrgMapDs(String mUserid, OrgRequest objOrgRequest)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_checkOrgUserMappingEservices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserid;
                        sCmd.Parameters.Add("@TradeLicenseNumber", SqlDbType.VarChar).Value = objOrgRequest.TradeLicNumber;



                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ds;
        }
        #endregion CreateUpdateOrgReq





        #region Eservice Azhar Sir

        internal static String DeleteOrgReqDocForEservice(OrgReqResultDocInfoParams data)
        {
            Result rslt = new Result();

            //Result rslt = GetValidUserDetails(data.tokenId, data.mUserid);
            rslt.status = "0";
            rslt.mUserId = data.mUserid;
            if (rslt.status == "0")
            {
                rslt.Data = DeleteOrgReqDocumentDSForEservice(data.EserviceRequestid, data.sOrgReqDocId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }

        private static DataSet DeleteOrgReqDocumentDSForEservice(String EserviceRequestid, String sOrgReqDocId)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.sp_DeleteUploadFileForBrokerEServices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(sOrgReqDocId))
                            sCmd.Parameters.Add("@sOrgReqDocId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@sOrgReqDocId", SqlDbType.VarChar).Value = sOrgReqDocId;

                        if (String.IsNullOrEmpty(EserviceRequestid))
                            sCmd.Parameters.Add("@EserviceRequestid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EserviceRequestid", SqlDbType.VarChar).Value = EserviceRequestid;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }



        internal static String DownloadOrgReqDocForEservice(OpenDocumentParams data)
        {
            Result rslt = new Result();

            //Result rslt = GetValidUserDetails(data.tokenId, data.mUserid);
            rslt.status = "0";
            rslt.mUserId = data.mUserid;
            if (rslt.status == "0")
            {
                rslt.Data = DownloadOrgReqDocumentDSForEservice(data.EserviceRequestid, data.DocumentId, data.hiderefprofile, data.tokenId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }

        private static DataSet DownloadOrgReqDocumentDSForEservice(String EserviceRequestid, String sOrgReqDocId, String hidreprofile,string sessionuserid)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_DwonloadFile_ForEservice", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(sOrgReqDocId))
                            sCmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = sOrgReqDocId;

                        if (String.IsNullOrEmpty(EserviceRequestid))
                            sCmd.Parameters.Add("@declarationid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@declarationid", SqlDbType.VarChar).Value = EserviceRequestid;


                        if (String.IsNullOrEmpty(hidreprofile))
                            sCmd.Parameters.Add("@hiderefProfile", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@hiderefProfile", SqlDbType.VarChar).Value = hidreprofile;

                        if (String.IsNullOrEmpty(sessionuserid))
                            sCmd.Parameters.Add("@sessionuserid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@sessionuserid", SqlDbType.VarChar).Value = sessionuserid;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String GetDocPathForEservice(String documentId, String hidreprofile, String eservicerequestid)
        {
            String sPath = String.Empty;
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_DwonloadFile_ForEservice", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (String.IsNullOrEmpty(documentId))
                            sCmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = documentId;

                        if (String.IsNullOrEmpty(documentId))
                            sCmd.Parameters.Add("@declarationid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@declarationid", SqlDbType.VarChar).Value = eservicerequestid;


                        if (String.IsNullOrEmpty(documentId))
                            sCmd.Parameters.Add("@hiderefProfile", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@hiderefProfile", SqlDbType.VarChar).Value = hidreprofile;

                        SqlParameter pm = new SqlParameter("@sPath", SqlDbType.NVarChar, 1000);

                        pm.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(pm);
                        sCon.Open();
                        sCmd.ExecuteNonQuery();
                        sPath = (sCmd.Parameters["@sPath"].Value == DBNull.Value) ? "" : sCmd.Parameters["@sPath"].Value.ToString();
                        sCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sPath;
        }


        #endregion Eservice Azhar Sir

        #region EserviceListDetailsEntityWise
        public static String GetEntityServiceList(BrokerServiceRequestModel frdata)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_GetEntityServiceList", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(frdata.userid))
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = frdata.userid;

                        //if (String.IsNullOrEmpty(frdata.CheckAvailableServicesforRequest))
                        //    sCmd.Parameters.Add("@Userid", SqlDbType.Bit).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@Userid", SqlDbType.Bit).Value = frdata.CheckAvailableServicesforRequest;

                        if (frdata.CheckAvailableServicesforRequest == null)
                            sCmd.Parameters.Add("@CheckAvailableServicesforRequest", SqlDbType.Char).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CheckAvailableServicesforRequest", SqlDbType.Char).Value = (bool)frdata.CheckAvailableServicesforRequest ? '1' : '0';



                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}// 

            catch (Exception ex)
            {

            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        public static String PostEntityServiceList(BrokerServiceRequestModel frdata)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_NewInsertBrokerServiceRequestList", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(frdata.userid))
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = frdata.userid;
                        if (String.IsNullOrEmpty(frdata.userid))
                            sCmd.Parameters.Add("@MobileUserid", SqlDbType.BigInt).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MobileUserid", SqlDbType.BigInt).Value = frdata.MobileUserid;
                        if (String.IsNullOrEmpty(frdata.userid))
                            sCmd.Parameters.Add("@RequestedForMobileUserid", SqlDbType.BigInt).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@RequestedForMobileUserid", SqlDbType.BigInt).Value = frdata.RequestedForMobileUserid;
                        if (String.IsNullOrEmpty(frdata.SelectedServiceids))
                            sCmd.Parameters.Add("@SelectedServiceids", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@SelectedServiceids", SqlDbType.VarChar).Value = frdata.SelectedServiceids;
                        if (String.IsNullOrEmpty(frdata.UnSelectedServiceids))
                            sCmd.Parameters.Add("@UnSelectedServiceids", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@UnSelectedServiceids", SqlDbType.VarChar).Value = frdata.UnSelectedServiceids;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                //NotifyEmail(Ds.Tables[0].Rows[0]["EServiceRequestNumber"].ToString(), "", 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        #endregion


        #endregion



        #region Eservice Abdul Karim
        internal static String SubmitEServiceRequest(EservicesRequests eServicesRequest)
        {
            Result rslt = new Result();

            rslt.status = "0";
            rslt.mUserId = "";
            if (rslt.status == "0")
            {
                rslt.dataTable = ExtractEserviceRequestValuesThenSubmit(eServicesRequest);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }


        private static DataTable ExtractEserviceRequestValuesThenSubmit(EservicesRequests eServicesRequest)
        {
            DataTable dataTable = new DataTable();

            if (eServicesRequest != null)
            {

                long EserviceRequestId = eServicesRequest.EserviceRequestId;

                long RequestForUserType = eServicesRequest.RequestsDetails[0].RequestForUserType;
                String RequestServicesId = eServicesRequest.RequestsDetails[0].RequestServicesId;
                //OrganizationId                 = eServicesRequest.RequestsDetails[0].OrganizationId;	
                String RequesterLicenseNumber = eServicesRequest.RequestsDetails[0].RequesterLicenseNumber;
                String RequesterArabicName = eServicesRequest.RequestsDetails[0].RequesterArabicName;
                String RequesterEnglishName = eServicesRequest.RequestsDetails[0].RequesterEnglishName;
                String ArabicFirstName = eServicesRequest.RequestsDetails[0].ArabicFirstName;
                String ArabicSecondName = eServicesRequest.RequestsDetails[0].ArabicSecondName;
                String ArabicThirdName = eServicesRequest.RequestsDetails[0].ArabicThirdName;
                String ArabicLastName = eServicesRequest.RequestsDetails[0].ArabicLastName;
                String EnglishFirstName = eServicesRequest.RequestsDetails[0].EnglishFirstName;
                String EnglishSecondName = eServicesRequest.RequestsDetails[0].EnglishSecondName;
                String EnglishThirdName = eServicesRequest.RequestsDetails[0].EnglishThirdName;
                String EnglishLastName = eServicesRequest.RequestsDetails[0].EnglishLastName;
                int Nationality = eServicesRequest.RequestsDetails[0].Nationality;
                String CivilID = eServicesRequest.RequestsDetails[0].CivilID;
                DateTime? CivilIDExpirydate = eServicesRequest.RequestsDetails[0].CivilIDExpiryDate;
                String MobileNumber = eServicesRequest.RequestsDetails[0].MobileNumber;
                String PassportNo = eServicesRequest.RequestsDetails[0].PassportNo;
                DateTime? PassportExpirydate = eServicesRequest.RequestsDetails[0].PassportExpiryDate;
                String Address = eServicesRequest.RequestsDetails[0].Address;
                String Email = eServicesRequest.RequestsDetails[0].Email;
                String LicenseNumber = eServicesRequest.RequestsDetails[0].LicenseNumber;
                DateTime? LicenseNumberExpirydate = eServicesRequest.RequestsDetails[0].LicenseNumberExpiryDate;
                String Remarks = eServicesRequest.RequestsDetails[0].Remarks;
                String RejectionRemarks = eServicesRequest.RequestsDetails[0].RejectionRemarks;
                Boolean RequestForVisit = eServicesRequest.RequestsDetails[0].RequestForVisit;
                String RequestForVisitRemarks = eServicesRequest.RequestsDetails[0].RequestForVisitRemarks;
                Int64 ExamAddmissionId = eServicesRequest.RequestsDetails[0].ExamAddmissionId;
                Int64 ExamDetailsId = eServicesRequest.RequestsDetails[0].ExamDetailsId;
                String status = eServicesRequest.RequestsDetails[0].status;
                String StateId = eServicesRequest.RequestsDetails[0].StateId;
                String gender = eServicesRequest.RequestsDetails[0].Gender;


                dataTable = SubmitEserviceRequest(RequestForUserType, RequestServicesId, RequesterLicenseNumber,
                RequesterArabicName, RequesterEnglishName, ArabicFirstName,
                ArabicSecondName, ArabicThirdName, ArabicLastName,
                EnglishFirstName, EnglishSecondName, EnglishThirdName,
                EnglishLastName, Nationality, CivilID,
                CivilIDExpirydate, MobileNumber, PassportNo,
                PassportExpirydate, Address, Email,
                LicenseNumber, LicenseNumberExpirydate, Remarks,
                RejectionRemarks, RequestForVisit, RequestForVisitRemarks,
                ExamAddmissionId, ExamDetailsId, status,
                StateId, EserviceRequestId, gender);
            }

            return dataTable;
        }


        private static DataTable SubmitEserviceRequest
            (long RequestForUserType, String RequestServicesId, String RequesterLicenseNumber,
              String RequesterArabicName, String RequesterEnglishName, String ArabicFirstName,
              String ArabicSecondName, String ArabicThirdName, String ArabicLastName,
              String EnglishFirstName, String EnglishSecondName, String EnglishThirdName,
              String EnglishLastName, int Nationality, String CivilID,
              DateTime? CivilIDExpirydate, String MobileNumber, String PassportNo,
              DateTime? PassportExpirydate, String Address, String Email,
              String LicenseNumber, DateTime? LicenseNumberExpirydate, String Remarks,
              String RejectionRemarks, Boolean RequestForVisit, String RequestForVisitRemarks,
              Int64 ExamAddmissionId, Int64 ExamDetailsId, String status,
              String StateId, long EserviceRequestId, String gender)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.SubmitEserviceRequest", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@EserviceRequestId", SqlDbType.BigInt).Value = EserviceRequestId;

                        sCmd.Parameters.Add("@RequestForUserType", SqlDbType.BigInt).Value = RequestForUserType;
                        sCmd.Parameters.Add("@RequestServicesId", SqlDbType.VarChar).Value = RequestServicesId;
                        sCmd.Parameters.Add("@RequesterLicenseNumber", SqlDbType.NVarChar).Value = RequesterLicenseNumber; // check
                        sCmd.Parameters.Add("@RequesterArabicName", SqlDbType.NVarChar).Value = RequesterArabicName; // check
                        sCmd.Parameters.Add("@RequesterEnglishName", SqlDbType.NVarChar).Value = RequesterEnglishName; // check
                        sCmd.Parameters.Add("@ArabicFirstName", SqlDbType.NVarChar).Value = ArabicFirstName;
                        sCmd.Parameters.Add("@ArabicSecondName", SqlDbType.NVarChar).Value = ArabicSecondName;
                        sCmd.Parameters.Add("@ArabicThirdName", SqlDbType.NVarChar).Value = ArabicThirdName;
                        sCmd.Parameters.Add("@ArabicLastName", SqlDbType.NVarChar).Value = ArabicLastName;
                        sCmd.Parameters.Add("@EnglishFirstName", SqlDbType.VarChar).Value = EnglishFirstName;
                        sCmd.Parameters.Add("@EnglishSecondName", SqlDbType.VarChar).Value = EnglishSecondName;
                        sCmd.Parameters.Add("@EnglishThirdName", SqlDbType.VarChar).Value = EnglishThirdName;
                        sCmd.Parameters.Add("@EnglishLastName", SqlDbType.VarChar).Value = EnglishLastName;
                        sCmd.Parameters.Add("@Nationality", SqlDbType.Int).Value = Nationality;
                        sCmd.Parameters.Add("@CivilID", SqlDbType.VarChar).Value = CivilID;
                        sCmd.Parameters.Add("@CivilIDExpirydate", SqlDbType.Date).Value = CivilIDExpirydate;
                        sCmd.Parameters.Add("@MobileNumber", SqlDbType.VarChar).Value = MobileNumber;
                        sCmd.Parameters.Add("@PassportNo", SqlDbType.NVarChar).Value = PassportNo;
                        sCmd.Parameters.Add("@PassportExpirydate", SqlDbType.Date).Value = PassportExpirydate;
                        sCmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = Address;
                        sCmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = Email;
                        sCmd.Parameters.Add("@LicenseNumber", SqlDbType.NVarChar).Value = LicenseNumber;
                        sCmd.Parameters.Add("@gender", SqlDbType.Char).Value = gender.Trim();

                        if (LicenseNumberExpirydate != null)
                        {
                            sCmd.Parameters.Add("@LicenseNumberExpirydate", SqlDbType.Date).Value = LicenseNumberExpirydate;
                        }
                        else
                        {
                            sCmd.Parameters.Add("@LicenseNumberExpirydate", SqlDbType.Date).Value = DBNull.Value;
                        }

                        sCmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = Remarks;
                        sCmd.Parameters.Add("@RejectionRemarks", SqlDbType.NVarChar).Value = DBNull.Value;
                        sCmd.Parameters.Add("@RequestForVisit", SqlDbType.Bit).Value = DBNull.Value;
                        sCmd.Parameters.Add("@RequestForVisitRemarks", SqlDbType.NVarChar).Value = DBNull.Value;
                        sCmd.Parameters.Add("@ExamAddmissionId", SqlDbType.BigInt).Value = DBNull.Value;
                        sCmd.Parameters.Add("@ExamDetailsId", SqlDbType.BigInt).Value = DBNull.Value;
                        sCmd.Parameters.Add("@status", SqlDbType.VarChar).Value = status;
                        sCmd.Parameters.Add("@StateIdDetails", SqlDbType.VarChar).Value = StateId;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(dataTable);

                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].RequestForUserType", SqlDbType.BigInt).Value = RequestForUserType;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].RequestServicesId", SqlDbType.VarChar).Value = RequestServicesId;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].RequesterLicenseNumber", SqlDbType.NVarChar).Value = RequesterLicenseNumber;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].RequesterArabicName", SqlDbType.NVarChar).Value = RequesterArabicName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].RequesterEnglishName", SqlDbType.NVarChar).Value = RequesterEnglishName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].ArabicFirstName", SqlDbType.NVarChar).Value = ArabicFirstName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].ArabicSecondName", SqlDbType.NVarChar).Value = ArabicSecondName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].ArabicThirdName", SqlDbType.NVarChar).Value = ArabicThirdName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].ArabicLastName", SqlDbType.NVarChar).Value = ArabicLastName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].EnglishFirstName", SqlDbType.VarChar).Value = EnglishFirstName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].EnglishSecondName", SqlDbType.VarChar).Value = EnglishSecondName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].EnglishThirdName", SqlDbType.VarChar).Value = EnglishThirdName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].EnglishLastName", SqlDbType.VarChar).Value = EnglishLastName;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].Nationality", SqlDbType.Int).Value = Nationality;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].CivilID", SqlDbType.VarChar).Value = CivilID;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].CivilIDExpirydate", SqlDbType.Date).Value = CivilIDExpirydate;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].MobileNumber", SqlDbType.VarChar).Value = MobileNumber;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].PassportNo", SqlDbType.NVarChar).Value = PassportNo;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].PassportExpirydate", SqlDbType.Date).Value = PassportExpirydate;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].Address", SqlDbType.NVarChar).Value = Address;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].Email", SqlDbType.VarChar).Value = Email;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].LicenseNumber", SqlDbType.NVarChar).Value = LicenseNumber;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].LicenseNumberExpirydate", SqlDbType.Date).Value = LicenseNumberExpirydate;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].Remarks", SqlDbType.NVarChar).Value = Remarks;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].RejectionRemarks", SqlDbType.NVarChar).Value = DBNull.Value;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].RequestForVisit", SqlDbType.Bit).Value = DBNull.Value;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].RequestForVisitRemarks", SqlDbType.NVarChar).Value = DBNull.Value;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].ExamAddmissionId", SqlDbType.BigInt).Value = DBNull.Value;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].ExamDetailsId", SqlDbType.BigInt).Value = DBNull.Value;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].status", SqlDbType.VarChar).Value = status;
                        //sCmd.Parameters.Add("eServicesRequest.RequestsDetails[0].StateIdDetails", SqlDbType.VarChar).Value = StateId;
                    }
                }
                //NotifyEmail(dataTable.Rows[0]["EserviceRequestNumber"].ToString(), "", 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataTable;
        }




        internal static String GetRequestIfExists(String RequestForUserType, String RequesterUserId, String ServiceId)
        {
            Result rslt = new Result();

            rslt.status = "0";
            rslt.mUserId = "";
            if (rslt.status == "0")
            {
                rslt.dataTable = GetRequestExists(RequestForUserType, RequesterUserId, ServiceId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }



        private static DataTable GetRequestExists(String RequestForUserType, String RequesterUserId, String ServiceId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetRequestIfExists", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@ServiceId", SqlDbType.BigInt).Value = ServiceId;

                        sCmd.Parameters.Add("@RequestForUserType", SqlDbType.BigInt).Value = RequestForUserType;
                        sCmd.Parameters.Add("@RequesterUserId", SqlDbType.BigInt).Value = RequesterUserId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataTable;
        }





        internal static String GetRequestIfExistsByCivilId(String CivilId, String serviceId, String stateId)
        {
            Result rslt = new Result();

            rslt.status = "0";
            rslt.mUserId = "";
            if (rslt.status == "0")
            {
                rslt.dataTable = GetRequestExistsByCivilId(CivilId, serviceId, stateId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None); ;
        }



        private static DataTable GetRequestExistsByCivilId(String CivilId, String serviceId, String stateId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetRequestIfExistsByCivilId", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@civilId", SqlDbType.VarChar).Value = CivilId;
                        sCmd.Parameters.Add("@serviceId", SqlDbType.BigInt).Value = serviceId;
                        sCmd.Parameters.Add("@stateId", SqlDbType.VarChar).Value = stateId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataTable;
        }



        #region OLD SubmitEserviceRequest 
        //private static DataSet SubmitEserviceRequest(EservicesRequests eServicesRequest)
        //{
        //    DataSet Ds = new DataSet();
        //    try
        //    {
        //        using (var sCon = new SqlConnection(connectionStr))
        //        {
        //            using (var sCmd = new SqlCommand("etrade.SubmitEserviceRequest", sCon))
        //            {
        //                sCmd.CommandType = CommandType.StoredProcedure;

        //                if (eServicesRequest != null)
        //                {
        //                    sCmd.Parameters.Add("@RequestForUserType", SqlDbType.BigInt).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@RequestServicesId", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@RequesterLicenseNumber", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@RequesterArabicName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@RequesterEnglishName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@ArabicFirstName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@ArabicSecondName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@ArabicThirdName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@ArabicLastName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@EnglishFirstName", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@EnglishSecondName", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@EnglishThirdName", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@EnglishLastName", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@Nationality", SqlDbType.Int).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@CivilID", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@CivilIDExpirydate", SqlDbType.Date).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@MobileNumber", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@PassportNo", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@PassportExpirydate", SqlDbType.Date).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@LicenseNumber", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@LicenseNumberExpirydate", SqlDbType.Date).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@RejectionRemarks", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@RequestForVisit", SqlDbType.Bit).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@RequestForVisitRemarks", SqlDbType.NVarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@ExamAddmissionId", SqlDbType.BigInt).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@ExamDetailsId", SqlDbType.BigInt).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@status", SqlDbType.VarChar).Value = DBNull.Value;
        //                    sCmd.Parameters.Add("@status", SqlDbType.VarChar).Value = DBNull.Value;

        //                    SqlDataAdapter da = new SqlDataAdapter(sCmd);
        //                    da.Fill(Ds);
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
        #endregion OLD SubmitEserviceRequest


        #endregion Eservice Abdul Karim









        private static DataSet DeleteOrgReqDocumentDS(String mUserId, String sOrgReqDocId)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_DeleteOrgReqDocument", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(sOrgReqDocId))
                            sCmd.Parameters.Add("@sOrgReqDocId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@sOrgReqDocId", SqlDbType.Int).Value = Convert.ToInt32(sOrgReqDocId);

                        if (String.IsNullOrEmpty(sOrgReqDocId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserId);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        private static DataSet OrgReqForUpdateDS(String mUserId, String organizationId, bool ApprovedDetail = false)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_GetNewFromOrganizationsDetails", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt64(mUserId);

                        if (String.IsNullOrEmpty(organizationId))
                            sCmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = Convert.ToInt64(organizationId);


                        sCmd.Parameters.Add("@ApprovedDetail", SqlDbType.Bit).Value = ApprovedDetail;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String GetNotificationsCount(String tokenId, String mUserid)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetNotificationsCountDS(rslt.mUserId, rslt.lang);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet GetNotificationsCountDS(String mUserId, String lang)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_NotificationsCount", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserId);


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String GetDocPath(String documentId, String mUserid)
        {
            String sPath = String.Empty;
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_GetDocFilePath", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserid))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserid);


                        if (String.IsNullOrEmpty(documentId))
                            sCmd.Parameters.Add("@documentId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@documentId", SqlDbType.VarChar).Value = documentId;


                        SqlParameter pm = new SqlParameter("@sPath", SqlDbType.NVarChar, 1000);

                        pm.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(pm);
                        sCon.Open();
                        sCmd.ExecuteNonQuery();
                        sPath = (sCmd.Parameters["@sPath"].Value == DBNull.Value) ? "" : sCmd.Parameters["@sPath"].Value.ToString();
                        sCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sPath;
        }

        private static DataSet GetNotificationsDS(String mUserId, String lang)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    //using (var sCmd = new SqlCommand("etrade.usp_MApp_GetNotificationsList", sCon))
                    using (var sCmd = new SqlCommand("etrade.usp_BRS_MApp_GetNotificationsList", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserId);


                        if (String.IsNullOrEmpty(lang))
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String GetOrgsAssociated(String tokenId, String mUserid)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetOrgsAssociatedDS(mUserid, rslt.lang);//rslt.mUserId
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet GetOrgsAssociatedDS(String mUserId, String lang)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_GetAssociatedOrganizations", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserId);

                        if (String.IsNullOrEmpty(lang))
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static string GetOrganizationDocuments(String mUserId, String OrgId, string OrgReqId)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (var sCmd = new SqlCommand("[etrade].[GetOrganizationDocs]", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = Convert.ToInt32(mUserId);

                        if (String.IsNullOrEmpty(OrgId))
                            sCmd.Parameters.Add("@OrganizationId", SqlDbType.VarChar).Value = "";
                        else
                            sCmd.Parameters.Add("@OrganizationId", SqlDbType.VarChar).Value = OrgId;
                        if (String.IsNullOrEmpty(OrgReqId))
                            sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.VarChar).Value = "";
                        else
                            sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.VarChar).Value = OrgReqId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Result rslt = new Result { Data = Ds };
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }


        internal static String ReSendOTP(String rType, String tokenId, String mUserid)
        {


            //Result rslt = GetValidUserDetails(tokenId, mUserid);
            Result rslt = new Result();

            rslt.mUserId = mUserid;

            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = ReSendOTPDS(rslt.mUserId, rType);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        internal static String ValidateContactsWithOTP(ValidateContacts Data)
        {
            Result rslt = GetValidUserDetails(Data.tokenId, Data.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = ValidateContactsWithOTPDS(Data);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet ValidateContactsWithOTPDS(ValidateContacts Data)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_ValidateContactsWithOTP", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(Data.mUserid))
                            sCmd.Parameters.Add("@mUserid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserid", SqlDbType.VarChar).Value = Data.mUserid;

                        if (String.IsNullOrEmpty(Data.Reference))
                            sCmd.Parameters.Add("@Reference", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Reference", SqlDbType.VarChar).Value = Data.Reference;
                        if (String.IsNullOrEmpty(Data.ReferenceType))
                            sCmd.Parameters.Add("@ReferenceType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ReferenceType", SqlDbType.VarChar).Value = Data.ReferenceType;
                        if (String.IsNullOrEmpty(Data.ReferenceId) || Data.ReferenceId == String.Empty)
                            sCmd.Parameters.Add("@ReferenceId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ReferenceId", SqlDbType.VarChar).Value = AES.DecryptToken(System.Web.HttpUtility.UrlDecode(Data.ReferenceId));
                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                {
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                                    if (Ds.Tables[i].Rows[0]["TableName"].ToString() == "ValidateContacts" && Data.ReferenceId != null && Data.ReferenceId == "")
                                    {
                                        String EmailKeyVal = Convert.ToString(Ds.Tables[i].Rows[0]["EmailKeyVal"]);
                                        Ds.Tables[i].Rows[0]["EmailKeyVal"] = "";
                                        String FirstName = Convert.ToString(Ds.Tables[i].Rows[0]["Firstname"]);
                                        String LastName = Convert.ToString(Ds.Tables[i].Rows[0]["Lastname"]);
                                        String Mobilekeyval = Convert.ToString(Ds.Tables[i].Rows[0]["MobileKeyVal"]);
                                        Ds.Tables[i].Rows[0]["MobileKeyVal"] = "";
                                        Ds.Tables[i].Rows[0]["EOTPreqId"] = System.Web.HttpUtility.UrlEncode(AES.EncryptToken(Ds.Tables[i].Rows[0]["EOTPreqId"].ToString()));
                                        Ds.Tables[i].Rows[0]["MOTPreqId"] = System.Web.HttpUtility.UrlEncode(AES.EncryptToken(Ds.Tables[i].Rows[0]["MOTPreqId"].ToString()));
                                        if (Data.ReferenceType != null && (Data.ReferenceType.ToUpper() == "E"))
                                        {
                                            sendEmailViaWebApi(Data.Reference, EmailKeyVal, FirstName + " " + LastName);
                                        }
                                        if (Data.ReferenceType != null && (Data.ReferenceType.ToUpper() == "M"))
                                        {
                                            String MobileMsg = "OTP: " + Mobilekeyval;
                                            SendSMSViaAPI(MobileMsg, Data.Reference);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String ForgotPwdOTP(String emailId, String mobileNo)
        {

            //using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
            //{
            //    file.WriteLine(emailId + "  "  + mobileNo);
            //}

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_BRS_MApp_ForgotPwdOTP", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(emailId))
                            sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = emailId;
                        if (String.IsNullOrEmpty(mobileNo))
                            sCmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = mobileNo;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                {

                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();

                                    if (Ds.Tables[i].Rows[0]["TableName"].ToString() == "ForgotPwdOTPResult")
                                    {
                                        String keyval = Convert.ToString(Ds.Tables[i].Rows[0]["OTPKeyVal"]);
                                        Ds.Tables[i].Rows[0]["OTPKeyVal"] = "";
                                        String eMail = Convert.ToString(Ds.Tables[i].Rows[0]["Email"]);
                                        String FirstName = Convert.ToString(Ds.Tables[i].Rows[0]["Firstname"]);
                                        String LastName = Convert.ToString(Ds.Tables[i].Rows[0]["Lastname"]);
                                        if (keyval != null && keyval != "" && emailId != null && emailId != "")
                                        {
                                            sendEmailViaWebApi(emailId, keyval, FirstName + " " + LastName,
                                                "eTrade Email Verification Code for Reset Password",
                                                "~/EmailResetPSDTempt.htm");
                                        }
                                        if (keyval != null && keyval != "" && mobileNo != null && mobileNo != "")
                                        {
                                            String MobileMsg = "OTP: " + keyval;
                                            SendSMSViaAPI(MobileMsg, mobileNo);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        internal static String ResetPwdByOTP(String otpId, String otpValue, String mUserid, String newPwd)
        {

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    //using (var sCmd = new SqlCommand("etrade.usp_MApp_ResetPwdbyOTP", sCon))
                    using (var sCmd = new SqlCommand("etrade.usp_BRS_MApp_ResetPwdbyOTP", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(otpId) || otpValue == "0")
                            sCmd.Parameters.Add("@OTPId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OTPId", SqlDbType.Int).Value = otpId;

                        if (String.IsNullOrEmpty(otpValue))
                            sCmd.Parameters.Add("@OTPValue", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OTPValue", SqlDbType.VarChar).Value = otpValue;

                        if (String.IsNullOrEmpty(mUserid))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserid;

                        if (String.IsNullOrEmpty(newPwd))
                            sCmd.Parameters.Add("@NewPwd", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@NewPwd", SqlDbType.VarChar).Value = newPwd;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        private static DataSet ReSendOTPDS(String mUserId, String rType)
        {

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    //using (var sCmd = new SqlCommand("etrade.usp_MApp_ReGenerateOTP", sCon))
                    using (var sCmd = new SqlCommand("etrade.usp_BRS_MApp_ReGenerateOTP", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;

                        if (String.IsNullOrEmpty(rType))
                            sCmd.Parameters.Add("@rType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@rType", SqlDbType.VarChar).Value = rType.ToUpper();

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                {
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                                    if (Ds.Tables[i].Rows[0]["TableName"].ToString() == "ReSendOTPStatus")
                                    {
                                        String EmailKeyVal = Convert.ToString(Ds.Tables[i].Rows[0]["EmailKeyVal"]);
                                        Ds.Tables[i].Rows[0]["EmailKeyVal"] = "";
                                        String eMail = Convert.ToString(Ds.Tables[i].Rows[0]["Email"]);
                                        String FirstName = Convert.ToString(Ds.Tables[i].Rows[0]["Firstname"]);
                                        String LastName = Convert.ToString(Ds.Tables[i].Rows[0]["Lastname"]);
                                        String Mobilekeyval = Convert.ToString(Ds.Tables[i].Rows[0]["MobileKeyVal"]);
                                        Ds.Tables[i].Rows[0]["MobileKeyVal"] = "";
                                        String MobileNumber = Convert.ToString(Ds.Tables[i].Rows[0]["MobileNo"]);

                                        if (EmailKeyVal != null && EmailKeyVal != "")
                                        {
                                            // sendEmailViaWebApi(eMail, EmailKeyVal, FirstName + " " + LastName);
                                            sendEmailViaWebApi(eMail, EmailKeyVal, FirstName + " " + LastName, "ResendOTP");
                                        }
                                        if (Mobilekeyval != null && Mobilekeyval != "" && MobileNumber != null && MobileNumber != "")
                                        {
                                            String MobileMsg = "OTP: " + Mobilekeyval;
                                            SendSMSViaAPI(MobileMsg, MobileNumber);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String GetEpaymentSearchDropdown(langParams data)
        {
            Result rslt = GetValidUserDetails(data.tokenId, data.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetEpaymentSearchDropdownDS(data.lang);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }
        private static DataSet GetEpaymentSearchDropdownDS(String lang)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_EpaymentSearchDropdown", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(lang))
                            sCmd.Parameters.Add("@Lang", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Lang", SqlDbType.VarChar).Value = lang;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                            Ds.Tables[i].Columns.Remove("TableName");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String EPaymentRequestDetails(String tokenId, String mUserid, String Language, String pagenumber, String searchCriteria, String searchDropdown)
        {

            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = EPaymentRequestDetailsDS(mUserid, Language, pagenumber, searchCriteria, searchDropdown);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }
        private static DataSet EPaymentRequestDetailsDS(String mUserId, String Language, String pagenumber, String searchCriteria, String searchDropdown)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    //using (var sCmd = new SqlCommand("etrade.usp_MApp_EPaymentRequestDetailsList", sCon))
                    using (var sCmd = new SqlCommand("etrade.usp_BRS_MApp_EPaymentRequestDetailsList", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;

                        if (String.IsNullOrEmpty(Language))
                            sCmd.Parameters.Add("@Language", SqlDbType.VarChar).Value = "en";
                        else
                            sCmd.Parameters.Add("@Language", SqlDbType.VarChar).Value = Language;

                        if (String.IsNullOrEmpty(pagenumber))
                            sCmd.Parameters.Add("@pagenumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@pagenumber", SqlDbType.VarChar).Value = pagenumber;

                        if (String.IsNullOrEmpty(searchCriteria))
                            sCmd.Parameters.Add("@searchCriteria", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@searchCriteria", SqlDbType.NVarChar).Value = searchCriteria;

                        if (String.IsNullOrEmpty(searchDropdown))
                            sCmd.Parameters.Add("@searchDropdown", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@searchDropdown", SqlDbType.VarChar).Value = searchDropdown;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String EPaymentReceiptPrintHtml(EPaymentReceipt data)
        {
            Result rslt = GetValidUserDetails(data.tokenId, data.mUserid);
            ResponseData RD = new ResponseData();
            RD.data = String.Empty;
            rslt.status = "0";
            if (rslt.status == "0")
            {
                RD.data = EPaymentReceiptPrintHtmlDS(data.OPTokenId, data.PaymentStatus);
            }
            return JsonConvert.SerializeObject(RD, Formatting.None);
        }

        private static String EPaymentReceiptPrintHtmlDS(String OPTokenId, String PaymentStatus)
        {
            OPTokenId = AES.DecryptToken(System.Web.HttpUtility.UrlDecode(OPTokenId));
            DataSet sDataResult = new DataSet("DS");
            String myString = "";

            try
            {
                using (var sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (var sCmd = new SqlCommand("[dbo].[GetOnlinePaymentDetailsWithToken]", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(OPTokenId))
                            sCmd.Parameters.Add("@TokenId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@TokenId", SqlDbType.VarChar).Value = OPTokenId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(sDataResult);

                        if (sDataResult != null && sDataResult.Tables.Count > 0)
                        {
                            String BodyHtmlFile = "~/KNETPaymentSuccessTemplate.html";
                            if (PaymentStatus.ToLower() != "success")
                                BodyHtmlFile = "~/KNETPaymentFailedTemplate.html";

                            FileStream fsreader = new FileStream(System.Web.HttpContext.Current.Server.MapPath(BodyHtmlFile), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                            StreamReader reader = new StreamReader(fsreader);

                            String readFile = reader.ReadToEnd();

                            fsreader.Dispose();
                            reader.Dispose();

                            String subString = "";
                            myString = readFile;

                            // updated the code to avoid #1 replacement for #10, #11, #12
                            myString = myString.Replace("#11", sDataResult.Tables[0].Rows[0]["OLTransId"].ToString());
                            myString = myString.Replace("#12", sDataResult.Tables[0].Rows[0]["PaymentForMail"].ToString());
                            if (PaymentStatus.ToLower() == "success")
                            {
                                myString = myString.Replace("#8", sDataResult.Tables[0].Rows[0]["PaidByTypeName"].ToString());
                                myString = myString.Replace("#10", sDataResult.Tables[0].Rows[0]["BankAuthNo"].ToString());
                                myString = myString.Replace("#7", sDataResult.Tables[0].Rows[0]["OnlineReceiptNo"].ToString());
                                subString = sDataResult.Tables[0].Rows[0]["OnlineReceiptNo"].ToString();
                            }
                            else
                                subString = sDataResult.Tables[0].Rows[0]["TempDeclNumber"].ToString();

                            myString = myString.Replace("#1", sDataResult.Tables[0].Rows[0]["PaymentTypeMail"].ToString());
                            myString = myString.Replace("#2", sDataResult.Tables[0].Rows[0]["TempDeclNumber"].ToString());
                            myString = myString.Replace("#5", sDataResult.Tables[0].Rows[0]["Amount"].ToString());
                            myString = myString.Replace("#6", sDataResult.Tables[0].Rows[0]["PaymentId"].ToString());

                            myString = myString.Replace("#9", sDataResult.Tables[0].Rows[0]["PostDate"].ToString());

                            String sImage = "~/KGACTransparentLogo.gif";
                            FileStream fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                            Byte[] imgByte = new byte[fsreader1.Length];
                            fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                            String LogInBase64 = Convert.ToBase64String(imgByte);
                            myString = myString.Replace("#KGACTransparentLogo", LogInBase64);


                            fsreader1.Dispose();

                            //fsreader1 = null;
                            imgByte = null;

                            if (PaymentStatus.ToLower() == "success")
                            {
                                sImage = "~/Success.png";
                                fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                                imgByte = new byte[fsreader1.Length];
                                fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                                LogInBase64 = Convert.ToBase64String(imgByte);
                                myString = myString.Replace("#SuccessImg", LogInBase64);
                            }
                            else
                            {
                                sImage = "~/Failed.png";
                                fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                                imgByte = new byte[fsreader1.Length];
                                fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                                LogInBase64 = Convert.ToBase64String(imgByte);
                                myString = myString.Replace("#FailedImg", LogInBase64);
                            }


                            fsreader1.Dispose();

                            //fsreader1 = null;
                            imgByte = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return myString;
        }

        internal static String ERequestPrintHtml(BrokerUpdateModel data)
        {
            //   Result rslt = GetValidUserDetails(data.tokenId, data.mUserid);
            ResponseData RD = new ResponseData();
            RD.data = String.Empty;
            //  rslt.status = "0";
            // if (rslt.status == "0")
            {
                RD.data = ERequestPrintHtmlDS(data.RequestNumber,data.mobileUserId);
            }
            return JsonConvert.SerializeObject(RD, Formatting.None);
        }

        private static String ERequestPrintHtmlDS(String RequestNUmber,int mobileUserId)
        {
            // OPTokenId = AES.DecryptToken(System.Web.HttpUtility.UrlDecode(OPTokenId));
            DataSet sDataResult = new DataSet("DS");
            String myString = "";

            try
            {
                using (var sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (var sCmd = new SqlCommand("[etrade].[GETRequestDetailsfortheRequest]", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(RequestNUmber))
                            sCmd.Parameters.Add("@ESERVICEREQUESTNUMBER", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ESERVICEREQUESTNUMBER", SqlDbType.VarChar).Value = RequestNUmber;

                        sCmd.Parameters.Add("@MobileUserid", SqlDbType.Int).Value = mobileUserId;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(sDataResult);

                        if (sDataResult != null && sDataResult.Tables.Count > 0)
                        {
                            String BodyHtmlFile = "~/RequestTemplate.html";
                            //  if (PaymentStatus.ToLower() != "success")
                            BodyHtmlFile = "~/RequestTemplate.html";

                            FileStream fsreader = new FileStream(System.Web.HttpContext.Current.Server.MapPath(BodyHtmlFile), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                            StreamReader reader = new StreamReader(fsreader);

                            String readFile = reader.ReadToEnd();


                            fsreader.Dispose();
                            reader.Dispose();

                            String subString = "";
                            myString = readFile;

                            // updated the code to avoid #1 replacement for #10, #11, #12
                            //   myString = myString.Replace("#11", sDataResult.Tables[0].Rows[0]["OLTransId"].ToString());
                            //   myString = myString.Replace("#12", sDataResult.Tables[0].Rows[0]["PaymentForMail"].ToString());
                            //if (PaymentStatus.ToLower() == "success")
                            //{
                            //    myString = myString.Replace("#8", sDataResult.Tables[0].Rows[0]["PaidByTypeName"].ToString());
                            //    myString = myString.Replace("#10", sDataResult.Tables[0].Rows[0]["BankAuthNo"].ToString());
                            //    myString = myString.Replace("#7", sDataResult.Tables[0].Rows[0]["OnlineReceiptNo"].ToString());
                            //    subString = sDataResult.Tables[0].Rows[0]["OnlineReceiptNo"].ToString();
                            //}
                            //else
                            //    subString = sDataResult.Tables[0].Rows[0]["TempDeclNumber"].ToString();

                            myString = myString.Replace("#1", sDataResult.Tables[0].Rows[0]["EServiceRequestNumber"].ToString());
                            myString = myString.Replace("#2", sDataResult.Tables[0].Rows[0]["ServiceNameEng"].ToString());
                            myString = myString.Replace("#9", sDataResult.Tables[0].Rows[0]["ServiceNameARA"].ToString());
                            myString = myString.Replace("#3", sDataResult.Tables[0].Rows[0]["DATECREATED"].ToString());

                            myString = myString.Replace("#4", sDataResult.Tables[0].Rows[0]["RequesterArabicName"].ToString());




                            myString = myString.Replace("#5", sDataResult.Tables[0].Rows[0]["EserviceRequestStatusArabic"].ToString());

                            myString = myString.Replace("#8", sDataResult.Tables[0].Rows[0]["EserviceRequestStatusEnglish"].ToString());
                            myString = myString.Replace("#7", sDataResult.Tables[0].Rows[0]["MobileNumber"].ToString());
                            myString = myString.Replace("#6", sDataResult.Tables[0].Rows[0]["Email"].ToString());


                            //     myString = myString.Replace("#9", sDataResult.Tables[0].Rows[0]["PostDate"].ToString());

                            String sImage = "~/KGACTransparentLogo.gif";
                            FileStream fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                            Byte[] imgByte = new byte[fsreader1.Length];
                            fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                            String LogInBase64 = Convert.ToBase64String(imgByte);
                            myString = myString.Replace("#KGACTransparentLogo", LogInBase64);

                            fsreader1.Dispose();


                            //fsreader1 = null;
                            imgByte = null;

                            // if (PaymentStatus.ToLower() == "success")
                            {
                                sImage = "~/Success.png";
                                fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                                imgByte = new byte[fsreader1.Length];
                                fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                                LogInBase64 = Convert.ToBase64String(imgByte);
                                myString = myString.Replace("#SuccessImg", LogInBase64);
                            }
                            //else
                            //{
                            //    sImage = "~/Failed.png";
                            //    fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                            //    imgByte = new byte[fsreader1.Length];
                            //    fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                            //    LogInBase64 = Convert.ToBase64String(imgByte);
                            //    myString = myString.Replace("#FailedImg", LogInBase64);
                            //}

                            fsreader1.Dispose();

                            //fsreader1 = null;
                            imgByte = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message.ToString(), ex.StackTrace.ToString() + "inAPI");
                throw ex;
            }

            return myString;
        }

        public static String OrgRequestPrintHtmlDS(String sOrgReqId, String mUserid)
        {
            // OPTokenId = AES.DecryptToken(System.Web.HttpUtility.UrlDecode(OPTokenId));
            DataSet ds = new DataSet("DS");
            String myString = "";

            ResponseData RD = new ResponseData();
            RD.data = String.Empty;

            try
            {
                ds = GetOrgReqAllInfo(sOrgReqId, mUserid);

                if (ds != null && ds.Tables.Count > 0)
                {
                    String BodyHtmlFile = "~/OrganizationRequestSubmit.html";

                    FileStream fsreader = new FileStream(System.Web.HttpContext.Current.Server.MapPath(BodyHtmlFile), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader reader = new StreamReader(fsreader);

                    String readFile = reader.ReadToEnd();

                    fsreader.Dispose();
                    reader.Dispose();


                    String subString = "";
                    myString = readFile;

                    //if (ds.Tables.Count > 0)
                    //{
                    //    if (ds.Tables.Count > 1)
                    //    {
                    //        if (ds.Tables[1].TableName != null)
                    //        {
                    //            jsonresult = jsonresult.ToString().Replace(ds.Tables[1].TableName.ToString(), "OrgGetBasicResult");
                    //        }
                    //    }
                    //    if (ds.Tables.Count > 2)
                    //    {
                    //        if (ds.Tables[2].TableName != null)
                    //        {
                    //            jsonresult = jsonresult.ToString().Replace(ds.Tables[2].TableName.ToString(), "OrgGetIndustrialResult");
                    //        }
                    //    }
                    //    if (ds.Tables.Count > 3)
                    //    {
                    //        if (ds.Tables[3].TableName != null)
                    //        {
                    //            jsonresult = jsonresult.ToString().Replace(ds.Tables[3].TableName.ToString(), "OrgGetImportLicenseResult");
                    //        }
                    //    }
                    //    if (ds.Tables.Count > 4)
                    //    {
                    //        if (ds.Tables[4].TableName != null)
                    //        {
                    //            jsonresult = jsonresult.ToString().Replace(ds.Tables[4].TableName.ToString(), "OrgGetCommercialLicenseResult");
                    //        }
                    //    }
                    //    if (ds.Tables.Count > 5)//added newly
                    //    {
                    //        if (ds.Tables[4].TableName != null && ds.Tables[5].TableName == "OrgAuthorizedSignatories")//added it newly  ds.Tables[4].TableName== "OrgAuthorizedSignatories"
                    //        {
                    //            jsonresult = jsonresult.ToString().Replace(ds.Tables[5].TableName.ToString(), "OrgAuthorizedSignatories");
                    //        }
                    //    }
                    //    if (ds.Tables.Count > 6)
                    //    {
                    //        if (ds.Tables[5].TableName != null && ds.Tables[6].TableName == "OrgRequestGetDocumentsResult")//added it newly  && ds.Tables[5].TableName== "OrgGetDocumentsResult") as the sp is now midified to return authrized signatories as well. Without thins change, if the there is no docs the it takes authsignatory datatable as docs table and it causes exceoption in upload page
                    //        {
                    //            jsonresult = jsonresult.ToString().Replace(ds.Tables[6].TableName.ToString(), "OrgGetDocumentsResult");
                    //        }
                    //    }
                    //}


                    // doing in revere order to avoid #1 replacement for #10, #11, #12

                    myString = myString.Replace("#31", ds.Tables[1].Rows[0]["Block"].ToString());
                    myString = myString.Replace("#32", ds.Tables[1].Rows[0]["Street"].ToString());//Country --CountryId
                    myString = myString.Replace("#30", ds.Tables[1].Rows[0]["WebPageAddress"].ToString());
                    myString = myString.Replace("#29", ds.Tables[1].Rows[0]["CountryId"].ToString());//Country --CountryId
                    myString = myString.Replace("#28", ds.Tables[1].Rows[0]["EmailId"].ToString());
                    myString = myString.Replace("#27", ds.Tables[1].Rows[0]["PostalCode"].ToString());
                    myString = myString.Replace("#26", ds.Tables[1].Rows[0]["BusiFaxNo"].ToString());
                    myString = myString.Replace("#25", ds.Tables[1].Rows[0]["State"].ToString());//State---done empty
                    myString = myString.Replace("#24", ds.Tables[1].Rows[0]["MobileNo"].ToString());
                    myString = myString.Replace("#23", ds.Tables[1].Rows[0]["City"].ToString());
                    myString = myString.Replace("#22", ds.Tables[1].Rows[0]["ResidenceNo"].ToString());
                    myString = myString.Replace("#21", ds.Tables[1].Rows[0]["Address"].ToString());
                    myString = myString.Replace("#20", ds.Tables[1].Rows[0]["BusiNo"].ToString());
                    myString = myString.Replace("#19", ds.Tables[1].Rows[0]["POBoxNo"].ToString());
                    myString = myString.Replace("#18", ds.Tables[4].Rows[0]["ExpiryDate"].ToString());
                    myString = myString.Replace("#17", ds.Tables[4].Rows[0]["IssueDate"].ToString());
                    myString = myString.Replace("#16", ds.Tables[4].Rows[0]["CommLicNo"].ToString());
                    myString = myString.Replace("#15", ds.Tables[4].Rows[0]["CommLicType"].ToString());
                    myString = myString.Replace("#14", ds.Tables[3].Rows[0]["ExpiryDate"].ToString());
                    myString = myString.Replace("#13", ds.Tables[3].Rows[0]["IssueDate"].ToString());
                    myString = myString.Replace("#12", ds.Tables[3].Rows[0]["ImpLicNo"].ToString());
                    myString = myString.Replace("#11", ds.Tables[3].Rows[0]["ImpLicType"].ToString());
                    myString = myString.Replace("#10", ds.Tables[2].Rows[0]["ExpiryDate"].ToString());
                    myString = myString.Replace("#9", ds.Tables[2].Rows[0]["IssueDate"].ToString());
                    myString = myString.Replace("#8", ds.Tables[2].Rows[0]["IndustrialLicenseNumber"].ToString());
                    myString = myString.Replace("#7", "");// ds.Tables[1].Rows[0]["PostalCode"].ToString());//Rejection reason - hardcode empty for now
                    myString = myString.Replace("#6", ds.Tables[1].Rows[0]["AuthPerson"].ToString());
                    myString = myString.Replace("#5", ds.Tables[1].Rows[0]["CivilId"].ToString());
                    myString = myString.Replace("#4", ds.Tables[1].Rows[0]["TradeLicNumber"].ToString());
                    myString = myString.Replace("#3", ds.Tables[1].Rows[0]["Description"].ToString());
                    myString = myString.Replace("#2", ds.Tables[1].Rows[0]["OrgAraName"].ToString());
                    myString = myString.Replace("#1", ds.Tables[1].Rows[0]["stateidAra"].ToString());
                    myString = myString.Replace("#0", ds.Tables[1].Rows[0]["RequestNumber"].ToString());



                    //myString = myString.Replace("#30", ds.Tables[0].Rows[0]["RequestNumber"].ToString());
                    //myString = myString.Replace("#29", ds.Tables[0].Rows[0]["OrgAraName"].ToString());
                    //myString = myString.Replace("#28", ds.Tables[0].Rows[0]["Description"].ToString());
                    //myString = myString.Replace("#27", ds.Tables[0].Rows[0]["CommLicNo"].ToString());
                    //myString = myString.Replace("#26", ds.Tables[0].Rows[0]["CommLicType"].ToString());
                    //myString = myString.Replace("#25", ds.Tables[0].Rows[0]["IssueDate"].ToString());
                    //myString = myString.Replace("#24", ds.Tables[0].Rows[0]["ExpiryDate"].ToString());
                    //myString = myString.Replace("#23", ds.Tables[0].Rows[0]["ImpLicNo"].ToString());
                    //myString = myString.Replace("#22", ds.Tables[0].Rows[0]["ImpLicType"].ToString());
                    //myString = myString.Replace("#21", ds.Tables[0].Rows[0]["IssueDate"].ToString());
                    //myString = myString.Replace("#20", ds.Tables[0].Rows[0]["ExpiryDate"].ToString());
                    //myString = myString.Replace("#19", ds.Tables[0].Rows[0]["IndustrialLicenseNumber"].ToString());
                    //myString = myString.Replace("#18", ds.Tables[0].Rows[0]["IndustrialRegistrationNumber"].ToString());
                    //myString = myString.Replace("#17", ds.Tables[0].Rows[0]["IssueDate"].ToString());
                    //myString = myString.Replace("#16", ds.Tables[0].Rows[0]["ExpiryDate"
                    //    ].ToString());
                    //myString = myString.Replace("#15", ds.Tables[0].Rows[0]["IssuanceDate"].ToString());
                    //myString = myString.Replace("#14", ds.Tables[0].Rows[0]["TradeLicNumber"].ToString());
                    //myString = myString.Replace("#13", ds.Tables[0].Rows[0]["CivilId"].ToString());
                    //myString = myString.Replace("#12", ds.Tables[0].Rows[0]["AuthPerson"].ToString());
                    //myString = myString.Replace("#11", ds.Tables[0].Rows[0]["Address"].ToString());
                    //myString = myString.Replace("#10", ds.Tables[0].Rows[0]["EmailId"].ToString());
                    //myString = myString.Replace("#9", ds.Tables[0].Rows[0]["MobileNo"].ToString());
                    //myString = myString.Replace("#8", ds.Tables[0].Rows[0]["BusiFaxNo"].ToString());
                    //myString = myString.Replace("#7", ds.Tables[0].Rows[0]["PostalCode"].ToString());
                    //myString = myString.Replace("#6", ds.Tables[0].Rows[0]["City"].ToString());
                    //myString = myString.Replace("#5", ds.Tables[0].Rows[0]["Address"].ToString());
                    //myString = myString.Replace("#4", ds.Tables[0].Rows[0]["POBoxNo"].ToString());
                    //myString = myString.Replace("#3", ds.Tables[0].Rows[0]["WebPageAddress"].ToString());
                    //myString = myString.Replace("#2", ds.Tables[0].Rows[0]["CountryId"].ToString());
                    //myString = myString.Replace("#1", ds.Tables[0].Rows[0]["ResidenceNo"].ToString());
                    //myString = myString.Replace("#0", ds.Tables[0].Rows[0]["BusiNo"].ToString());




                    String sImage = "~/KGACTransparentLogo.gif";
                    FileStream fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                    Byte[] imgByte = new byte[fsreader1.Length];
                    fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                    String LogInBase64 = Convert.ToBase64String(imgByte);
                    myString = myString.Replace("#KGACTransparentLogo", LogInBase64);


                    fsreader1.Dispose();

                    //fsreader1 = null;
                    imgByte = null;



                    //sImage = "~/Success.png";
                    //fsreader1 = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sImage), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    //imgByte = new byte[fsreader1.Length];
                    //fsreader1.Read(imgByte, 0, System.Convert.ToInt32(fsreader1.Length));
                    //LogInBase64 = Convert.ToBase64String(imgByte);
                    //myString = myString.Replace("#SuccessImg", LogInBase64);
                    //fsreader1 = null;
                    //imgByte = null;
                }

            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message.ToString(), ex.StackTrace.ToString() + "inAPI");
                //throw ex;
            }
            RD.data = myString;
            return JsonConvert.SerializeObject(RD, Formatting.None);
            //return myString;
        }


        internal static String EPaymentRequestInfo(String tokenId, String mUserid, String RequestNo, String Lang)
        {

            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = EPaymentRequestInfoDS(mUserid, RequestNo, Lang);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }
        private static DataSet EPaymentRequestInfoDS(String mUserId, String RequestNo, String Lang)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    //using (var sCmd = new SqlCommand("etrade.usp_MApp_EPaymentRequestInfo", sCon))
                    using (var sCmd = new SqlCommand("etrade.usp_BRS_MApp_EPaymentRequestInfo", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;
                        if (String.IsNullOrEmpty(RequestNo))
                            sCmd.Parameters.Add("@RequestNo", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@RequestNo", SqlDbType.VarChar).Value = RequestNo;
                        if (String.IsNullOrEmpty(Lang))
                            sCmd.Parameters.Add("@Language", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Language", SqlDbType.VarChar).Value = Lang;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                {
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                                    if (Ds.Tables[i].Rows[0]["TableName"].ToString() == "EPaymentRequestInfo")
                                    {
                                        DateTime CurrentTime = DateTime.Now;
                                        foreach (DataRow dr in Ds.Tables[i].Rows)
                                        {
                                            if (dr["OPTokenId"].ToString().Trim() != "")
                                                //dr["OPTokenId"] = EncryptToken(dr["OPTokenId"].ToString(), CurrentTime
                                                dr["OPTokenId"] = System.Web.HttpUtility.UrlEncode(AES.EncryptToken(dr["OPTokenId"].ToString()));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String VerifyOrgEmail(VerifyOTPParams data)
        {
            Result rslt = GetValidUserDetails(data.tokenId, data.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = VerifyOTPDS(data.ReferenceId, data.Email, data.Mobile, "Org", "Email");
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }
        public static String ExistingOrgRequestFortheUser(String LicenseNumber)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var connectDB = new SqlConnection(connectionStr))
                //using (var commandDB = new SqlCommand("etrade.usp_MApp_LogOnAction", connectDB))
                using (var commandDB = new SqlCommand("etrade.sp_Checkpendingrequestsforbrseservices", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {
                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@OrgTradeLicenseNumber", SqlDbType.NVarChar).Value = LicenseNumber;

                    dataAdapter.Fill(Ds);
                    for (int i = 0; i < Ds.Tables.Count; i++)
                    {
                        if (Ds.Tables[i].Columns.Contains("TableName"))
                        {
                            if (Ds.Tables[i].Rows.Count > 0)
                                Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                        }
                        else
                        {
                            Ds.Tables.RemoveAt(i);
                            i--;
                        }
                    }
                    connectDB.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // return JsonConvert.SerializeObject(Ds,Formatting.None);
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }


        internal static String VerifyOTP(String email, String mobile, String tokenId, String mUserid, string lang)
        {
            Result rslt = new Result(); //GetValidUserDetails(tokenId, mUserid);

            rslt.mUserId = mUserid;
            rslt.emailId = email;

            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = VerifyOTPDS(rslt.mUserId, email, mobile, lang);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet VerifyOTPDS(String mUserId, String emailCode, String mobileCode, String refType = "", String Mode = "", String Lang = "ar")
        {

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_BRS_MApp_VerifyOTP", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;

                        if (String.IsNullOrEmpty(emailCode))
                            sCmd.Parameters.Add("@emailCode", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@emailCode", SqlDbType.VarChar).Value = emailCode;
                        if (String.IsNullOrEmpty(mobileCode))
                            sCmd.Parameters.Add("@mobileCode", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mobileCode", SqlDbType.VarChar).Value = mobileCode;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@refId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@refId", SqlDbType.VarChar).Value = mUserId;
                        if (String.IsNullOrEmpty(refType))
                            sCmd.Parameters.Add("@refType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@refType", SqlDbType.VarChar).Value = refType;
                        if (String.IsNullOrEmpty(Mode))
                            sCmd.Parameters.Add("@mode", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mode", SqlDbType.VarChar).Value = Mode;
                        if (String.IsNullOrEmpty(Lang))
                            sCmd.Parameters.Add("@Lang", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Lang", SqlDbType.VarChar).Value = Lang;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }


        internal static String UpdateUserDetails(User userData)
        {
            //Result rslt = GetValidUserDetails(userData.tokenId, userData.mUserid);

            Result rslt = new Result();
            rslt.mUserId = userData.mUserid;

            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = UpdateUserDetailsDS(userData);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        //internal static DataSet UpdateUserDetailsDS(User userData)
        //{
        //    connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    DataSet Ds = new DataSet();

        //    try
        //    {
        //        using (var sCon = new SqlConnection(connectionStr))
        //        {
        //            using (var sCmd = new SqlCommand("etrade.usp_MApp_UpdateUserDetails", sCon))
        //            {
        //                sCmd.CommandType = CommandType.StoredProcedure;

        //                //if (String.IsNullOrEmpty(userData.FirstName))
        //                //    sCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                //else
        //                //    sCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = userData.FirstName;

        //                //if (String.IsNullOrEmpty(userData.LastName))
        //                //    sCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                //else
        //                //    sCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = userData.LastName;

        //                //if (String.IsNullOrEmpty(userData.CivilId))
        //                //    sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = DBNull.Value;
        //                //else
        //                //    sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = userData.CivilId;
        //                if (String.IsNullOrEmpty(userData.CivilId))
        //                    sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = userData.CivilId;
        //                if (String.IsNullOrEmpty(userData.LicenceNumber))
        //                    sCmd.Parameters.Add("@LicenseNumber", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@LicenseNumber", SqlDbType.VarChar).Value = userData.LicenceNumber;


        //                if (String.IsNullOrEmpty(userData.MobileTelNumber))
        //                    sCmd.Parameters.Add("@MobileTelNumber", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@MobileTelNumber", SqlDbType.VarChar).Value = userData.MobileTelNumber;


        //                //if (String.IsNullOrEmpty(userData.EmailId))
        //                //    sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = DBNull.Value;
        //                //else
        //                //    sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = userData.EmailId;

        //                sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = userData.mUserid;

        //                //SqlParameter pm = new SqlParameter("@EmailKeyVal", SqlDbType.VarChar, 10);
        //                //pm.Direction = ParameterDirection.Output;
        //                //sCmd.Parameters.Add(pm);
        //                //SqlParameter paramMobileKeyVal = new SqlParameter("@MobileKeyVal", SqlDbType.VarChar, 10);
        //                //paramMobileKeyVal.Direction = ParameterDirection.Output;
        //                //sCmd.Parameters.Add(paramMobileKeyVal);

        //                SqlDataAdapter da = new SqlDataAdapter(sCmd);
        //                da.Fill(Ds);

        //                //String EmailKeyVal = sCmd.Parameters["@EmailKeyVal"].Value.ToString();
        //                //String MobileKeyVal = sCmd.Parameters["@MobileKeyVal"].Value.ToString();

        //                //if (EmailKeyVal != null && EmailKeyVal != "")
        //                //{
        //                //    sendEmailViaWebApi(userData.EmailId, EmailKeyVal, userData.FirstName + " " + userData.LastName);
        //                //}

        //                //if (MobileKeyVal != null && MobileKeyVal != "" && userData.MobileTelNumber != null && userData.MobileTelNumber != "")
        //                //{
        //                //    String MobileMsg = "Etrade OTP: " + MobileKeyVal;
        //                //    SendSMSViaAPI(MobileMsg, userData.MobileTelNumber);
        //                //}


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




        internal static DataSet UpdateUserDetailsDS(User userData)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();

            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_UpdateUserDetails", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        //if (String.IsNullOrEmpty(userData.FirstName))
                        //    sCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = userData.FirstName;

                        //if (String.IsNullOrEmpty(userData.LastName))
                        //    sCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = userData.LastName;

                        if (String.IsNullOrEmpty(userData.CivilId))
                            sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = userData.CivilId;
                        if (String.IsNullOrEmpty(userData.LicenceNumber))
                            sCmd.Parameters.Add("@LicenseNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@LicenseNumber", SqlDbType.VarChar).Value = userData.LicenceNumber;


                        if (String.IsNullOrEmpty(userData.MobileTelNumber))
                            sCmd.Parameters.Add("@MobileTelNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MobileTelNumber", SqlDbType.VarChar).Value = userData.MobileTelNumber;



                        if (String.IsNullOrEmpty(userData.Themes))
                            sCmd.Parameters.Add("@Themes", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Themes", SqlDbType.VarChar).Value = userData.Themes;
                        //if (String.IsNullOrEmpty(userData.EmailId))
                        //    sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = userData.EmailId;

                        sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = userData.mUserid;

                        //SqlParameter pm = new SqlParameter("@EmailKeyVal", SqlDbType.VarChar, 10);
                        //pm.Direction = ParameterDirection.Output;
                        //sCmd.Parameters.Add(pm);
                        //SqlParameter paramMobileKeyVal = new SqlParameter("@MobileKeyVal", SqlDbType.VarChar, 10);
                        //paramMobileKeyVal.Direction = ParameterDirection.Output;
                        //sCmd.Parameters.Add(paramMobileKeyVal);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        //String EmailKeyVal = sCmd.Parameters["@EmailKeyVal"].Value.ToString();
                        //String MobileKeyVal = sCmd.Parameters["@MobileKeyVal"].Value.ToString();

                        //if (EmailKeyVal != null && EmailKeyVal != "")
                        //{
                        //    sendEmailViaWebApi(userData.EmailId, EmailKeyVal, userData.FirstName + " " + userData.LastName);
                        //}

                        //if (MobileKeyVal != null && MobileKeyVal != "" && userData.MobileTelNumber != null && userData.MobileTelNumber != "")
                        //{
                        //    String MobileMsg = "Etrade OTP: " + MobileKeyVal;
                        //    SendSMSViaAPI(MobileMsg, userData.MobileTelNumber);
                        //}


                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }


        internal static String GETParentUserActiveServices(int ParentID, string Action = "True", string ServicePreference = "")
        {

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GETParentUserActiveServices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = ParentID;
                        sCmd.Parameters.Add("@Preference", SqlDbType.VarChar).Value = ServicePreference;
                        if (Action == "False")
                        {
                            sCmd.Parameters.Add("@ASSOCPURPOSE", SqlDbType.Bit).Value = false;

                        }
                        else
                        {
                            sCmd.Parameters.Add("@ASSOCPURPOSE", SqlDbType.Bit).Value = true;
                        }

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        //for (int i = 0; i < Ds.Tables.Count; i++)
                        //{
                        //    if (Ds.Tables[i].Columns.Contains("TableName"))
                        //    {
                        //        if (Ds.Tables[i].Rows.Count > 0)
                        //            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                        //    }
                        //    else
                        //    {
                        //        Ds.Tables.RemoveAt(i);
                        //        i--;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        internal static String GovernorateDetails(int GovernorateId)
        {

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetGovernorateDetails", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@GovernorateId", SqlDbType.Int).Value = GovernorateId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                return JsonConvert.SerializeObject(Ds, Formatting.None);
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static String GetGovernorates()
        {

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetGovernorates", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                return JsonConvert.SerializeObject(Ds, Formatting.None);
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        internal static String GetBrokerSubOrdinateDetails(BrokerSubOrdinateReq BrokerSubOrdinateReq)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GETSubBrokerDetailsForEservices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@OrganizationIDS", SqlDbType.Int).Value = BrokerSubOrdinateReq.OrganizationIds;



                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        //for (int i = 0; i < Ds.Tables.Count; i++)
                        //{
                        //    if (Ds.Tables[i].Columns.Contains("TableName"))
                        //    {
                        //        if (Ds.Tables[i].Rows.Count > 0)
                        //            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                        //    }
                        //    else
                        //    {
                        //        Ds.Tables.RemoveAt(i);
                        //        i--;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }


        internal static String GETChildUsers(int ParentID)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GETChildUsers", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = ParentID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        //for (int i = 0; i < Ds.Tables.Count; i++)
                        //{
                        //    if (Ds.Tables[i].Columns.Contains("TableName"))
                        //    {
                        //        if (Ds.Tables[i].Rows.Count > 0)
                        //            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                        //    }
                        //    else
                        //    {
                        //        Ds.Tables.RemoveAt(i);
                        //        i--;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        internal static String GETUserDetail(int UserID)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("ETRADE.GETUserDetail", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        DataTable dt1 = new DataTable("GETUserDetail");
                        da.Fill(dt1);// Ds.Tables[0]);
                        Ds.Tables.Add(dt1);

                    }

                    using (var sCmd = new SqlCommand("ETRADE.GETUserAssociatedServices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        DataTable dt3 = new DataTable("GETUserAssociatedServices");
                        da.Fill(dt3);// Ds.Tables[0]);
                        Ds.Tables.Add(dt3);

                    }
                    if (Ds.Tables[0].Rows[0][7].ToString().Contains("Organization"))//Legalentity  10-08-2019
                    {
                        //This sp has issue and will not be called in prod as currently only broker services are rolling out
                        //Parent org trade license in etrade.MobileUserOrgMaps table no more exist..Its depnedency is still there in  sp[etrade].[GETUserAssociatedOrganizations]
                        using (var sCmd = new SqlCommand("ETRADE.GETUserAssociatedOrganizations", sCon))
                        {
                            sCmd.CommandType = CommandType.StoredProcedure;

                            sCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                            SqlDataAdapter da = new SqlDataAdapter(sCmd);
                            DataTable dt2 = new DataTable("GETUserAssociatedOrganizations");
                            da.Fill(dt2);// Ds.Tables[0]);
                            Ds.Tables.Add(dt2);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.StackTrace, ex.Message + " String GETUserDetail(int UserID)");
                throw ex;

            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static String ServicesAndOrgManagementFortheUser(int UserID, string ServiceID, string SubscriptionID, bool isLinked,
            int ParentUserID, string OrganizationID, string Action)
        {

            //int SubUserID = UserID;

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    if (Action == "ServiceAction")
                    {
                        using (var sCmd = new SqlCommand("ETRADE.UserServicesLinker", sCon))
                        {
                            sCmd.CommandType = CommandType.StoredProcedure;

                            sCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                            sCmd.Parameters.Add("@ServiceID", SqlDbType.VarChar).Value = ServiceID;
                            sCmd.Parameters.Add("@SubscriptionID", SqlDbType.VarChar).Value = SubscriptionID;
                            sCmd.Parameters.Add("@isLinked", SqlDbType.Bit).Value = isLinked;
                            sCmd.Parameters.Add("@ParentUserID", SqlDbType.Int).Value = ParentUserID;

                            SqlDataAdapter da = new SqlDataAdapter(sCmd);
                            DataTable dt1 = new DataTable();
                            da.Fill(dt1);// Ds.Tables[0]);
                            Ds.Tables.Add(dt1);

                        }
                    }
                    else if (Action == "OrganizationAction")
                    {
                        using (var sCmd = new SqlCommand("ETRADE.SubUserOrganizationLinker", sCon))
                        {
                            sCmd.CommandType = CommandType.StoredProcedure;

                            sCmd.Parameters.Add("@SubUserID", SqlDbType.Int).Value = UserID;
                            sCmd.Parameters.Add("@OrganizationID", SqlDbType.VarChar).Value = OrganizationID;
                            sCmd.Parameters.Add("@isLinked", SqlDbType.Bit).Value = isLinked;
                            sCmd.Parameters.Add("@ParentUserID", SqlDbType.Int).Value = ParentUserID;

                            SqlDataAdapter da = new SqlDataAdapter(sCmd);
                            DataTable dt1 = new DataTable();
                            da.Fill(dt1);// Ds.Tables[0]);
                            Ds.Tables.Add(dt1);

                        }
                    }



                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static String CanCreateOrg(int ParentID, string action, string orgid)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.isMainOrgApproved", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;


                        sCmd.Parameters.Add("@action", SqlDbType.VarChar).Value = action;
                        sCmd.Parameters.Add("@orgid", SqlDbType.VarChar).Value = orgid;
                        sCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = ParentID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }



        internal static String GETAdminUserOrganization(int ParentID)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GETAdminUserOrganization", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = ParentID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        //for (int i = 0; i < Ds.Tables.Count; i++)
                        //{
                        //    if (Ds.Tables[i].Columns.Contains("TableName"))
                        //    {
                        //        if (Ds.Tables[i].Rows.Count > 0)
                        //            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                        //    }
                        //    else
                        //    {
                        //        Ds.Tables.RemoveAt(i);
                        //        i--;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        //internal static String DeActivateChildUser(int ParentID, int ChildUser)
        internal static String DeActivateChildUser(int UserID, string Action)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.DeActivateChildUser", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        //sCmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = ParentID;
                        sCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                        sCmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = Action;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static String EncyptData(String data)
        {
            //String base64encodeddata = Convert.ToBase64String(data);
            SymmetricEncryption CgServices = ServiceFactory.GetSymmetricEncryptionInstance();
            //String encryptedteddata = CgServices.DecryptData(base64encodeddata);
            String encryptedteddata = CgServices.EncryptData(data);
            return encryptedteddata;
        }
        internal static String Encrypt(String plainText)
        {
            String PasswordHash = "P@@Sw0rd";
            String SaltKey = "S@LT&KEY";
            String VIKey = "@1B2c3D4e5F6g7H8";

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }


            return Convert.ToBase64String(cipherTextBytes);

        }
        internal static String SignUp(User userData)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            DataSet Ds = new DataSet();

            /*  if (userData.LegalEntity == "2") { Code not required as it is handled in blur event of user reg
                  MociLicensesServiceControl MOCI = new MociLicensesServiceControl();
                  if (userData.CompanyCivilId != "" && userData.CompanyCivilId != null)
                  {
                      MOCIOrgDetails MOCIOrgDetails = MOCI.GetOrgDetailswithCompanyCivilId(userData.CompanyCivilId, userData.IndustrialLicenseNumber, userData.CommercialLicenseNumber);
                      if (!MOCIOrgDetails.ValidfromMOCI)
                      {
                          DataTable DT = new DataTable();
                          DT.TableName = "SignupResult";
                          DT.Columns.Add("FirstName", typeof(string));
                          DT.Columns.Add("LastName", typeof(string));
                          DT.Columns.Add("TokenId", typeof(string));
                          DT.Columns.Add("UserId", typeof(string));
                          DT.Columns.Add("Status", typeof(string));
                          DT.Columns.Add("TableName", typeof(string));

                          if (!MOCIOrgDetails.IssueinMOCIService)
                              DT.Rows.Add(userData.FirstName, userData.LastName, userData.Token, userData.EmailId, "-801", "SignupResult");
                          else
                              DT.Rows.Add(userData.FirstName, userData.LastName, userData.Token, userData.EmailId, "-802", "SignupResult");

                          Ds.Tables.Add(DT);
                          return JsonConvert.SerializeObject(Ds, Formatting.None);
                      }
                  }
              }
              */
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_NewSignupAction", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;


                        // using (System.IO.StreamWriter file =
                        //new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
                        // {
                        //     file.WriteLine("Before ");
                        // }


                        if (String.IsNullOrEmpty(userData.LegalEntity))
                            sCmd.Parameters.Add("@LegalEntity", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@LegalEntity", SqlDbType.NVarChar).Value = userData.LegalEntity;

                        if (!userData.SubUser)
                        {
                            if (!String.IsNullOrEmpty(userData.GeneralBrokerLicenseNumber) || !String.IsNullOrEmpty(userData.TradeLicenseNumber))
                                sCmd.Parameters.Add("@LicenseNumber", SqlDbType.VarChar).Value =
                                    !String.IsNullOrEmpty(userData.GeneralBrokerLicenseNumber) ? userData.GeneralBrokerLicenseNumber : userData.TradeLicenseNumber;
                            else
                                sCmd.Parameters.Add("@LicenseNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(userData.LicenceNumber))
                                sCmd.Parameters.Add("@LicenseNumber", SqlDbType.VarChar).Value = DBNull.Value;
                            else
                                sCmd.Parameters.Add("@LicenseNumber", SqlDbType.VarChar).Value = userData.LicenceNumber;
                        }




                        sCmd.Parameters.Add("@ExistingUser", SqlDbType.Bit).Value = userData.ExistingUser;
                        sCmd.Parameters.Add("@isAdmin", SqlDbType.Bit).Value = userData.IsAdmin;



                        if (String.IsNullOrEmpty(userData.MCUserID))
                            sCmd.Parameters.Add("@MCUserName", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MCUserName", SqlDbType.VarChar).Value = userData.MCUserID;

                        if (String.IsNullOrEmpty(userData.MCPassword))
                            sCmd.Parameters.Add("@MCPassword", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MCPassword", SqlDbType.VarChar).Value = EncyptData(userData.MCPassword);

                        if (userData.ParentID == 0)
                            sCmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = userData.ParentID;

                        //if (userData.SubUser is null)
                        //    sCmd.Parameters.Add("@SubUser", SqlDbType.Bit).Value = DBNull.Value;
                        //else
                        sCmd.Parameters.Add("@SubUser", SqlDbType.Bit).Value = userData.SubUser;

                        if (String.IsNullOrEmpty(userData.SelectedServices))
                            sCmd.Parameters.Add("@SelectedServices", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@SelectedServices", SqlDbType.VarChar).Value = userData.SelectedServices;
                        if (String.IsNullOrEmpty(userData.SelectedOrganizations))
                            sCmd.Parameters.Add("@SelectedOrganizations", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@SelectedOrganizations", SqlDbType.VarChar).Value = userData.SelectedOrganizations;

                        if (String.IsNullOrEmpty(userData.FirstName))
                            sCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = userData.FirstName;

                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = userData.LastName;


                        if (String.IsNullOrEmpty(userData.MobileTelNumber))
                            sCmd.Parameters.Add("@MobileTelNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MobileTelNumber", SqlDbType.VarChar).Value = userData.MobileTelNumber;

                        if (String.IsNullOrEmpty(userData.Pass))
                            sCmd.Parameters.Add("@Pass", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Pass", SqlDbType.VarChar).Value = userData.Pass;

                        if (String.IsNullOrEmpty(userData.EmailId))
                            sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = userData.EmailId;

                        if (String.IsNullOrEmpty(userData.CivilId))
                            sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = userData.CivilId;

                        if (String.IsNullOrEmpty(userData.Language))
                            sCmd.Parameters.Add("@Language", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Language", SqlDbType.VarChar).Value = userData.Language;

                        if (String.IsNullOrEmpty(userData.Gender))
                            sCmd.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = userData.Gender;

                        if (String.IsNullOrEmpty(userData.Token))
                            sCmd.Parameters.Add("@TokenId", SqlDbType.NVarChar).Value = RandomToken(32);
                        else
                            sCmd.Parameters.Add("@TokenId", SqlDbType.VarChar).Value = userData.Token;


                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@IndustrialLicenseNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IndustrialLicenseNumber", SqlDbType.NVarChar).Value = userData.IndustrialLicenseNumber;
                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@CommercialLicenseNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CommercialLicenseNumber", SqlDbType.NVarChar).Value = userData.CommercialLicenseNumber;
                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@Governorate", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Governorate", SqlDbType.NVarChar).Value = userData.Governorate;
                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@Region", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Region", SqlDbType.NVarChar).Value = userData.Region;
                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar).Value = userData.PostalCode;
                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@Block", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Block", SqlDbType.NVarChar).Value = userData.Block;
                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@Street", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Street", SqlDbType.NVarChar).Value = userData.Street;
                        if (String.IsNullOrEmpty(userData.LastName))
                            sCmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = userData.Address;
                        if (String.IsNullOrEmpty(userData.CompanyCivilId))
                            sCmd.Parameters.Add("@CompanyCivilId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CompanyCivilId", SqlDbType.VarChar).Value = userData.CompanyCivilId;
                        if (String.IsNullOrEmpty(userData.CommercialLicenseYear))
                            sCmd.Parameters.Add("@CommercialLicenseYear", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CommercialLicenseYear", SqlDbType.NVarChar).Value = userData.CommercialLicenseYear;

                        if (String.IsNullOrEmpty(userData.importerLicenseNumber))
                            sCmd.Parameters.Add("@importerLicenseNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@importerLicenseNumber", SqlDbType.NVarChar).Value = userData.importerLicenseNumber;

                        if (String.IsNullOrEmpty(userData.CompanyName))
                            sCmd.Parameters.Add("@CompanyName", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CompanyName", SqlDbType.VarChar).Value = userData.CompanyName;

                         if (String.IsNullOrEmpty(userData.Nationality))
                            sCmd.Parameters.Add("@Nationality", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Nationality", SqlDbType.NVarChar).Value = userData.Nationality;

                        //-----------------------------

                        if (String.IsNullOrEmpty(userData.OrganizationNameEng))
                            sCmd.Parameters.Add("@OrganizationNameEng", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationNameEng", SqlDbType.NVarChar).Value = userData.OrganizationNameEng;

                        if (String.IsNullOrEmpty(userData.OrganizationNameAra))
                            sCmd.Parameters.Add("@OrganizationNameAra", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationNameAra", SqlDbType.NVarChar).Value = userData.OrganizationNameAra;

                        if (String.IsNullOrEmpty(userData.OrganizationCode))
                            sCmd.Parameters.Add("@OrganizationCode", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationCode", SqlDbType.NVarChar).Value = userData.OrganizationCode;

                        if (String.IsNullOrEmpty(userData.ClearanceFileNumber))
                            sCmd.Parameters.Add("@ClearanceFileNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ClearanceFileNumber", SqlDbType.NVarChar).Value = userData.ClearanceFileNumber;

                        //if (String.IsNullOrEmpty(userData.DROrg))
                        //    sCmd.Parameters.Add("@DROrg", SqlDbType.Bit).Value = false;
                        //else
                            sCmd.Parameters.Add("@DROrg", SqlDbType.Bit).Value = userData.DROrg;
                        //----------------------------------

                        //sCmd.Parameters.Add("@TokenId", SqlDbType.VarChar).Value = RandomToken(32); //commented this and added above - security feedback for additional user-Siraj

                        SqlParameter pm = new SqlParameter("@EmailKeyVal", SqlDbType.VarChar, 10);
                        pm.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(pm);

                        SqlParameter ParamMobileKeyVal = new SqlParameter("@MobileKeyVal", SqlDbType.VarChar, 10);
                        ParamMobileKeyVal.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(ParamMobileKeyVal);


                        //  using (System.IO.StreamWriter file =
                        //new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
                        //  {
                        //      file.WriteLine("Before Data Fill ");
                        //  }



                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        //----------------------------
                        if (Ds.Tables[0].Rows[0]["Status"].ToString()=="0"){
                            if (Convert.ToBoolean(Ds.Tables[0].Rows[0]["DROrg"].ToString()))
                            {
                                string UserId, ReqId, ReqNum , DataProfileClass;
                                UserId = Ds.Tables[0].Rows[0]["UserId"].ToString();
                                ReqId = Ds.Tables[0].Rows[0]["ReqId"].ToString();
                                ReqNum = Ds.Tables[0].Rows[0]["ReqNum"].ToString();

                                DataProfileClass = "DROrgUserRegistrationRequest";
                                try
                                {
                                    Controllers.FileController FC = new Controllers.FileController();
                                    FC.UploadSingleReqFile(UserId, ReqId, ReqNum, userData.File, DataProfileClass);
                                   //if ( FC.UploadSingleReqFile(UserId, ReqId, ReqNum, userData.File, DataProfileClass))
                                   // { 

                                   // }
                                }
                                catch
                                {

                                }
                            }
                        }
                        //----------------------


                        //using (System.IO.StreamWriter file =
                        //new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
                        //{
                        //    file.WriteLine("DS Table Count " + Ds.Tables.Count.ToString());
                        //    file.WriteLine("DS row Count " + Ds.Tables[0].Rows.Count.ToString());
                        //}


                        String EmailKeyVal = sCmd.Parameters["@EmailKeyVal"].Value.ToString();
                        String Mobilekeyval = sCmd.Parameters["@MobileKeyVal"].Value.ToString();
                        if (EmailKeyVal != null && EmailKeyVal != "")
                        {
                            sendEmailViaWebApi(userData.EmailId, EmailKeyVal, userData.FirstName + " " + userData.LastName, "UserRegistrationEmailLog");
                        }
                        if (Mobilekeyval != null && Mobilekeyval != "" && userData.MobileTelNumber != null && userData.MobileTelNumber != "")
                        {
                            String MobileMsg = "Etrade OTP: " + Mobilekeyval;
                            //SendSMSViaAPI(MobileMsg, userData.MobileTelNumber);
                        }


                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                        string UserHostAddress = HttpContext.Current.Request.UserHostAddress;
                        LogUserActivity LUA = new LogUserActivity
                        {
                            ActivityPerformed = "UserRegistrationOverAllMethodLog",
                            IPAddress = UserHostAddress,
                            LoginTime = DateTime.Now.ToString(),
                            OtherAdditionalInfo = ""
                        };

                        UpdateUserActivityDetailsDS(LUA);
                    }
                }
            }
            catch (Exception ex)
            {
                string UserHostAddress = HttpContext.Current.Request.UserHostAddress;
                LogUserActivity LUA = new LogUserActivity
                {
                    ActivityPerformed = "UserRegistrationOverAllMethodLog - Exception",
                    IPAddress = UserHostAddress,
                    LoginTime = DateTime.Now.ToString(),
                    OtherAdditionalInfo = ex.Message.ToString()
                };

                UpdateUserActivityDetailsDS(LUA);
                //using (System.IO.StreamWriter file =
                //new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
                //{
                //    file.WriteLine("signup  Exception");
                //    file.WriteLine(ex.Message);
                //    file.WriteLine(ex.Data);
                //    file.WriteLine(ex.ToString());
                //}
                //throw ex;
            }

            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }



        //internal static String SignUp(User userData)
        //{
        //    connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //    DataSet Ds = new DataSet();

        //    try
        //    {
        //        using (var sCon = new SqlConnection(connectionStr))
        //        {
        //            using (var sCmd = new SqlCommand("etrade.usp_MApp_SignupAction", sCon))
        //            {
        //                sCmd.CommandType = CommandType.StoredProcedure;

        //                if (String.IsNullOrEmpty(userData.FirstName))
        //                    sCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = userData.FirstName;

        //                if (String.IsNullOrEmpty(userData.LastName))
        //                    sCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = userData.LastName;

        //                if (String.IsNullOrEmpty(userData.MobileTelNumber))
        //                    sCmd.Parameters.Add("@MobileTelNumber", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@MobileTelNumber", SqlDbType.VarChar).Value = userData.MobileTelNumber;

        //                if (String.IsNullOrEmpty(userData.Pass))
        //                    sCmd.Parameters.Add("@Pass", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@Pass", SqlDbType.VarChar).Value = userData.Pass;

        //                if (String.IsNullOrEmpty(userData.EmailId))
        //                    sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = userData.EmailId;

        //                if (String.IsNullOrEmpty(userData.CivilId))
        //                    sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = userData.CivilId;

        //                if (String.IsNullOrEmpty(userData.Language))
        //                    sCmd.Parameters.Add("@Language", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@Language", SqlDbType.VarChar).Value = userData.Language;

        //                sCmd.Parameters.Add("@TokenId", SqlDbType.VarChar).Value = RandomToken(32);
        //                SqlParameter pm = new SqlParameter("@EmailKeyVal", SqlDbType.VarChar, 10);
        //                pm.Direction = ParameterDirection.Output;
        //                sCmd.Parameters.Add(pm);

        //                SqlParameter ParamMobileKeyVal = new SqlParameter("@MobileKeyVal", SqlDbType.VarChar, 10);
        //                ParamMobileKeyVal.Direction = ParameterDirection.Output;
        //                sCmd.Parameters.Add(ParamMobileKeyVal);


        //                SqlDataAdapter da = new SqlDataAdapter(sCmd);
        //                da.Fill(Ds);

        //                String EmailKeyVal = sCmd.Parameters["@EmailKeyVal"].Value.ToString();
        //                String Mobilekeyval = sCmd.Parameters["@MobileKeyVal"].Value.ToString();
        //                if (EmailKeyVal != null && EmailKeyVal != "")
        //                {
        //                    sendEmailViaWebApi(userData.EmailId, EmailKeyVal, userData.FirstName + " " + userData.LastName);
        //                }
        //                if (Mobilekeyval != null && Mobilekeyval != "" && userData.MobileTelNumber != null && userData.MobileTelNumber != "")
        //                {
        //                    String MobileMsg = "Etrade OTP: " + Mobilekeyval;
        //                    SendSMSViaAPI(MobileMsg, userData.MobileTelNumber);
        //                }


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
        //    return JsonConvert.SerializeObject(Ds, Formatting.None);
        //}


        #region OLD Send Email
        // private static void sendEmailViaWebApi(String emailId,
        //String emailKeyVal,
        //String Name,
        //String Subject = "Kuwait eCustoms Services Email Verification Code",
        //String BodyHtmlFile = "~/EmailTempt.htm")
        // {
        //     if (ConfigurationManager.AppSettings["EnableSendingMail"].ToString() == "Y")
        //     {
        //         try
        //         {
        //             MailMessage objeto_mail = new MailMessage();
        //             SmtpClient client = new SmtpClient();
        //             client.Port = 25;
        //             client.Host = ConfigurationManager.AppSettings["ExchangeServer"].ToString();
        //             client.Timeout = 10000;
        //             client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //             client.UseDefaultCredentials = false;
        //             client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["FromEmail"].ToString(), Crypt.DecryptData(ConfigurationManager.AppSettings["FromPassword"].ToString()));
        //             objeto_mail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"].ToString());
        //             objeto_mail.To.Add(new MailAddress(emailId, "Kuwait eCustoms Services"));
        //             objeto_mail.Subject = Subject;// "eTrade Email Verification Code";
        //             //objeto_mail.Body = "Hello " + Name + ", <br /><br />Welcome to the eTrade.<br /><br />Your eTrade EMail verification code is : <b>" + emailKeyVal + "</b><br /> <br /> <br />This is an autogenerated email. Please do not reply to this Email.<br /> <br /><br />Best Regards<br /><b>Kuwait General Administration of Customs</b></br><b>eTrade team</b>";

        //             //String sHTMLpath = System.Web.HttpContext.Current.Server.MapPath(@"EmailTempt.htm");
        //             //String sImagepath = System.Web.HttpContext.Current.Server.MapPath(@"eTrade.png");

        //             // StreamReader reader = new StreamReader(@"C:\inetpub\wwwroot\ETradeLive\ETradeAPI\ETradeAPI\EmailTempt.htm");
        //             FileStream fsreader = new FileStream(System.Web.HttpContext.Current.Server.MapPath(BodyHtmlFile), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        //             StreamReader reader = new StreamReader(fsreader);
        //             String readFile = reader.ReadToEnd();
        //             String myString = "";
        //             myString = readFile.ToString();
        //             myString = myString.Replace("#1", Name);
        //             myString = myString.Replace("#2", emailKeyVal);
        //             myString = myString.Replace("#3", emailId);
        //             reader.Dispose();
        //             fsreader.Dispose();

        //             String sMailBody = myString.ToString();
        //             objeto_mail.Body = sMailBody;

        //             //var inlineLogo = new LinkedResource(@"C:\inetpub\wwwroot\ETradeLive\ETradeAPI\ETradeAPI\eTrade.png");
        //             var inlineLogo = new LinkedResource(System.Web.HttpContext.Current.Server.MapPath("~/eTrade.png"));
        //             inlineLogo.ContentId = "DFFD1A8F-5393-4A67-9531-CBA0854B00D2";

        //             var view = AlternateView.CreateAlternateViewFromString(objeto_mail.Body, null, "text/html");
        //             view.LinkedResources.Add(inlineLogo);
        //             objeto_mail.AlternateViews.Add(view);


        //             objeto_mail.IsBodyHtml = true;
        //             client.Send(objeto_mail);

        //         }
        //         catch (Exception ex)
        //         {
        //             throw ex;
        //         }
        //     }
        // }
        #endregion OLD Send Email


        private static void sendEmailViaWebApi(string emailId,
       string emailKeyVal,
       string Name, string NotificationType = "", string ServiceNameEng = "", string ServiceNameAra = "", string Status = "",
       string Subject = "Kuwait eCustoms Services Email Verification Code",
       string BodyHtmlFile = "~/EmailTempt.htm")
        {
            string NotifTypeLog = "";
            if (NotificationType.Contains("UserRegistration"))
            {
                NotifTypeLog = "UserRegistration";
            }
            else if (NotificationType.ToUpper().Contains("RESET"))
            {
                NotifTypeLog = "ResetPassword";
            }
            else if (NotificationType.ToUpper().Contains("RESENDOTP"))
            {
                NotifTypeLog = "ResendOTP";
            }
            else
            {
                StackFrame caller = (new System.Diagnostics.StackTrace()).GetFrame(1);
                string methodName = caller.GetMethod().Name;
                NotifTypeLog = methodName + " From StackFrame ";
            }
            string UserHostAddress = HttpContext.Current.Request.UserHostAddress;
            LogUserActivity LUA = new LogUserActivity
            {
                ActivityPerformed = NotifTypeLog + " EMAIL",
                IPAddress = UserHostAddress,
                LoginTime = DateTime.Now.ToString(),
                OtherAdditionalInfo = emailId
            };
            #region test
            //string smtpAddress = "smtp.gmail.com";
            //int portNumber = 587;
            //bool enableSSL = true;
            //string emailFromAddress = "testappforall1@gmail.com"; //Sender Email Address 
            //string password = "Password@123"; //Sender Password 
            //string emailToAddress = "sruknudeen@agility.com"; //Receiver Email Address 
            //string subject = "Hello";
            //string body = "Hello, This is Email sending test using gmail.";
            #endregion test


            // Test Calls 
            //using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
            //{
            //    file.WriteLine("First Call");
            //}


            if (ConfigurationManager.AppSettings["EnableSendingMail"].ToString() == "Y")
            {
                try
                {

                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };

                    MailMessage objeto_mail = new MailMessage();
                    SmtpClient client = new SmtpClient();
                    client.Port = 25;
                    client.EnableSsl = true;
                    client.Host = ConfigurationManager.AppSettings["ExchangeServer"].ToString();
                    client.Timeout = 10000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("kagcwebsite", "kgac@123", "kgachq");// new System.Net.NetworkCredential(ConfigurationManager.AppSettings["FromEmail"].ToString(), "Aji");// Crypt.DecryptData(ConfigurationManager.AppSettings["FromPassword"].ToString()));
                    objeto_mail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"].ToString());
                    objeto_mail.To.Add(new MailAddress(emailId, "Kuwait eCustoms Services"));
                    objeto_mail.Subject = Subject;// "eTrade Email Verification Code";
                                                  //objeto_mail.Body = "Hello " + Name + ", <br /><br />Welcome to the eTrade.<br /><br />Your eTrade EMail verification code is : <b>" + emailKeyVal + "</b><br /> <br /> <br />This is an autogenerated email. Please do not reply to this Email.<br /> <br /><br />Best Regards<br /><b>Kuwait General Administration of Customs</b></br><b>eTrade team</b>";

                    //string sHTMLpath = System.Web.HttpContext.Current.Server.MapPath(@"EmailTempt.htm");
                    //string sImagepath = System.Web.HttpContext.Current.Server.MapPath(@"eTrade.png");

                    // StreamReader reader = new StreamReader(@"C:\inetpub\wwwroot\ETradeLive\ETradeAPI\ETradeAPI\EmailTempt.htm");
                    if (Status == "Submitted")// if (NotificationType == "ServiceRequestSubmitted")
                    {
                        objeto_mail.Subject = "eService Request";
                        BodyHtmlFile = "~/EServiceRequest.htm";
                    }
                    else
                    {
                        BodyHtmlFile = "~/EmailTempt.htm";
                    }

                    FileStream fsreader = new FileStream(System.Web.HttpContext.Current.Server.MapPath(BodyHtmlFile), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader reader = new StreamReader(fsreader);
                    string readFile = reader.ReadToEnd();

                    reader.Dispose();
                    fsreader.Dispose();

                    string myString = "";
                    if (Status == "Submitted")
                    {
                        myString = readFile.ToString();
                        myString = myString.Replace("#0", Name);
                        myString = myString.Replace("#1", emailKeyVal);
                        myString = myString.Replace("#2", ServiceNameEng);
                        myString = myString.Replace("#3", Status);
                        myString = myString.Replace("#4", DateTime.Now.ToString());
                    }
                    else //if(Status !="Discard")
                    {
                        myString = readFile.ToString();
                        myString = myString.Replace("#1", Name);
                        myString = myString.Replace("#2", emailKeyVal);
                        myString = myString.Replace("#3", emailId);
                    }
                    //reader.Dispose();
                    //fsreader.Dispose();

                    string sMailBody = myString.ToString();
                    objeto_mail.Body = sMailBody;

                    //var inlineLogo = new LinkedResource(@"C:\inetpub\wwwroot\ETradeLive\ETradeAPI\ETradeAPI\eTrade.png");
                    var inlineLogo = new LinkedResource(System.Web.HttpContext.Current.Server.MapPath("~/eTrade.png"));
                    inlineLogo.ContentId = "DFFD1A8F-5393-4A67-9531-CBA0854B00D2";

                    var view = AlternateView.CreateAlternateViewFromString(objeto_mail.Body, null, "text/html");
                    view.LinkedResources.Add(inlineLogo);
                    objeto_mail.AlternateViews.Add(view);


                    objeto_mail.IsBodyHtml = true;
                    client.Send(objeto_mail);

                    LUA.ActivityPerformed = LUA.ActivityPerformed + " - EMAIL SUCCESS";
                    UpdateUserActivityDetailsDS(LUA);
                    //MailMessage mail = new MailMessage();
                    //mail.From = new MailAddress(emailFromAddress);
                    //mail.To.Add(emailToAddress);
                    //mail.Subject = Subject;
                    //mail.Body = sMailBody;
                    //mail.IsBodyHtml = true;
                    //SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);
                    //var inlineLogo1 = new LinkedResource(System.Web.HttpContext.Current.Server.MapPath("~/eTrade.png"));
                    //inlineLogo1.ContentId = "DFFD1A8F-5393-4A67-9531-CBA0854B00D2";

                    //var view1 = AlternateView.CreateAlternateViewFromString(mail.Body, null, "text/html");
                    //view1.LinkedResources.Add(inlineLogo1);
                    //mail.AlternateViews.Add(view1);

                    //smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    //smtp.EnableSsl = enableSSL;
                    //smtp.Send(mail);

                }
                catch (Exception ex)
                {
                    LUA.ActivityPerformed = LUA.ActivityPerformed + " - EMAIL FAILURE";
                    LUA.OtherAdditionalInfo = LUA.OtherAdditionalInfo + " - FAILED - " + ex.Message.ToString();
                    UpdateUserActivityDetailsDS(LUA);

                    //    using (System.IO.StreamWriter file =
                    //new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
                    //    {
                    //        file.WriteLine("sendEmailViaWebApi  Exception");
                    //        file.WriteLine(ex.Message);
                    //        file.WriteLine(ex.Data);
                    //        file.WriteLine(ex.ToString());
                    //    }
                    //    //throw ex;
                }
            }
        }



        public String GetNewIntKey(String sCounterName)
        {
            Int64 counterValueStart;
            Int64 counterValueEnd;
            GetNewIntCounter(sCounterName, 1, out counterValueStart, out counterValueEnd);
            return counterValueStart.ToString();
        }

        public Int64 GetNewIntCounter(String sCounterName, Int64 seedValue, out Int64 counterValueStart, out Int64 counterValueEnd)
        {
            //  sqlHelp = new SQLHelper();

            String connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
            SqlParameter[] commandParameters = new SqlParameter[4];

            commandParameters[0] = new SqlParameter("@DataSourceName", SqlDbType.VarChar, 50);
            commandParameters[1] = new SqlParameter("@SeedValue", SqlDbType.BigInt);
            commandParameters[2] = new SqlParameter("@CounterValueStart", SqlDbType.BigInt);
            commandParameters[3] = new SqlParameter("@CounterValueEnd", SqlDbType.BigInt);
            commandParameters[2].Direction = ParameterDirection.Output;
            commandParameters[3].Direction = ParameterDirection.Output;
            commandParameters[0].Value = sCounterName;
            commandParameters[1].Value = seedValue;


            try
            {
                ExecuteNonQueryWithOutvalue("usp_MCPKCounters", commandParameters);
            }
            catch (Exception ex)
            {

                String Error = ex.ToString();
            }
            //   sqlHelp = null;
            counterValueStart = (Int64)commandParameters[2].Value;
            counterValueEnd = (Int64)commandParameters[3].Value;
            return (Int64)commandParameters[2].Value;
        }
        public int ExecuteNonQueryWithOutvalue(String spName, SqlParameter[] parameterValues)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            return ExecuteNonQueryWithOutvalue(connectionStr, spName, parameterValues);
        }
        private int ExecuteNonQueryWithOutvalue(String connectionString, String spName, SqlParameter[] parameterValues)
        {
            int result = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_MCPKCounters", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (parameterValues != null)
                        {
                            foreach (SqlParameter p in parameterValues)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }

                        connection.Open();
                        result = cmd.ExecuteNonQuery();
                        connection.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                String error = ex.ToString();
            }

            return result;
        }
        internal static string UniqueImporterLicenseCheck(String LicenseNumber, string OrgRequestId, string OrgId)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            string StatusCode = "-1";
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("ETRADE.CheckForUniqueImporterLicense", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@LicenseNumber", SqlDbType.NVarChar).Value = LicenseNumber;
                        sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = Int32.Parse(OrgRequestId);
                        sCmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = Int32.Parse(OrgId);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        StatusCode = Ds.Tables[0].Rows[0][0].ToString();

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return StatusCode;
        }

        internal static DataSet UpdateUploadDataDS(String mUserId, FileResult frdata, string OrgId, bool ImportLicenseDoc, string LicenseNumber, string IssuanceDate, string ExpiryDate, string LicenseType)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_BRS_MApp_UpdateUploadData", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;

                        if (String.IsNullOrEmpty(frdata.Name))
                            sCmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = frdata.Name;


                        if (String.IsNullOrEmpty(frdata.DocumentType))
                            sCmd.Parameters.Add("@DocumentType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DocumentType", SqlDbType.VarChar).Value = frdata.DocumentType;

                        if (String.IsNullOrEmpty(frdata.DocumentName))
                            sCmd.Parameters.Add("@DocumentName", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DocumentName", SqlDbType.NVarChar).Value = frdata.NewFileName;

                        if (String.IsNullOrEmpty(frdata.FilePath))
                            sCmd.Parameters.Add("@FilePath", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@FilePath", SqlDbType.NVarChar).Value = frdata.FilePath;

                        if (String.IsNullOrEmpty(frdata.ContentType))
                            sCmd.Parameters.Add("@ContentType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ContentType", SqlDbType.VarChar).Value = frdata.ContentType;

                        if (String.IsNullOrEmpty(frdata.OrgReqId))
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = Convert.ToInt32(frdata.OrgReqId);

                        if (String.IsNullOrEmpty(OrgId))//added newly
                            sCmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = Convert.ToInt32(OrgId);
                        sCmd.Parameters.Add("@IsImporterLic", SqlDbType.Bit).Value = ImportLicenseDoc;//added newly
                        if (String.IsNullOrEmpty(LicenseType))//added newly
                            sCmd.Parameters.Add("@LicenseType", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@LicenseType", SqlDbType.Int).Value = GetImpLicType(LicenseType);
                        if (String.IsNullOrEmpty(LicenseNumber))//added newly
                            sCmd.Parameters.Add("@ImporterLicNum", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ImporterLicNum", SqlDbType.NVarChar).Value = LicenseNumber;
                        if (String.IsNullOrEmpty(IssuanceDate))//added newly
                            sCmd.Parameters.Add("@IssuanceDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IssuanceDate", SqlDbType.DateTime).Value = DateTime.ParseExact(IssuanceDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                        if (String.IsNullOrEmpty(ExpiryDate))//added newly
                            sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DateTime.ParseExact(ExpiryDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                        if (String.IsNullOrEmpty(frdata.FileSize))
                            sCmd.Parameters.Add("@FileSize", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@FileSize", SqlDbType.VarChar).Value = frdata.FileSize;
                        if (String.IsNullOrEmpty(frdata.EserviceRequestId))
                            sCmd.Parameters.Add("@EserviceRequestId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EserviceRequestId", SqlDbType.Int).Value = Convert.ToInt32(frdata.EserviceRequestId);

                        if (String.IsNullOrEmpty(frdata.UploadedFrom))
                            sCmd.Parameters.Add("@UploadedFrom", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@UploadedFrom", SqlDbType.VarChar).Value = frdata.UploadedFrom;


                        SqlParameter pm = new SqlParameter("@NewName", SqlDbType.NVarChar, 1000);

                        pm.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(pm);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);


                        foreach (DataRow row in Ds.Tables[0].Rows)
                        {
                            foreach (DataColumn column in Ds.Tables[0].Columns)
                            {
                                String s = System.Web.HttpUtility.UrlEncode(WebApplication1.Models.CommonFunctions.CsUploadEncrypt(row["OrganizationRequestDocumentId"].ToString()));
                                row.SetField("Encryptedid", s);
                            }
                        }

                        Ds.Tables[0].AcceptChanges();

                        frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static DataSet InsertDROrgUserReqUploadData(String mUserId, FileResult frdata)//,  bool ImportLicenseDoc, string LicenseNumber, string IssuanceDate, string ExpiryDate, string LicenseType)
        {
            //string OrgId,--get in sp and other details also
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.InsertDROrgUserReqUploadData", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;

                        if (String.IsNullOrEmpty(frdata.Name))
                            sCmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = frdata.Name;


                        if (String.IsNullOrEmpty(frdata.DocumentType))
                            sCmd.Parameters.Add("@DocumentType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DocumentType", SqlDbType.VarChar).Value = frdata.DocumentType;

                        if (String.IsNullOrEmpty(frdata.DocumentName))
                            sCmd.Parameters.Add("@DocumentName", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DocumentName", SqlDbType.NVarChar).Value = frdata.NewFileName;

                        if (String.IsNullOrEmpty(frdata.FilePath))
                            sCmd.Parameters.Add("@FilePath", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@FilePath", SqlDbType.NVarChar).Value = frdata.FilePath;

                        if (String.IsNullOrEmpty(frdata.ContentType))
                            sCmd.Parameters.Add("@ContentType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ContentType", SqlDbType.VarChar).Value = frdata.ContentType;

                        if (String.IsNullOrEmpty(frdata.OrgReqId))
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = Convert.ToInt32(frdata.OrgReqId);

                        //if (String.IsNullOrEmpty(OrgId))//added newly
                        //    sCmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = Convert.ToInt32(OrgId);
                        //sCmd.Parameters.Add("@IsImporterLic", SqlDbType.Bit).Value = ImportLicenseDoc;//added newly
                        //if (String.IsNullOrEmpty(LicenseType))//added newly
                        //    sCmd.Parameters.Add("@LicenseType", SqlDbType.Int).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@LicenseType", SqlDbType.Int).Value = GetImpLicType(LicenseType);
                        //if (String.IsNullOrEmpty(LicenseNumber))//added newly
                        //    sCmd.Parameters.Add("@ImporterLicNum", SqlDbType.NVarChar).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@ImporterLicNum", SqlDbType.NVarChar).Value = LicenseNumber;
                        //if (String.IsNullOrEmpty(IssuanceDate))//added newly
                        //    sCmd.Parameters.Add("@IssuanceDate", SqlDbType.DateTime).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@IssuanceDate", SqlDbType.DateTime).Value = DateTime.ParseExact(IssuanceDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                        //if (String.IsNullOrEmpty(ExpiryDate))//added newly
                        //    sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DateTime.ParseExact(ExpiryDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                        if (String.IsNullOrEmpty(frdata.FileSize))
                            sCmd.Parameters.Add("@FileSize", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@FileSize", SqlDbType.VarChar).Value = frdata.FileSize;
                        if (String.IsNullOrEmpty(frdata.EserviceRequestId))
                            sCmd.Parameters.Add("@EserviceRequestId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EserviceRequestId", SqlDbType.Int).Value = Convert.ToInt32(frdata.EserviceRequestId);

                        if (String.IsNullOrEmpty(frdata.UploadedFrom))
                            sCmd.Parameters.Add("@UploadedFrom", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@UploadedFrom", SqlDbType.VarChar).Value = frdata.UploadedFrom;


                        SqlParameter pm = new SqlParameter("@NewName", SqlDbType.NVarChar, 1000);

                        pm.Direction = ParameterDirection.Output;
                        sCmd.Parameters.Add(pm);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);


                        foreach (DataRow row in Ds.Tables[0].Rows)
                        {
                            foreach (DataColumn column in Ds.Tables[0].Columns)
                            {
                                String s = System.Web.HttpUtility.UrlEncode(WebApplication1.Models.CommonFunctions.CsUploadEncrypt(row["OrganizationRequestDocumentId"].ToString()));
                                row.SetField("Encryptedid", s);
                            }
                        }

                        Ds.Tables[0].AcceptChanges();

                        frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }
        internal static string GetDROrgDetails(string OrganizationCode, string TradeLicenseNumber, string ClearanceFileNumber)//,  bool ImportLicenseDoc, string LicenseNumber, string IssuanceDate, string ExpiryDate, string LicenseType)
        {
            //string OrgId,--get in sp and other details also
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.VerifyDROrgDetails", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(OrganizationCode))
                            sCmd.Parameters.Add("@OrganizationCode", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationCode", SqlDbType.NVarChar).Value = OrganizationCode;
                        if (String.IsNullOrEmpty(TradeLicenseNumber))
                            sCmd.Parameters.Add("@TradeLicenseNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@TradeLicenseNumber", SqlDbType.NVarChar).Value = TradeLicenseNumber;
                        if (String.IsNullOrEmpty(ClearanceFileNumber))
                            sCmd.Parameters.Add("@ClearanceFileNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ClearanceFileNumber", SqlDbType.NVarChar).Value = ClearanceFileNumber;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static string ResubmitDROrgUserRegistrationRequest(String mUserId,HttpPostedFileBase File)//,  bool ImportLicenseDoc, string LicenseNumber, string IssuanceDate, string ExpiryDate, string LicenseType)
        {
            //string OrgId,--get in sp and other details also
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.ResubmitDROrgUserRegistrationRequest", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = mUserId;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                //----------------------------
                //if (Ds.Tables[0].Rows[0]["Status"].ToString() == "0")
                //{
                    //if (Convert.ToBoolean(Ds.Tables[0].Rows[0]["DROrg"].ToString()))
                    //{
                        string UserId, ReqId, ReqNum, DataProfileClass;
                        UserId = mUserId;// Ds.Tables[0].Rows[0]["UserId"].ToString();
                        ReqId = Ds.Tables[0].Rows[0]["EserviceRequestId"].ToString();
                        ReqNum = Ds.Tables[0].Rows[0]["ESERVICEREQUESTNUMBER"].ToString();

                        DataProfileClass = "DROrgUserRegistrationRequest";
                        try
                        {
                            Controllers.FileController FC = new Controllers.FileController();
                            FC.UploadSingleReqFile(UserId, ReqId, ReqNum, File, DataProfileClass);
                            //if ( FC.UploadSingleReqFile(UserId, ReqId, ReqNum, userData.File, DataProfileClass))
                            // { 

                            // }
                        }
                        catch
                        {

                        }
                //    }
                //}
                //----------------------
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static string CancelDROrgUserRegistration(String mUserId)//,  bool ImportLicenseDoc, string LicenseNumber, string IssuanceDate, string ExpiryDate, string LicenseType)
        {
            //string OrgId,--get in sp and other details also
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.CancelDROrgUserRegistration", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = mUserId;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static string GetDROrgUsereqDetails(String mUserId)//,  bool ImportLicenseDoc, string LicenseNumber, string IssuanceDate, string ExpiryDate, string LicenseType)
        {
            //string OrgId,--get in sp and other details also
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetDROrgUsereqDetails", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@DROrgUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DROrgUserId", SqlDbType.Int).Value = mUserId;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        #region User Validation

        public static String LogOnAction(String EmailId, String Password, String Language)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var connectDB = new SqlConnection(connectionStr))
                //using (var commandDB = new SqlCommand("etrade.usp_MApp_LogOnAction", connectDB))
                using (var commandDB = new SqlCommand("etrade.usp_BRS_MApp_LogOnAction", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {
                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@EmailId", SqlDbType.NVarChar).Value = EmailId;
                    commandDB.Parameters.Add("@Pass", SqlDbType.NVarChar).Value = Password;
                    commandDB.Parameters.Add("@Language", SqlDbType.NVarChar).Value = Language;
                    commandDB.Parameters.Add("@TokenId", SqlDbType.VarChar).Value = RandomToken(32);

                    dataAdapter.Fill(Ds);
                    for (int i = 0; i < Ds.Tables.Count; i++)
                    {
                        if (Ds.Tables[i].Columns.Contains("TableName"))
                        {
                            if (Ds.Tables[i].Rows.Count > 0)
                                Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                        }
                        else
                        {
                            Ds.Tables.RemoveAt(i);
                            i--;
                        }
                    }
                    connectDB.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // return JsonConvert.SerializeObject(Ds,Formatting.None);
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        internal static Result GetValidUserDetails(String tokenId, String mUserid)
        {

            Result rslt = new Result();
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataTable Dt = new DataTable();
            try
            {
                using (var connectDB = new SqlConnection(connectionStr))
                using (var commandDB = new SqlCommand("etrade.usp_MApp_GetValidUserDetails", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {
                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@tokenId", SqlDbType.NVarChar).Value = tokenId;
                    commandDB.Parameters.Add("@mUserid", SqlDbType.NVarChar).Value = mUserid;

                    dataAdapter.Fill(Dt);
                    if (Dt.Rows.Count > 0 && Dt.Rows[0]["TableName"].ToString() == "UserStatus")
                    {
                        rslt.mUserId = Dt.Rows[0]["mUserId"].ToString();
                        rslt.status = Dt.Rows[0]["status"].ToString();
                        rslt.personalId = Dt.Rows[0]["personalId"].ToString();
                        rslt.mC_UserId = Dt.Rows[0]["mC_UserId"].ToString();
                        rslt.emailId = Dt.Rows[0]["emailId"].ToString();
                        rslt.mobileNumber = Dt.Rows[0]["mobileNumber"].ToString();
                        rslt.lang = Dt.Rows[0]["lang"].ToString();

                    }
                    connectDB.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rslt;
        }
        #endregion User Validation


        #region Organization Requests
        internal static String ListOrgRequests(String tokenId, String mUserid)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = ListOrgRequestsDS(mUserid, rslt.lang);//rslt.mUserId
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet ListOrgRequestsDS(String mUserId, String lang)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_ListOrgRequests", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;

                        if (String.IsNullOrEmpty(lang))
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        internal static String UpdateOrgReqImpLicInfo(OrgRequestImp objOrgRequest)
        {
            Result rslt = GetValidUserDetails(objOrgRequest.tokenId, objOrgRequest.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = UpdateOrgReqImpLicInfoDS(rslt.mUserId, objOrgRequest);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet UpdateOrgReqImpLicInfoDS(String mUserId, OrgRequestImp objOrgRequest)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_UpdateImportLicInfo", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;


                        sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.Int).Value = Convert.ToInt32(objOrgRequest.OrganizationRequestId);

                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = mUserId;

                        //if (String.IsNullOrEmpty(objOrgRequest.ImpLicNo))
                        //    sCmd.Parameters.Add("@ImpLicNo", SqlDbType.VarChar).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@ImpLicNo", SqlDbType.VarChar).Value = objOrgRequest.ImpLicNo;

                        int sImpLicType = GetImpLicType(objOrgRequest.ImpLicType);
                        if (sImpLicType == 18)
                        {
                            sCmd.Parameters.Add("@ImpLicNo", SqlDbType.VarChar).Value = objOrgRequest.ImpLicNo;
                        }
                        else if (sImpLicType == 19)
                        {
                            //sCmd.Parameters.Add("@ImpLicNo", SqlDbType.VarChar).Value = objOrgRequest.ImpLicNo1 + "/" + objOrgRequest.ImpLicNo2; //Siraj -commented and added below line as the value sent is always just "/", as we dont pass ImpLicNo1 , ImpLicNo2
                            sCmd.Parameters.Add("@ImpLicNo", SqlDbType.VarChar).Value = objOrgRequest.ImpLicNo;
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(objOrgRequest.ImpLicNo))
                                sCmd.Parameters.Add("@ImpLicNo", SqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (String.IsNullOrEmpty(objOrgRequest.ImpLicType))
                            sCmd.Parameters.Add("@ImpLicType", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ImpLicType", SqlDbType.Int).Value = GetImpLicType(objOrgRequest.ImpLicType);

                        if (String.IsNullOrEmpty(objOrgRequest.IssueDate))
                            sCmd.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = DateTime.ParseExact(objOrgRequest.IssueDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.IssueDate);


                        if (String.IsNullOrEmpty(objOrgRequest.ExpiryDate))
                            sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DateTime.ParseExact(objOrgRequest.ExpiryDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.ExpiryDate);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Ds;
        }
        private static int GetImpLicType(String Name)
        {
            if (Name.ToUpper() == "PERMANENT")
            {
                return 18;
            }
            else if (Name.ToUpper() == "TEMPORARY")
                return 19;
            else
                return 0;
        }
        private static int GetCommLicType(String Name)
        {
            //Personal    3
            //Industrial  4
            //Corporation 5
            if (Name.ToUpper() == "INDUSTRIAL")
            {
                return 4;
            }
            else if (Name.ToUpper() == "CORPORATION")
            {
                return 5;
            }
            else if (Name.ToUpper() == "PERSONAL")

                return 3;
            else return 0;
        }
        internal static String UpdateOrgReqCommLicInfo(OrgRequestComm objOrgRequest)
        {
            Result rslt = GetValidUserDetails(objOrgRequest.tokenId, objOrgRequest.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = UpdateOrgReqCommLicInfoDS(rslt.mUserId, objOrgRequest);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet UpdateOrgReqCommLicInfoDS(String mUserId, OrgRequestComm objOrgRequest)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_UpdateCommLicInfo", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;


                        sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.Int).Value = Convert.ToInt32(objOrgRequest.OrganizationRequestId);

                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = mUserId;

                        if (String.IsNullOrEmpty(objOrgRequest.CommLicNo))
                            sCmd.Parameters.Add("@CommLicNo", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CommLicNo", SqlDbType.NVarChar).Value = objOrgRequest.CommLicNo;

                        if (String.IsNullOrEmpty(objOrgRequest.CommLicType))
                            sCmd.Parameters.Add("@CommLicType", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CommLicType", SqlDbType.Int).Value = GetCommLicType(objOrgRequest.CommLicType);

                        if (String.IsNullOrEmpty(objOrgRequest.CommLicSubType))
                            sCmd.Parameters.Add("@CommLicSubType", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CommLicSubType", SqlDbType.Int).Value = Convert.ToInt32(objOrgRequest.CommLicSubType);

                        if (String.IsNullOrEmpty(objOrgRequest.IssueDate))
                            sCmd.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = DateTime.ParseExact(objOrgRequest.IssueDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.IssueDate);


                        if (String.IsNullOrEmpty(objOrgRequest.ExpiryDate))
                            sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DateTime.ParseExact(objOrgRequest.ExpiryDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.ExpiryDate);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Ds;
        }
        internal static String ManageOrgAuthPerson(OrgAuthorizedSignatory OrgAuthPerson)
        {
            Result rslt = GetValidUserDetails(OrgAuthPerson.tokenId, OrgAuthPerson.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = ManageOrgAuthPersonDS(rslt.mUserId, OrgAuthPerson);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet ManageOrgAuthPersonDS(String mUserId, OrgAuthorizedSignatory OrgAuthPerson)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("ETRADE.ManageOrgRequestAuthorizedSignatories", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (OrgAuthPerson.OrganizationRequestId == 0 || OrgAuthPerson.OrganizationRequestId == null)
                            sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.BigInt).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.BigInt).Value = Convert.ToInt64(OrgAuthPerson.OrganizationRequestId);

                        if (OrgAuthPerson.OrganizationId == 0 || OrgAuthPerson.OrganizationId == null)
                            sCmd.Parameters.Add("@OrganizationId", SqlDbType.BigInt).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationId", SqlDbType.BigInt).Value = Convert.ToInt64(OrgAuthPerson.OrganizationId);

                        if (OrgAuthPerson.OrgAuthorizedSignatoryId == 0 || OrgAuthPerson.OrgAuthorizedSignatoryId == null)
                            sCmd.Parameters.Add("@OrgAuthorizedSignatoryId", SqlDbType.BigInt).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgAuthorizedSignatoryId", SqlDbType.BigInt).Value = Convert.ToInt64(OrgAuthPerson.OrgAuthorizedSignatoryId);

                        if (String.IsNullOrEmpty(OrgAuthPerson.AuthPerson))
                            sCmd.Parameters.Add("@AuthorizedPerson", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@AuthorizedPerson", SqlDbType.NVarChar).Value = (OrgAuthPerson.AuthPerson);

                        if (String.IsNullOrEmpty(OrgAuthPerson.CivilId))
                            sCmd.Parameters.Add("@CivilID", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CivilID", SqlDbType.NVarChar).Value = (OrgAuthPerson.CivilId);

                        if ((OrgAuthPerson.OrgAuthorizedSignatoryId == 0 || OrgAuthPerson.OrgAuthorizedSignatoryId == null))
                            sCmd.Parameters.Add("@NewPerson", SqlDbType.Bit).Value = true;
                        else
                            sCmd.Parameters.Add("@NewPerson", SqlDbType.Bit).Value = false;

                        if (!String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.BigInt).Value = Convert.ToInt64(mUserId);
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.BigInt).Value = DBNull.Value;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ds;
        }

        internal static String UpdateOrgReqIndLicInfo(OrgRequestInd objOrgRequest)
        {
            Result rslt = GetValidUserDetails(objOrgRequest.tokenId, objOrgRequest.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = UpdateOrgReqIndLicInfoDS(rslt.mUserId, objOrgRequest);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet UpdateOrgReqIndLicInfoDS(String mUserId, OrgRequestInd objOrgRequest)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_UpdateIndustrialInfo", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;


                        sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.Int).Value = Convert.ToInt32(objOrgRequest.OrganizationRequestId);


                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.Int).Value = mUserId;

                        if (String.IsNullOrEmpty(objOrgRequest.IndustrialLicenseNumber))
                            sCmd.Parameters.Add("@IndustrialLicenseNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IndustrialLicenseNumber", SqlDbType.VarChar).Value = objOrgRequest.IndustrialLicenseNumber;

                        if (String.IsNullOrEmpty(objOrgRequest.IndustrialRegistrationNumber))
                            sCmd.Parameters.Add("@IndustrialRegistrationNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IndustrialRegistrationNumber", SqlDbType.VarChar).Value = objOrgRequest.IndustrialRegistrationNumber;


                        if (String.IsNullOrEmpty(objOrgRequest.IssuanceDate))
                            sCmd.Parameters.Add("@IssuanceDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IssuanceDate", SqlDbType.DateTime).Value = DateTime.ParseExact(objOrgRequest.IssuanceDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.IssuanceDate);

                        if (String.IsNullOrEmpty(objOrgRequest.IssueDate))
                            sCmd.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = DateTime.ParseExact(objOrgRequest.IssueDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.IssueDate);


                        if (String.IsNullOrEmpty(objOrgRequest.ExpiryDate))
                            sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = DateTime.ParseExact(objOrgRequest.ExpiryDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));//  DateTime.Parse(objOrgRequest.ExpiryDate);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Ds;
        }

        #endregion


        #region Search MC Data

        #region Search Declaration
        internal static String DeclarationSearch(String tempDeclNumber, String tokenId, String mUserid, String lang)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetDeclarationInfoFromDatabase(tempDeclNumber, mUserid, lang);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet GetDeclarationInfoFromDatabase(String TempDeclNo, String UserID, String lang)
        {
            DataSet Ds = new DataSet();

            try
            {
                using (var connectDB = new SqlConnection(connectionStr))
                using (var commandDB = new SqlCommand("[etrade].[usp_getdeclarationtracking]", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {
                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@TmpDeclNo", SqlDbType.VarChar).Value = TempDeclNo; //"TEB/2717/DOH15";
                    commandDB.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserID;
                    if (String.IsNullOrEmpty(lang))
                        commandDB.Parameters.Add("@lang", SqlDbType.VarChar).Value = "en";
                    else
                        commandDB.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang;

                    dataAdapter.Fill(Ds);
                    connectDB.Close();
                }
                for (int i = 0; i < Ds.Tables.Count; i++)
                {
                    if (Ds.Tables[i].Columns.Contains("TableName"))
                    {
                        if (Ds.Tables[i].Rows.Count > 0)
                            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                    }
                    else
                    {
                        Ds.Tables.RemoveAt(i);
                        i--;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ds;
        }

        #endregion Search Declaration

        #region Search House Bill

        internal static String HouseBillSearch(String houseBillNumber, string DONumber, string SecurityCode, bool HBSearch, String tokenId, String mUserid, String lang)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetHouseBillInfoFromDatabase(houseBillNumber, DONumber, SecurityCode, HBSearch, mUserid, lang);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }
        private static DataSet GetHouseBillInfoFromDatabase(String HousebillNum, string DONumber, string SecurityCode, bool HBSearch, String UserID, String lang)
        {
            DataSet Ds = new DataSet();

            try
            {
                using (var connectDB = new SqlConnection(connectionStr))//"Data Source=10.10.27.201;user id=MCWeb;password=mcweb;initial catalog=MicroClearkw_SITA;Connect Timeout=300"))
                using (var commandDB = new SqlCommand("etrade.GetMCHBStatus", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {
                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@HousebillNo", SqlDbType.VarChar).Value = HousebillNum;  //"EXHB/1";

                    //newly added
                    commandDB.Parameters.Add("@HBSearch", SqlDbType.Bit).Value = HBSearch;  //"EXHB/1";
                    commandDB.Parameters.Add("@DONumber", SqlDbType.VarChar).Value = DONumber;  //"EXHB/1";
                    commandDB.Parameters.Add("@SecurityCode", SqlDbType.VarChar).Value = SecurityCode;  //"EXHB/1";

                    commandDB.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserID;
                    if (String.IsNullOrEmpty(lang))
                        commandDB.Parameters.Add("@lang", SqlDbType.VarChar).Value = "en";
                    else
                        commandDB.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang;

                    dataAdapter.Fill(Ds);
                    connectDB.Close();
                }
                for (int i = 0; i < Ds.Tables.Count; i++)
                {
                    if (Ds.Tables[i].Columns.Contains("TableName"))
                    {
                        if (Ds.Tables[i].Rows.Count > 0)
                            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                    }
                    else
                    {
                        Ds.Tables.RemoveAt(i);
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Ds;
        }

        #endregion Search House Bill

        #region Search HS Code

        internal static String HSCode(String data, String paramType, String tokenId, String mUserid)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                String _hsCode = String.Empty, _description = String.Empty;
                if (paramType == "HSCode") _hsCode = data;
                else _description = data;
                rslt.Data = GetHSCodeFromDatabase(_hsCode, _description
                    , rslt.mC_UserId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet GetHSCodeFromDatabase(String code, String desc, String UserID)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var connectDB = new SqlConnection(connectionStr))//"Data Source=10.10.27.201;user id=MCWeb;password=mcweb;initial catalog=MicroClearkw_SITA;Connect Timeout=300"))
                using (var commandDB = new SqlCommand("etrade.GetMobileHSCodeSearch", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {

                    code = code.Trim();
                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@HSCode", SqlDbType.NVarChar).Value = code;
                    commandDB.Parameters.Add("@Description", SqlDbType.NVarChar).Value = desc;
                    commandDB.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserID;

                    dataAdapter.Fill(Ds);
                    connectDB.Close();
                }
                for (int i = 0; i < Ds.Tables.Count; i++)
                {
                    if (Ds.Tables[i].Columns.Contains("TableName"))
                    {
                        if (Ds.Tables[i].Rows.Count > 0)
                            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                    }
                    else
                    {
                        Ds.Tables.RemoveAt(i);
                        i--;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }
        internal static String HSCodeTreeView(String hsCodeId, String tokenId, String mUserid)
        {

            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {


                rslt.Data = HSCodeTreeViewFromDatabase(hsCodeId, rslt.lang, rslt.mUserId);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet HSCodeTreeViewFromDatabase(String hsCodeId, String lang, String mUserId)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var connectDB = new SqlConnection(connectionStr))//"Data Source=10.10.27.201;user id=MCWeb;password=mcweb;initial catalog=MicroClearkw_SITA;Connect Timeout=300"))
                using (var commandDB = new SqlCommand("etrade.Usp_MApp_GetTariffItemDetails", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {


                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@TariffItemId", SqlDbType.NVarChar).Value = hsCodeId;
                    commandDB.Parameters.Add("@Lang", SqlDbType.VarChar).Value = lang;
                    commandDB.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;

                    dataAdapter.Fill(Ds);
                    connectDB.Close();
                }
                for (int i = 0; i < Ds.Tables.Count; i++)
                {
                    if (Ds.Tables[i].Columns.Contains("TableName"))
                    {
                        if (Ds.Tables[i].Rows.Count > 0)
                            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                    }
                    else
                    {
                        Ds.Tables.RemoveAt(i);
                        i--;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ds;
        }

        #endregion Search HS Code

        #endregion Search MC Data





        #region GetOrgRequestResultInfo
        internal static String GetOrgReqResultInfo(String sOrgReqId, String tokenId, String mUserid)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetOrgReqAllInfo(sOrgReqId, mUserid);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet GetOrgReqAllInfo(String sOrgReqId, String mUserid)
        {

            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_GetOrgRequestDetails", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.Int).Value = Convert.ToInt32(sOrgReqId);
                        sCmd.Parameters.Add("@mUserid", SqlDbType.Int).Value = Convert.ToInt32(mUserid);
                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Ds;
        }

        #endregion GetOrgRequestResultInfo

        internal static String DeleteOrgReq(OrgRequestWithId objOrgRequest)
        {
            Result rslt = GetValidUserDetails(objOrgRequest.tokenId, objOrgRequest.mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = DeleteOrgReqDS(objOrgRequest);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet DeleteOrgReqDS(OrgRequestWithId objOrgRequest)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_DeleteOrgRequest", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (objOrgRequest.mUserid == null)
                            sCmd.Parameters.Add("@mUserid", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserid", SqlDbType.Int).Value = Convert.ToInt32(objOrgRequest.mUserid);

                        if ((objOrgRequest.OrganizationRequestId == null || objOrgRequest.OrganizationRequestId == 0))
                            sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.Int).Value = Convert.ToInt32(objOrgRequest.OrganizationRequestId);


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ds;
        }


        #region CreateUpdateOrgReq
        internal static String CreateUpdateOrgReqBasicInfo(String tokenId, String mUserid, OrgRequest objOrgRequest)
        {
            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = CreateUpdateOrgReqBasicInfoDS(rslt.mUserId, objOrgRequest);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet CreateUpdateOrgReqBasicInfoDS(String mUserid, OrgRequest objOrgRequest)
        {
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_InsertUpdateOrgRequest", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if ((objOrgRequest.OrganizationRequestId == null || objOrgRequest.OrganizationRequestId == 0))
                            sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationRequestId", SqlDbType.Int).Value = Convert.ToInt32(objOrgRequest.OrganizationRequestId);

                        if ((objOrgRequest.OrganizationId == null || objOrgRequest.OrganizationId == 0))
                            sCmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = Convert.ToInt32(objOrgRequest.OrganizationId);

                        if (String.IsNullOrEmpty(objOrgRequest.OrgEngName))
                            sCmd.Parameters.Add("@OrgEngName", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgEngName", SqlDbType.NVarChar).Value = objOrgRequest.OrgEngName;

                        if (String.IsNullOrEmpty(objOrgRequest.OrgAraName))
                            sCmd.Parameters.Add("@OrgAraName", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgAraName", SqlDbType.NVarChar).Value = objOrgRequest.OrgAraName;
                        if (String.IsNullOrEmpty(objOrgRequest.Description))
                            sCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = objOrgRequest.Description;
                        if (String.IsNullOrEmpty(objOrgRequest.TradeLicNumber))
                            sCmd.Parameters.Add("@TradeLicNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@TradeLicNumber", SqlDbType.NVarChar).Value = objOrgRequest.TradeLicNumber;
                        if (String.IsNullOrEmpty(objOrgRequest.CivilId))
                            sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CivilId", SqlDbType.VarChar).Value = objOrgRequest.CivilId;
                        if (String.IsNullOrEmpty(objOrgRequest.AuthPerson))
                            sCmd.Parameters.Add("@AuthPerson", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@AuthPerson", SqlDbType.NVarChar).Value = objOrgRequest.AuthPerson;
                        if (String.IsNullOrEmpty(objOrgRequest.POBoxNo))
                            sCmd.Parameters.Add("@POBoxNo", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@POBoxNo", SqlDbType.NVarChar).Value = objOrgRequest.POBoxNo;
                        if (String.IsNullOrEmpty(objOrgRequest.Address))
                            sCmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = objOrgRequest.Address;
                        if (String.IsNullOrEmpty(objOrgRequest.City))
                            sCmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = objOrgRequest.City;
                        if (String.IsNullOrEmpty(objOrgRequest.State))
                            sCmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = objOrgRequest.State;
                        if (String.IsNullOrEmpty(objOrgRequest.PostalCode))
                            sCmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar).Value = objOrgRequest.PostalCode;
                        if (String.IsNullOrEmpty(objOrgRequest.CountryId))
                            sCmd.Parameters.Add("@CountryId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CountryId", SqlDbType.VarChar).Value = objOrgRequest.CountryId;
                        if (String.IsNullOrEmpty(objOrgRequest.BusiNo))
                            sCmd.Parameters.Add("@BusiNo", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@BusiNo", SqlDbType.NVarChar).Value = objOrgRequest.BusiNo;
                        if (String.IsNullOrEmpty(objOrgRequest.BusiFaxNo))
                            sCmd.Parameters.Add("@BusiFaxNo", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@BusiFaxNo", SqlDbType.NVarChar).Value = objOrgRequest.BusiFaxNo;
                        if (String.IsNullOrEmpty(objOrgRequest.MobileNo))
                            sCmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = objOrgRequest.MobileNo;
                        if (String.IsNullOrEmpty(objOrgRequest.ResidenceNo))
                            sCmd.Parameters.Add("@ResidenceNo", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ResidenceNo", SqlDbType.VarChar).Value = objOrgRequest.ResidenceNo;
                        if (String.IsNullOrEmpty(objOrgRequest.EmailId))
                            sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = objOrgRequest.EmailId;

                        if (String.IsNullOrEmpty(objOrgRequest.WebPageAddress))
                            sCmd.Parameters.Add("@WebPageAddress", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@WebPageAddress", SqlDbType.VarChar).Value = objOrgRequest.WebPageAddress;

                        if (objOrgRequest.isIndustrial == null)
                            sCmd.Parameters.Add("@IsIndustrial", SqlDbType.Char).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@IsIndustrial", SqlDbType.Char).Value = (bool)objOrgRequest.isIndustrial ? '1' : '0';

                        sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserid;
                        sCmd.Parameters.Add("@Operation", SqlDbType.VarChar).Value = (objOrgRequest.OrganizationRequestId == null || objOrgRequest.OrganizationRequestId == 0) ? "Insert" : "Update";


                        if (String.IsNullOrEmpty(objOrgRequest.Block))
                            sCmd.Parameters.Add("@Block", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Block", SqlDbType.NVarChar).Value = objOrgRequest.Block;

                        if (String.IsNullOrEmpty(objOrgRequest.Street))
                            sCmd.Parameters.Add("@Street", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Street", SqlDbType.NVarChar).Value = objOrgRequest.Street;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ds;
        }
        #endregion CreateUpdateOrgReq

        public static String GetTypes(String SpName, String lang, String sOrgReqId = "", string sOrgId = "")
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand(SpName, sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang;
                        if (sOrgReqId != "")
                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.VarChar).Value = sOrgReqId;
                        if (sOrgId != "")
                            sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = sOrgId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        internal static String DenyPayRequest(String tokenId, String mUserid, String requestNo, String lang)
        {

            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = DenyPayRequestDS(mUserid, requestNo, lang);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet DenyPayRequestDS(String mUserId, String RequestNo, String lang)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.usp_MApp_DenyPayRequest", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(mUserId))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = mUserId;
                        if (String.IsNullOrEmpty(RequestNo))
                            sCmd.Parameters.Add("@RequestNo", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@RequestNo", SqlDbType.VarChar).Value = RequestNo;

                        if (String.IsNullOrEmpty(lang))
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = "ar";
                        else
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ds;
        }

        #region Captha


        public static String GetCaptcha(int CaptchaId = 0)
        {
            Captcha retCaptcha = new Captcha();
            String RandomAlphaNumetic;
            if (CaptchaId == 0)
            {
                RandomAlphaNumetic = RandomCaseToken(6);
                CaptchaId = ManageETradeCaptchaData("Insert", CaptchaId, RandomAlphaNumetic);
                retCaptcha.CaptchaId = CaptchaId;
                retCaptcha.CaptchaImage = GenerateCaptchaImage(RandomAlphaNumetic, CaptchaId);
            }
            else
            {
                RandomAlphaNumetic = RandomCaseToken(6);
                ManageETradeCaptchaData("Update", CaptchaId, RandomAlphaNumetic);
                retCaptcha.CaptchaId = CaptchaId;
                retCaptcha.CaptchaImage = GenerateCaptchaImage(RandomAlphaNumetic, CaptchaId);
            }

            return JsonConvert.SerializeObject(retCaptcha, Formatting.None);
        }
        public static bool IsCapthaValid(int CaptchaId, String RandomAlphaNumetic)
        {
            //int CaptchaId = sData.uCaptchaNo;
            //int RandomNumber = sData.uCaptcha;
            if (ManageETradeCaptchaData("CompareCaptcha", CaptchaId, RandomAlphaNumetic) != -1)
            {                //ManageETradeCaptchaData("Delete", CaptchaId, RandomNumber);
                return true;
            }
            return false;
        }

        //private static int GetRandomNumber()
        //{
        //    int Captcha = 0;
        //    Random rnd = new Random();
        //    Captcha = rnd.Next(100000, 999999);
        //    return Captcha;
        //}

        private static int ManageETradeCaptchaData(String ActionType, int sCaptchaId, String RandomAlphaNumeric)
        {
            String ConString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(ConString);
            int CaptchaId = 0;
            try
            {
                SqlCommand sqlCmd = new SqlCommand("etrade.usp_MApp_ManageETradeCaptchaData", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                SqlParameter paramsCaptchaId = sqlCmd.Parameters.Add("@sCaptchaId", SqlDbType.Int);
                paramsCaptchaId.Value = sCaptchaId;
                SqlParameter paramCaptcha = sqlCmd.Parameters.Add("@sCaptcha", SqlDbType.VarChar);
                paramCaptcha.Value = RandomAlphaNumeric;
                SqlParameter paramActionType = sqlCmd.Parameters.Add("@sActionType", SqlDbType.NVarChar, 50);
                paramActionType.Value = ActionType;

                SqlParameter paramCaptchaId = sqlCmd.Parameters.Add("@CaptchaId", SqlDbType.Int);
                paramCaptchaId.Direction = ParameterDirection.Output;
                paramCaptchaId.Value = CaptchaId;

                sqlCon.Open();
                sqlCmd.ExecuteNonQuery();
                CaptchaId = (int)(sqlCmd.Parameters["@CaptchaId"].Value);
                sqlCon.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                if (sqlCon != null && sqlCon.State == ConnectionState.Open)
                    sqlCon.Close();
            }
            return CaptchaId;
        }

        private static String GenerateCaptchaImage(String RandomAlphaNumeric, int CaptchaId)
        {
            String CaptchaImageString = String.Empty;
            Bitmap CaptchaImage;
            if (CaptchaId == 0)
                return CaptchaImageString;
            //CaptchaImage = ConvertTextToImage(RandomAlphaNumeric, "Bookman Old Style", 18, Color.WhiteSmoke, Color.Black, 200, 50);
            CaptchaImage = GenerateImage(RandomAlphaNumeric, "Bookman Old Style", 18, Color.WhiteSmoke, Color.Black, 200, 50);
            CaptchaImageString = "data:image/png;base64, " + ImageToBase64String(CaptchaImage);
            return CaptchaImageString;
        }

        private static String ImageToBase64String(Bitmap img)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static Bitmap ConvertTextToImage(String txt, String fontname, int fontsize, Color bgcolor, Color fcolor, int width, int Height)
        {
            Bitmap bmp = new Bitmap(width, Height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                Font font = new Font(fontname, fontsize);
                graphics.FillRectangle(new SolidBrush(bgcolor), 0, 0, bmp.Width, bmp.Height);
                graphics.DrawString(txt, font, new SolidBrush(fcolor), width / 4, Height / 4);
                graphics.Flush();
                font.Dispose();
                graphics.Dispose();
            }
            return bmp;
        }

        // ====================================================================
        // Creates the bitmap image.
        // ====================================================================
        private static Bitmap GenerateImage(String text, String familyName, int fontsize, Color bgcolor, Color fcolor, int width, int height)
        {
            // Create a new 32-bit bitmap image.
            Bitmap bitmap = new Bitmap(
             width,
             height,
              PixelFormat.Format32bppArgb);

            // Create a graphics object for drawing.
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, width, height);

            // Fill in the background.
            HatchBrush hatchBrush = new HatchBrush(
              HatchStyle.SmallConfetti,
              Color.LightGray,
              Color.White);
            g.FillRectangle(hatchBrush, rect);

            // Set up the text font.
            SizeF size;
            float fontSize = rect.Height + 1;
            Font font;
            // Adjust the font size until the text fits within the image.
            do
            {
                fontSize--;
                font = new Font(
                 familyName,
                  fontSize,
                  FontStyle.Bold);
                size = g.MeasureString(text, font);
            } while (size.Width > rect.Width);

            // Set up the text format.
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            // Create a path using the text and warp it randomly.
            GraphicsPath path = new GraphicsPath();
            path.AddString(
             text,
              font.FontFamily,
              (int)font.Style,
              font.Size, rect,
              format);
            float v = 4F;
            PointF[] points =
            {
    new PointF(
     random.Next(rect.Width) / v,
     random.Next(rect.Height) / v),
    new PointF(
      rect.Width -random.Next(rect.Width) / v,
     random.Next(rect.Height) / v),
    new PointF(
     random.Next(rect.Width) / v,
      rect.Height -random.Next(rect.Height) / v),
    new PointF(
      rect.Width -random.Next(rect.Width) / v,
      rect.Height -random.Next(rect.Height) / v)
  };
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

            // Draw the text.
            hatchBrush = new HatchBrush(
              HatchStyle.LargeConfetti,
              Color.Black,
              Color.Black);
            g.FillPath(hatchBrush, path);

            // Add some random noise.
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = random.Next(rect.Width);
                int y = random.Next(rect.Height);
                int w = random.Next(m / 50);
                int h = random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }

            // Clean up.
            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();

            // Set the image.
            return bitmap;
        }
        #endregion

        #region brokertransfermode
        internal static String GetBrokerTypesList(BrokerUpdateModel objtransfermodel)
        {

            Result rslt = GetValidUserDetails(objtransfermodel.tokenId, objtransfermodel.Userid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = GetBrokerTypesListDS(objtransfermodel);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static DataSet GetBrokerTypesListDS(BrokerUpdateModel objtransfermodel)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetBrokerTypes_forBrsEservices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(objtransfermodel.Userid))
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mUserId", SqlDbType.VarChar).Value = objtransfermodel.Userid;


                        if (String.IsNullOrEmpty(objtransfermodel.lang))
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = "ar";
                        else
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = objtransfermodel.lang;

                        if (String.IsNullOrEmpty(objtransfermodel.CivilIdNo))
                            sCmd.Parameters.Add("@civilidno", SqlDbType.BigInt).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@civilidno", SqlDbType.BigInt).Value = Convert.ToInt64(objtransfermodel.CivilIdNo);

                        if (String.IsNullOrEmpty(objtransfermodel.BrokerType))
                            sCmd.Parameters.Add("@BrokerType", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@BrokerType", SqlDbType.Int).Value = Convert.ToInt32(objtransfermodel.BrokerType);

                        if (String.IsNullOrEmpty(objtransfermodel.BrokerLicenseNumber))
                            sCmd.Parameters.Add("@BrokerLicenseNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@BrokerLicenseNumber", SqlDbType.VarChar).Value = objtransfermodel.BrokerLicenseNumber;

                        if (String.IsNullOrEmpty(objtransfermodel.TypeOfAction))
                            sCmd.Parameters.Add("@typeofAction", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@typeofAction", SqlDbType.VarChar).Value = objtransfermodel.TypeOfAction;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ds;
        }
        #endregion

        #region brokerUpdateDetails

        public static String GetBrokerDetails(int organizationid, BrokerUpdateModel frdata)
        {

              

            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_getBrokerDetailsForEservices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(frdata.Userid))
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = frdata.Userid;
                        if (String.IsNullOrEmpty(organizationid.ToString()))
                            sCmd.Parameters.Add("@orgid", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@orgid", SqlDbType.Int).Value = organizationid;

                        if (String.IsNullOrEmpty(frdata.RequestNumber))
                            sCmd.Parameters.Add("@requestNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@requestNumber", SqlDbType.VarChar).Value = frdata.RequestNumber;
                        if (String.IsNullOrEmpty(frdata.Referenceprofile))
                            sCmd.Parameters.Add("@ReferenceProfile", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ReferenceProfile", SqlDbType.VarChar).Value = frdata.Referenceprofile;
                        if (String.IsNullOrEmpty(frdata.lang))
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = frdata.lang;
                        // added for generic  service id 
                        if (String.IsNullOrEmpty(frdata.Serviceid.ToString()))
                            sCmd.Parameters.Add("@Serviceid", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Serviceid", SqlDbType.Int).Value = frdata.Serviceid;

                        if (String.IsNullOrEmpty(frdata.mobileUserId.ToString()))
                            sCmd.Parameters.Add("@mobileUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mobileUserId", SqlDbType.Int).Value = frdata.mobileUserId;

                        if (String.IsNullOrEmpty(frdata.requestForMobileUserId.ToString()))
                            sCmd.Parameters.Add("@RequestedForMobileUserid", SqlDbType.BigInt).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@RequestedForMobileUserid", SqlDbType.BigInt).Value = frdata.requestForMobileUserId;


                        String examRequestReferenceProfile = ConfigurationManager.AppSettings["ExamRequestReference"].ToString();

                        if (frdata.Referenceprofile == examRequestReferenceProfile)
                            sCmd.Parameters.Add("@ExamReq", SqlDbType.Bit).Value = true;
                        else
                            sCmd.Parameters.Add("@ExamReq", SqlDbType.Bit).Value = false;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    //Notify(Ds);
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message + ex.StackTrace + organizationid, "GetBrokerDetails");
                throw ex;

            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        internal static String UpdateBrokerUserDetails(BrokerUpdateModel BrokerData)
        {
            Result rslt = new Result();
            //   Result rslt = GetValidUserDetails("29UJIHFVMPT5ARMAJC9J4WM1AJE43K25", BrokerData.Userid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.Data = UpdateBrokerUserDetailsDS(BrokerData);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        internal static DataSet UpdateBrokerUserDetailsDS(BrokerUpdateModel BrokerData)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;





            DataSet Ds = new DataSet();
            StringBuilder sb;
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.sp_UpdateUserDetailsForEServices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (String.IsNullOrEmpty(BrokerData.Userid))
                            sCmd.Parameters.Add("@Userid", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Userid", SqlDbType.NVarChar).Value = BrokerData.Userid;

                        //if (String.IsNullOrEmpty(BrokerData.CivilIdExpirydate))
                        //    sCmd.Parameters.Add("@CivilIdExpirydate", SqlDbType.NVarChar).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@CivilIdExpirydate", SqlDbType.NVarChar).Value = BrokerData.CivilIdExpirydate;

                        //if (String.IsNullOrEmpty(BrokerData.TradeLicenseExpiryDate))
                        //    sCmd.Parameters.Add("@TradeLicenseExpiryDate", SqlDbType.NVarChar).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@TradeLicenseExpiryDate", SqlDbType.NVarChar).Value = BrokerData.TradeLicenseExpiryDate;

                        //if (String.IsNullOrEmpty(BrokerData.PassportExpirydate))
                        //    sCmd.Parameters.Add("@PassportExpirydate", SqlDbType.NVarChar).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@PassportExpirydate", SqlDbType.NVarChar).Value = BrokerData.PassportExpirydate;

                        if (String.IsNullOrEmpty(BrokerData.CivilIdExpirydate))
                            sCmd.Parameters.Add("@CivilIdExpirydate", SqlDbType.DateTime).Value = DBNull.Value;
                        else

                            sCmd.Parameters.Add("@CivilIdExpirydate", SqlDbType.DateTime).Value = DateTime.ParseExact(BrokerData.CivilIdExpirydate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.IssueDate);


                        if (String.IsNullOrEmpty(BrokerData.TradeLicenseExpiryDate))
                            sCmd.Parameters.Add("@TradeLicenseExpiryDate", SqlDbType.DateTime).Value = DBNull.Value;
                        else

                            sCmd.Parameters.Add("@TradeLicenseExpiryDate", SqlDbType.DateTime).Value = DateTime.ParseExact(BrokerData.TradeLicenseExpiryDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.IssueDate);


                        if (String.IsNullOrEmpty(BrokerData.PassportExpirydate))
                            sCmd.Parameters.Add("@PassportExpirydate", SqlDbType.DateTime).Value = DBNull.Value;
                        else

                            sCmd.Parameters.Add("@PassportExpirydate", SqlDbType.DateTime).Value = DateTime.ParseExact(BrokerData.PassportExpirydate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));// DateTime.Parse(objOrgRequest.IssueDate);



                        if (String.IsNullOrEmpty(BrokerData.passportNo))
                            sCmd.Parameters.Add("@PassportNo", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@PassportNo", SqlDbType.NVarChar).Value = BrokerData.passportNo;

                        if (String.IsNullOrEmpty(BrokerData.MobileNumber))
                            sCmd.Parameters.Add("@MobileNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MobileNumber", SqlDbType.NVarChar).Value = BrokerData.MobileNumber;

                        if (String.IsNullOrEmpty(BrokerData.MailAddress))
                            sCmd.Parameters.Add("@MailAddress", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MailAddress", SqlDbType.NVarChar).Value = BrokerData.MailAddress;

                        if (String.IsNullOrEmpty(BrokerData.officialAddress))
                            sCmd.Parameters.Add("@OfficialAddress", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OfficialAddress", SqlDbType.NVarChar).Value = BrokerData.officialAddress;

                        if (String.IsNullOrEmpty(BrokerData.Nationality))
                            sCmd.Parameters.Add("@Nationality", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Nationality", SqlDbType.NVarChar).Value = BrokerData.Nationality;

                        sCmd.Parameters.Add("@DateCreated", SqlDbType.NVarChar).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                        if (String.IsNullOrEmpty(BrokerData.RequestNumber))
                            sCmd.Parameters.Add("@EServiceRequestNumber", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EServiceRequestNumber", SqlDbType.NVarChar).Value = BrokerData.RequestNumber;

                        if (String.IsNullOrEmpty(BrokerData.SelectedOrgidForIssuance))
                            sCmd.Parameters.Add("@SelectedOrgidForIssuance", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@SelectedOrgidForIssuance", SqlDbType.NVarChar).Value = BrokerData.SelectedOrgidForIssuance;

                        // for printing services 
                        if (String.IsNullOrEmpty(BrokerData.fromBusiness))
                            sCmd.Parameters.Add("@fromBusiness", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@fromBusiness", SqlDbType.NVarChar).Value = BrokerData.fromBusiness;

                        if (String.IsNullOrEmpty(BrokerData.ChangeJobTitleFrom))
                            sCmd.Parameters.Add("@ChangeJobTitleFrom", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ChangeJobTitleFrom", SqlDbType.NVarChar).Value = BrokerData.ChangeJobTitleFrom;

                        if (String.IsNullOrEmpty(BrokerData.ChangeJobTitleTo))
                            sCmd.Parameters.Add("@ChangeJobTitleTo", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ChangeJobTitleTo", SqlDbType.VarChar).Value = BrokerData.ChangeJobTitleTo;

                     


        //SqlParameter pm = new SqlParameter("@EmailKeyVal", SqlDbType.VarChar, 10);
        //pm.Direction = ParameterDirection.Output;
        //sCmd.Parameters.Add(pm);
        //SqlParameter paramMobileKeyVal = new SqlParameter("@MobileKeyVal", SqlDbType.VarChar, 10);
        //paramMobileKeyVal.Direction = ParameterDirection.Output;
        //sCmd.Parameters.Add(paramMobileKeyVal);

        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        //String EmailKeyVal = sCmd.Parameters["@EmailKeyVal"].Value.ToString();
                        //String MobileKeyVal = sCmd.Parameters["@MobileKeyVal"].Value.ToString();

                        //if (EmailKeyVal != null && EmailKeyVal != "")
                        //{
                        //    sendEmailViaWebApi(userData.EmailId, EmailKeyVal, userData.FirstName + " " + userData.LastName);
                        //}

                        //if (MobileKeyVal != null && MobileKeyVal != "" && userData.MobileTelNumber != null && userData.MobileTelNumber != "")
                        //{
                        //    String MobileMsg = "Etrade OTP: " + MobileKeyVal;
                        //    SendSMSViaAPI(MobileMsg, userData.MobileTelNumber);
                        //}


                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                Ds.Tables["UpdateUserDetails"].Columns.Add("RedirectUrl");
                SqlConnection con = new SqlConnection(connectionStr);
                con.Open();
                SqlCommand cmd = new SqlCommand("etrade.Sp_checkpaymentsForBrsEservices", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EServiceRequestNumber", BrokerData.RequestNumber.ToString());

                cmd.Parameters.Add("@Result", SqlDbType.Char, 500);

                cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@AmountToBepaid", SqlDbType.Char, 500);

                cmd.Parameters["@AmountToBepaid"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                String CheckInsertResult = (String)cmd.Parameters["@Result"].Value;
                String AmountToBepaid = (String)cmd.Parameters["@AmountToBepaid"].Value;

                if (CheckInsertResult.Trim() != "FreeService" && CheckInsertResult.Trim() != "Paid")
                {

                    sb = new StringBuilder();
                    DateTime time = DateTime.Now;
                    String ReceiptDate = time.ToString("dd-MM-yyyy");
                    sb.Append("ReferenceType=" + BrokerData.Referenceprofile + "~PaymentMode=" + BrokerData.paymentMode + "~");
                    sb.Append("PaymentTypeId=" + BrokerData.PaymentTypeId + "~ReceiptAmount=" + AmountToBepaid + "~");
                    sb.Append("ReceiptDate=" + ReceiptDate + "~PaidFor=" + BrokerData.paidfor + "");
                    sb.Append("~PaidBy=" + BrokerData.paidby + "~PaidByType=" + BrokerData.PaidByType + "~");
                    sb.Append("ReferenceId=" + BrokerData.Eservicerequestid + "~PaidByType=B~");
                    sb.Append("UserId=" + BrokerData.BrokerArabicName.ToString() + "~OwnerLocId=" + BrokerData.ownerlocid + "");
                    sb.Append("~lang=" + BrokerData.lang.ToString() + "~OwnerOrgId=" + BrokerData.ownerorgid + "");
                    sb.Append("~ReferenceNumber=" + BrokerData.RequestNumber.ToString() + "~PaymentFor=1~");
                    sb.Append("eServiceUserEmailId=" + BrokerData.eServiceUserEmailId);
                    //String  = Regex.Replace(sb.ToString(), @"s", "");
                    String trimString = Regex.Replace(sb.ToString(), @"", "").Replace(" ", "");
                    con = new SqlConnection(connectionStr);
                    con.Open();
                    cmd = new SqlCommand("etrade.LogOnlinePaymentQueryStrParamsSp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@qrystr", trimString);

                    cmd.Parameters.Add("@tokenId", SqlDbType.BigInt, 100);

                    cmd.Parameters["@tokenId"].Direction = ParameterDirection.Output;


                    int InsertResult = cmd.ExecuteNonQuery();
                    //    Int64 outputtoken = (Int64)cmd.Parameters["@tokenId"].Value;

                    if (InsertResult == -1)
                    {

                        Ds.Tables["UpdateUserDetails"].Rows[0]["RedirectUrl"] = "ErrorPage";
                    }
                    else
                    {


                        String requestNumberWithoutBackSlaSh = BrokerData.RequestNumber.Replace('/', '_');
                        var encodedRequestNumber = System.Web.HttpUtility.UrlEncode(requestNumberWithoutBackSlaSh);
                        var enCryptedRequestNumber = System.Web.HttpUtility.UrlEncode(CommonFunctions.CsUploadEncrypt(encodedRequestNumber));


                        Int64 outputtoken = (Int64)cmd.Parameters["@tokenId"].Value;
                        var enxryptedtoken = MCEncrypt(outputtoken.ToString());
                        var encodeenxryptedtoken = System.Web.HttpUtility.UrlEncode(enxryptedtoken);

                        BrokerData.urltoredirectforpayments = BrokerData.urltoredirectforpayments + encodeenxryptedtoken;
                        BrokerData.redirecturl = BrokerData.redirecturl + "?TokenId=" + encodeenxryptedtoken + "&ePayReqNo=" + enCryptedRequestNumber;

                        con = new SqlConnection(connectionStr);
                        con.Open();
                        cmd = new SqlCommand("etrade.Sp_UpdateRedirectiondetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RedirectURL", BrokerData.redirecturl);
                        cmd.Parameters.AddWithValue("@tokenId", outputtoken);
                        cmd.Parameters.AddWithValue("@sessionid", BrokerData.sessionid);
                        cmd.Parameters.AddWithValue("@Userid", BrokerData.Userid);
                        InsertResult = cmd.ExecuteNonQuery();
                        Ds.Tables["UpdateUserDetails"].Rows[0]["RedirectUrl"] = BrokerData.urltoredirectforpayments;
                        //String statusofpayments = GetpaidStatus(BrokerData.RequestNumber).ToString();
                        //if (statusofpayments.Trim() == "Paid")
                        //{

                        //    Ds.Tables["UpdateUserDetails"].Rows[0]["RedirectUrl"] = BrokerData.redirecturl;

                        //    //NotifyEmail(BrokerData.RequestNumber, "", 0);

                        //    //sendEmailViaWebApi(Ds1.Tables[0].Rows[0]["RequestForEmail"].ToString(),
                        //    //                          Ds1.Tables[0].Rows[0]["ESERVICEREQUESTNUMBER"].ToString(),
                        //    //                          Ds1.Tables[0].Rows[0]["Name"].ToString(),
                        //    //                          "",//Notification type param not used
                        //    //                         EngSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameEng"].ToString(),
                        //    //                         AraSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameAra"].ToString(),
                        //    //                          Ds1.Tables[0].Rows[0]["StatusEng"].ToString(), 
                        //    //                          Ds1.Tables[0].Rows[0]["StatusAra"].ToString()
                        //    //                          , "EService Request " + Ds1.Tables[0].Rows[0]["Status"].ToString());
                        //}
                        //else
                        //{
                        //    Ds.Tables["UpdateUserDetails"].Rows[0]["RedirectUrl"] = BrokerData.urltoredirectforpayments;
                        //}

                    }
                }
                else
                {

                    Ds.Tables["UpdateUserDetails"].Rows[0]["RedirectUrl"] = FreeServiceredirecturl;

                }


            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message + ex.StackTrace + BrokerData.RequestNumber + BrokerData.Userid, "UpdateBrokerUserDetailsDS In Mobiledatabase.cs");


                throw ex;
            }
            return Ds;
        }

        public static void WriteToLogFile(string inputvalue, string sessionDetails)
        {

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(sessionDetails + "Inpu" +
                    "tValue>" + inputvalue, EventLogEntryType.Information, 101, 1);
            }

        }

        public static String GetpaidStatus(String requestNumber)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            SqlConnection con = new SqlConnection(connectionStr);
            con.Open();
            SqlCommand cmd = new SqlCommand("etrade.Sp_checkpaymentsForBrsEservices", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EServiceRequestNumber", requestNumber);

            cmd.Parameters.Add("@Result", SqlDbType.Char, 500);

            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            String CheckInsertResult = (String)cmd.Parameters["@Result"].Value;
            return CheckInsertResult;

        }
        public static String MCEncrypt(String clearText)
        {
            String EncryptionKey = getPrivateKey();// dsConfig.Tables[0].Rows[0]["configvalue"].ToString();
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Rijndael encryptor = Rijndael.Create())
            {
                //encryptor.Padding = PaddingMode.;
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        private static String getPrivateKey()
        {
            String privateKey = "";
            //try
            {
                String connectStr = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
                using (SqlConnection connect = new SqlConnection(connectStr))
                {
                    using (SqlCommand command = new SqlCommand("getPrivateKey", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@privateKey", SqlDbType.NVarChar, 150).Direction = ParameterDirection.Output;

                        connect.Open();

                        command.ExecuteNonQuery();


                        if (command.Parameters["@privateKey"].Value != null)
                        {
                            privateKey = command.Parameters["@privateKey"].Value.ToString();
                        }

                    }
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("privateKeyGCS", " Error => " + ex.ToString(), EventLogEntryType.Error);
            //}

            return privateKey;
        }
        #endregion

        #region BrokerRenewal
        public static String GetRequestDetaillist(BrokerRenewalModel frdata)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_EServiceRequest", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(frdata.Userid))
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = frdata.Userid;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        public static String GetBrokerDetailslist(BrokerRenewalModel frdata)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_SubBrokerDetailsForEservices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(frdata.Userid))
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = frdata.Userid;


                        if (String.IsNullOrEmpty(frdata.Orgid))
                            sCmd.Parameters.Add("@OrganizationIDS", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrganizationIDS", SqlDbType.VarChar).Value = frdata.Orgid;

                        if (String.IsNullOrEmpty(frdata.Requestorserviceid))
                            sCmd.Parameters.Add("@serviceid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@serviceid", SqlDbType.VarChar).Value = frdata.Requestorserviceid;






                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        public static String FNCheckAutheticationForBrokerRenwal(BrokerRenewalModel frdata)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_getBrokerDetailsForEservices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(frdata.tokenid))
                            sCmd.Parameters.Add("@TokenID", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@TokenID", SqlDbType.VarChar).Value = frdata.tokenid;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        #endregion

        #region EserviceListDetailsEntityWise
        //public static String GetEntityServiceList(BrokerServiceRequestModel frdata)
        //{
        //    connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    DataSet Ds = new DataSet();
        //    try
        //    {
        //        using (var sCon = new SqlConnection(connectionStr))
        //        {
        //            using (var sCmd = new SqlCommand("etrade.usp_GetEntityServiceList", sCon))
        //            {
        //                sCmd.CommandType = CommandType.StoredProcedure;
        //                if (String.IsNullOrEmpty(frdata.userid))
        //                    sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = DBNull.Value;
        //                else
        //                    sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = frdata.userid;


        //                SqlDataAdapter da = new SqlDataAdapter(sCmd);
        //                da.Fill(Ds);
        //                // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
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
        //    // return Ds;
        //    return JsonConvert.SerializeObject(Ds, Formatting.None);
        //}


        #endregion


        internal static String UpdateCallbackRedirectInfo(String tokenId, String mUserid, String OPTokenId, String RedirectUrl, String EpayReqNo)
        {

            Result rslt = GetValidUserDetails(tokenId, mUserid);
            rslt.status = "0";
            if (rslt.status == "0")
            {
                rslt.status = UpdateOnlinePaymentTokens(tokenId, mUserid, OPTokenId, RedirectUrl, EpayReqNo);
            }
            return JsonConvert.SerializeObject(rslt, Formatting.None);
        }

        private static String UpdateOnlinePaymentTokens(String tokenId, String mUserid, String OPTokenId, String RedirectUrl, String EpayReqNo)
        {
            String ConString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(ConString);
            String RetStatus = "0";
            try
            {
                var EpayReqNoEncoDed = System.Web.HttpUtility.UrlEncode(EpayReqNo);
                EpayReqNo = CommonFunctions.CsUploadEncrypt(EpayReqNoEncoDed);

                String EncyTokenId = AES.EncryptToken(System.Web.HttpUtility.UrlEncode(tokenId));
                //SqlCommand sqlCmd = new SqlCommand("etrade.usp_UpdateOnlinePaymentTokensForUrlCallBack", sqlCon);
                SqlCommand sqlCmd = new SqlCommand("etrade.usp_BRS_UpdateOnlinePaymentTokensForUrlCallBack", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                SqlParameter paramsTokenId = sqlCmd.Parameters.Add("@TokenId", SqlDbType.VarChar, 200);
                paramsTokenId.Value = EncyTokenId;
                SqlParameter paramUserid = sqlCmd.Parameters.Add("@Userid", SqlDbType.VarChar, 50);
                paramUserid.Value = mUserid;
                SqlParameter paramOPTokenId = sqlCmd.Parameters.Add("@OPTokenId", SqlDbType.VarChar, 50);
                paramOPTokenId.Value = AES.DecryptToken(System.Web.HttpUtility.UrlDecode(OPTokenId));
                SqlParameter paramRedirectUrl = sqlCmd.Parameters.Add("@RedirectUrl", SqlDbType.NVarChar, 500);
                paramRedirectUrl.Value = RedirectUrl + "?TokenId=" + System.Web.HttpUtility.UrlEncode(EncyTokenId) + "&ePayReqNo=" + EpayReqNo;

                SqlParameter paramStatus = sqlCmd.Parameters.Add("@Status", SqlDbType.VarChar, 50);
                paramStatus.Direction = ParameterDirection.Output;
                paramStatus.Value = RetStatus;

                sqlCon.Open();
                sqlCmd.ExecuteNonQuery();
                RetStatus = (String)(sqlCmd.Parameters["@Status"].Value);
                sqlCon.Close();

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                if (sqlCon != null && sqlCon.State == ConnectionState.Open)
                    sqlCon.Close();
            }
            return RetStatus;
        }





        //private static String UpdateOnlinePaymentTokens(String tokenId, String mUserid, String OPTokenId, String RedirectUrl)
        //{
        //    String ConString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    SqlConnection sqlCon = new SqlConnection(ConString);
        //    String RetStatus = "0";
        //    try
        //    {
        //        String EncyTokenId = Crypt.EncryptData(tokenId) ;
        //        SqlCommand sqlCmd = new SqlCommand("etrade.usp_UpdateOnlinePaymentTokensForUrlCallBack", sqlCon);
        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        SqlParameter paramsTokenId = sqlCmd.Parameters.Add("@TokenId", SqlDbType.VarChar,200);
        //        paramsTokenId.Value = EncyTokenId;
        //        SqlParameter paramUserid = sqlCmd.Parameters.Add("@Userid", SqlDbType.VarChar,50);
        //        paramUserid.Value = mUserid;
        //        SqlParameter paramRequestNo = sqlCmd.Parameters.Add("@RequestNo", SqlDbType.VarChar,50);
        //        paramRequestNo.Value = OPTokenId;
        //        SqlParameter paramRedirectUrl = sqlCmd.Parameters.Add("@RedirectUrl", SqlDbType.NVarChar, 500);
        //        paramRedirectUrl.Value = RedirectUrl+"?"+ EncyTokenId;

        //        SqlParameter paramStatus = sqlCmd.Parameters.Add("@Status", SqlDbType.VarChar, 50);
        //        paramStatus.Direction = ParameterDirection.Output;
        //        paramStatus.Value = RetStatus;

        //        sqlCon.Open();
        //        sqlCmd.ExecuteNonQuery();
        //        RetStatus = (String)(sqlCmd.Parameters["@Status"].Value);
        //        sqlCon.Close();

        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //    finally
        //    {
        //        if (sqlCon != null && sqlCon.State == ConnectionState.Open)
        //            sqlCon.Close();
        //    }
        //    return RetStatus;
        //}





        internal static String ValidateUserToken(String tokenId, String mUserid)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var connectDB = new SqlConnection(connectionStr))
                //using (var commandDB = new SqlCommand("etrade.usp_ValidateUserTokensOnUrlCallBack", connectDB))
                using (var commandDB = new SqlCommand("etrade.usp_BRS_ValidateUserTokensOnUrlCallBack", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {
                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@SessionId", SqlDbType.NVarChar).Value = AES.DecryptToken(tokenId);
                    dataAdapter.Fill(Ds);
                    for (int i = 0; i < Ds.Tables.Count; i++)
                    {
                        if (Ds.Tables[i].Columns.Contains("TableName"))
                        {
                            if (Ds.Tables[i].Rows.Count > 0)
                                Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                        }
                        else
                        {
                            Ds.Tables.RemoveAt(i);
                            i--;
                        }
                    }
                    connectDB.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        //public static String GetCivilIDDetailsFromMoci(User frdata)
        //{
        //    connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    var TradeLicData = "";
        //    var commLicData = "";
        //    var company_name = "";
        //    //   var commLicDataT = "";

        //    var importLicData = "";
        //    DataSet Ds = new DataSet();
        //    string xmlreceived = "";
        //    try
        //    {


        //        MociLicensesService.MociLicensesServiceProxyWebServiceSoapClient svc1 = new MociLicensesService.MociLicensesServiceProxyWebServiceSoapClient();

        //        PAIProxyService.PAIProxyServiceSoapClient svc2 = new PAIProxyService.PAIProxyServiceSoapClient();


        //        //WriteToLogFile(frdata.CompanyCivilId.ToString(), "civilid");
        //        if (frdata.CompanyCivilId != null && frdata.CompanyCivilId!="" && frdata.importerLicenseNumber==null)

        //        {

        //            WriteToLogFile(frdata.CompanyCivilId.ToString(), "ImportLicenseCheck");
        //            xmlreceived = svc1.InquireCommLicDetailsFromSvcV2(frdata.CompanyCivilId);


        //            var xDoc = XDocument.Parse(xmlreceived); //OR XDocument.Load(filename)

        //            WriteToLogFile(xmlreceived, "insidecompanycivilid");
        //            //var penta = xDoc.Descendants("").FirstOrDefault




        //            foreach (var item in xDoc.Descendants("{http://tempuri.org/}licnData"))
        //            {

        //                var e = item.Elements();

        //                foreach (var i in e)
        //                {

        //                    if (i.Name.ToString().Contains("licn_comm_no"))
        //                    {
        //                        commLicData = i.Value;

        //                    }
        //                    if (i.Name.ToString().Contains("comm_book_no"))
        //                    {
        //                        TradeLicData = i.Value;
        //                        break;
        //                    }
        //                    if (i.Name.ToString().Contains("company_name"))
        //                    {
        //                        company_name = i.Value;

        //                    }


        //                }


        //            }

        //            foreach (var item in xDoc.Descendants("{http://tempuri.org/}ImportLicn"))
        //            {

        //                var e = item.Elements();

        //                foreach (var i in e)
        //                {
        //                    if (i.Name.ToString().Contains("IMPORT_NUMBER"))
        //                    {
        //                        importLicData = i.Value;
        //                        break;
        //                    }



        //                }


        //            }

        //        }
        //        else
        //        {

        //            WriteToLogFile("insideimportlicense", frdata.importerLicenseNumber);
        //            string TempimporterLicenseNumber = "";
        //            string CompanyCivilIDfromXml = "";
        //            // string importresponse = "";

        //             xmlreceived = svc1.InquireImportLicDetailsFromSvcV2(frdata.importerLicenseNumber.ToString());

        //            var rootdataFromNewImpService = XElement.Parse(xmlreceived);
        //            XNamespace ns1 = "http://www.w3.org/2001/XMLSchema-instance";
        //            //WriteToLogFile(xmlrecvd, "fromNewImportService");

        //            WriteToLogFile("XMlRecvdFromNewService", xmlreceived);

        //            if(rootdataFromNewImpService.Descendants("GetImportLicnResult").FirstOrDefault().Element("LICN_CIVIL_ID")!=null)
        //            {

        //                WriteToLogFile("Inside", "inside");

        //                CompanyCivilIDfromXml = rootdataFromNewImpService.Descendants("GetImportLicnResult").FirstOrDefault().Element("LICN_CIVIL_ID").Value;
        //                xmlreceived = svc1.InquireCommLicDetailsFromSvcV2(frdata.CompanyCivilId);


        //                var xDoc = XDocument.Parse(xmlreceived); 

        //                foreach (var item in xDoc.Descendants("{http://tempuri.org/}licnData"))
        //                {

        //                    var e = item.Elements();

        //                    foreach (var i in e)
        //                    {

        //                        if (i.Name.ToString().Contains("licn_comm_no"))
        //                        {
        //                            commLicData = i.Value;

        //                        }
        //                        if (i.Name.ToString().Contains("comm_book_no"))
        //                        {
        //                            TradeLicData = i.Value;
        //                            break;
        //                        }
        //                        //if (i.Name.ToString().Contains("company_name"))
        //                        //{
        //                        //    company_name = i.Value;

        //                        //}

        //                    }


        //                }
        //                importLicData = frdata.importerLicenseNumber;



        //            }
        //            else
        //            { 
        //            // connect first this and then move else  var s = svc1.InquireImportLicDetailsFromSvcV2
        //            if (frdata.importerLicenseNumber.Contains('/'))
        //            {

        //                TempimporterLicenseNumber = CommonFunctions.getformattedImportLicense(frdata.importerLicenseNumber);

        //                xmlreceived = svc1.inquireImportLicDetail(Convert.ToInt32(TempimporterLicenseNumber), 2);
        //            }
        //            else
        //            {
        //                //  Console.WriteLine(DateTime.Now.ToLongTimeString());
        //                xmlreceived = svc1.inquireImportLicDetail(Convert.ToInt32(frdata.importerLicenseNumber), 1);
        //                //  Console.WriteLine(DateTime.Now.ToLongTimeString());
        //                WriteToLogFile(xmlreceived, "xmlRecivedInElse");
        //            }

        //            var root = XElement.Parse(xmlreceived);
        //            XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";

        //            // importLicData = root.Descendants("importLicData").FirstOrDefault().Element("importLicNumber").Value;
        //            importLicData = frdata.importerLicenseNumber;
        //            commLicData = root.Descendants("commLicData").FirstOrDefault().Element("commLicNumber").Value;
        //            TradeLicData = root.Descendants("commLicData").FirstOrDefault().Element("commRecord").Value;

        //                //commLicData = root.Descendants("importLicData").FirstOrDefault().Element("commLicNumber").Value;
        //                //TradeLicData = root.Descendants("importLicData").FirstOrDefault().Element("commRecord").Value;

        //                // company_name=root.Descendants("commLicData").FirstOrDefault().Element("commName").Value;
        //            }
        //        }



        //        Ds.Tables.Add("MociDataResult");


        //        Ds.Tables["MociDataResult"].Columns.Add("TradeLicData", typeof(string));
        //        Ds.Tables["MociDataResult"].Columns.Add("commLicData", typeof(string));
        //        Ds.Tables["MociDataResult"].Columns.Add("importLicData", typeof(string));
        //  //    Ds.Tables["MociDataResult"].Columns.Add("companyname", typeof(string));

        //        Ds.Tables["MociDataResult"].Columns.Add("TableName", typeof(string));

        //        Ds.Tables["MociDataResult"].Rows.Add(TradeLicData, commLicData, importLicData, "MociDataResult");

        //        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
        //        for (int i = 0; i < Ds.Tables.Count; i++)
        //        {
        //            if (Ds.Tables[i].Columns.Contains("TableName"))
        //            {
        //                if (Ds.Tables[i].Rows.Count > 0)
        //                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
        //            }
        //            else
        //            {
        //                Ds.Tables.RemoveAt(i);
        //                i--;
        //            }
        //        }
        //    }


        //    catch (Exception ex)
        //    {
        //        WriteToLogFile(ex.StackTrace + ex.Message + "error fetching Moci Result", "companycivilid='" + frdata.CompanyCivilId + "" + "importerLicenseNumber=" + frdata.importerLicenseNumber + "'TheXmlIfanyreceived'" + xmlreceived + "'");
        //    }
        //    // return Ds;
        //    return JsonConvert.SerializeObject(Ds, Formatting.None);
        //}
        public static String GetCivilIDDetailsFromMoci(User frdata)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var TradeLicData = "";
            var commLicData = "";
            var company_name = "";
            //   var commLicDataT = "";

            var importLicData = "";
            DataSet Ds = new DataSet();
            string xmlreceived = "";
            try
            {


                MociLicensesService.MociLicensesServiceProxyWebServiceSoapClient svc1 = new MociLicensesService.MociLicensesServiceProxyWebServiceSoapClient();

                PAIProxyService.PAIProxyServiceSoapClient svc2 = new PAIProxyService.PAIProxyServiceSoapClient();


                //WriteToLogFile(frdata.CompanyCivilId.ToString(), "civilid");
                if (frdata.CompanyCivilId != null && frdata.CompanyCivilId != "" && frdata.importerLicenseNumber == null)

                {

                  //  WriteToLogFile(frdata.CompanyCivilId.ToString(), "ImportLicenseCheck");
                    xmlreceived = svc1.InquireCommLicDetailsFromSvcV2(frdata.CompanyCivilId);


                    var xDoc = XDocument.Parse(xmlreceived); //OR XDocument.Load(filename)

                 //   WriteToLogFile(xmlreceived, "insidecompanycivilid");
                    //var penta = xDoc.Descendants("").FirstOrDefault




                    foreach (var item in xDoc.Descendants("{http://tempuri.org/}licnData"))
                    {

                        var e = item.Elements();

                        foreach (var i in e)
                        {

                            if (i.Name.ToString().Contains("licn_comm_no"))
                            {
                                commLicData = i.Value;

                            }
                            if (i.Name.ToString().Contains("comm_book_no"))
                            {
                                TradeLicData = i.Value;
                                break;
                            }
                            if (i.Name.ToString().Contains("company_name"))
                            {
                                company_name = i.Value;

                            }


                        }


                    }

                    foreach (var item in xDoc.Descendants("{http://tempuri.org/}ImportLicn"))
                    {

                        var e = item.Elements();

                        foreach (var i in e)
                        {
                            if (i.Name.ToString().Contains("IMPORT_NUMBER"))
                            {
                                importLicData = i.Value;
                                break;
                            }



                        }


                    }

                }
                else
                {

                //    WriteToLogFile("insideimportlicense", frdata.importerLicenseNumber);
                    string TempimporterLicenseNumber = "";
                    string CompanyCivilIDfromXml = "";
                    // string importresponse = "";

                    xmlreceived = svc1.InquireImportLicDetailsFromSvcV2(frdata.importerLicenseNumber.ToString());

                    // var xDoc = XDocument.Parse(xmlreceived);
                    var rootdataFromNewImpService = XDocument.Parse(xmlreceived);

                    foreach (var item in rootdataFromNewImpService.Descendants("{http://tempuri.org/}GetImportLicnResult"))
                    {

                        var e = item.Elements();

                        foreach (var i in e)
                        {
                            if (i.Name.ToString().Contains("LICN_CIVIL_ID"))
                            {
                                CompanyCivilIDfromXml = i.Value;
                                break;
                            }

                        }
                    }

                  //  WriteToLogFile("XMlRecvdFromNewService", xmlreceived);

                    if (CompanyCivilIDfromXml != "")
                    {



                        xmlreceived = svc1.InquireCommLicDetailsFromSvcV2(CompanyCivilIDfromXml);


                        var xDoc = XDocument.Parse(xmlreceived);

                        foreach (var item in xDoc.Descendants("{http://tempuri.org/}licnData"))
                        {

                            var e = item.Elements();

                            foreach (var i in e)
                            {

                                if (i.Name.ToString().Contains("licn_comm_no"))
                                {
                                    commLicData = i.Value;

                                }
                                if (i.Name.ToString().Contains("comm_book_no"))
                                {
                                    TradeLicData = i.Value;
                                    break;
                                }
                                //if (i.Name.ToString().Contains("company_name"))
                                //{
                                //    company_name = i.Value;

                                //}

                            }


                        }
                        importLicData = frdata.importerLicenseNumber;



                    }
                    else
                    {
                        // connect first this and then move else  var s = svc1.InquireImportLicDetailsFromSvcV2
                        if (frdata.importerLicenseNumber.Contains('/'))
                        {

                            TempimporterLicenseNumber = CommonFunctions.getformattedImportLicense(frdata.importerLicenseNumber);

                            xmlreceived = svc1.inquireImportLicDetail(Convert.ToInt32(TempimporterLicenseNumber), 2);
                        }
                        else
                        {
                            //  Console.WriteLine(DateTime.Now.ToLongTimeString());
                            xmlreceived = svc1.inquireImportLicDetail(Convert.ToInt32(frdata.importerLicenseNumber), 1);
                            //  Console.WriteLine(DateTime.Now.ToLongTimeString());
                          //  WriteToLogFile(xmlreceived, "xmlRecivedInElse");
                        }

                        var root = XElement.Parse(xmlreceived);
                        XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";

                        // importLicData = root.Descendants("importLicData").FirstOrDefault().Element("importLicNumber").Value;
                        importLicData = frdata.importerLicenseNumber;
                        commLicData = root.Descendants("commLicData").FirstOrDefault().Element("commLicNumber").Value;
                        TradeLicData = root.Descendants("commLicData").FirstOrDefault().Element("commRecord").Value;

                        //commLicData = root.Descendants("importLicData").FirstOrDefault().Element("commLicNumber").Value;
                        //TradeLicData = root.Descendants("importLicData").FirstOrDefault().Element("commRecord").Value;

                        // company_name=root.Descendants("commLicData").FirstOrDefault().Element("commName").Value;
                    }
                }



                Ds.Tables.Add("MociDataResult");


                Ds.Tables["MociDataResult"].Columns.Add("TradeLicData", typeof(string));
                Ds.Tables["MociDataResult"].Columns.Add("commLicData", typeof(string));
                Ds.Tables["MociDataResult"].Columns.Add("importLicData", typeof(string));
                //    Ds.Tables["MociDataResult"].Columns.Add("companyname", typeof(string));

                Ds.Tables["MociDataResult"].Columns.Add("TableName", typeof(string));

                Ds.Tables["MociDataResult"].Rows.Add(TradeLicData, commLicData, importLicData, "MociDataResult");

                // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                for (int i = 0; i < Ds.Tables.Count; i++)
                {
                    if (Ds.Tables[i].Columns.Contains("TableName"))
                    {
                        if (Ds.Tables[i].Rows.Count > 0)
                            Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                    }
                    else
                    {
                        Ds.Tables.RemoveAt(i);
                        i--;
                    }
                }
            }


            catch (Exception ex)
            {
                WriteToLogFile(ex.StackTrace + ex.Message + "error fetching Moci Result", "companycivilid='" + frdata.CompanyCivilId + "" + "importerLicenseNumber=" + frdata.importerLicenseNumber + "'TheXmlIfanyreceived'" + xmlreceived + "'");
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        internal static String UpdateUserSession(UserSession data)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var connectDB = new SqlConnection(connectionStr))
                using (var commandDB = new SqlCommand("etrade.UpdateUserSession", connectDB))
                using (var dataAdapter = new SqlDataAdapter(commandDB))
                {
                    commandDB.CommandType = CommandType.StoredProcedure;
                    commandDB.Parameters.Add("@UserId", SqlDbType.Int).Value = data.Userid;
                    commandDB.Parameters.Add("@lang", SqlDbType.VarChar).Value = data.lang;
                    dataAdapter.Fill(Ds);
                    //commandDB.ExecuteNonQuery();
                    connectDB.Close();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }



        #region Exam Request 

        public static String getExamDetailsByEservicesRequestId(EservicesRequests EserviceRequest)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.getExamDetailsByEservicesRequestId", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@EserviceRequestId", SqlDbType.BigInt).Value = EserviceRequest.EserviceRequestId;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }


        public static String updateExamCandidateInfo(ExamCandidateInfo eXamCandidateInfo)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.SP_UpdateExamCandidateInfoStatus", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@EserviceRequestId", SqlDbType.BigInt).Value = eXamCandidateInfo.EserviceRequestId;


                        sCmd.Parameters.Add("@Stateid", SqlDbType.VarChar).Value = eXamCandidateInfo.stateId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        public static String InitiateExamRequest(examRequestViewModel examRequestViewMd)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_getBrokerDetailsForEservices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;
                        if (String.IsNullOrEmpty(examRequestViewMd.RequesterUserId.ToString()))
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Userid", SqlDbType.VarChar).Value = examRequestViewMd.RequesterUserId.ToString();



                        //if (String.IsNullOrEmpty(examRequestViewMd.organizationid.ToString()))
                        /// sCmd.Parameters.Add("@orgid", SqlDbType.NVarChar).Value = DBNull.Value;
                        //else
                        sCmd.Parameters.Add("@orgid", SqlDbType.NVarChar).Value = DBNull.Value;  //examRequestViewMd.organizationid.ToString();



                        //if (String.IsNullOrEmpty(examRequestViewMd.RequestNumber.ToString()))
                        // sCmd.Parameters.Add("@requestNumber", SqlDbType.VarChar).Value = DBNull.Value;
                        // else
                        sCmd.Parameters.Add("@requestNumber", SqlDbType.VarChar).Value = DBNull.Value; //examRequestViewMd.RequestNumber.ToString();



                        if (String.IsNullOrEmpty(examRequestViewMd.Referenceprofile))
                            sCmd.Parameters.Add("@ReferenceProfile", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@ReferenceProfile", SqlDbType.VarChar).Value = examRequestViewMd.Referenceprofile;



                        if (String.IsNullOrEmpty(examRequestViewMd.lang))
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = examRequestViewMd.lang;




                        if (String.IsNullOrEmpty(examRequestViewMd.Serviceid.ToString()))
                            sCmd.Parameters.Add("@Serviceid", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@Serviceid", SqlDbType.Int).Value = examRequestViewMd.Serviceid;


                        if (String.IsNullOrEmpty(examRequestViewMd.mobileUserId.ToString()))
                            sCmd.Parameters.Add("@mobileUserId", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@mobileUserId", SqlDbType.Int).Value = examRequestViewMd.mobileUserId;

                        //if (String.IsNullOrEmpty(examRequestViewMd.mobileUserId.ToString()))
                        sCmd.Parameters.Add("@RequestedForMobileUserid ", SqlDbType.Int).Value = DBNull.Value;
                        //else
                        //    sCmd.Parameters.Add("@RequestedForMobileUserid ", SqlDbType.Int).Value = examRequestViewMd.mobileUserId;


                        sCmd.Parameters.Add("@EXAMREQ", SqlDbType.Bit).Value = examRequestViewMd.EXAMREQ; //1;

                        sCmd.Parameters.Add("@RequestForUserType", SqlDbType.BigInt).Value = examRequestViewMd.BrokerType;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        #endregion Exam Request 





        internal static string GETRequestListfortheUser(EserviceRequest R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GETRequestListfortheUser", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = R.RequesterUserId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
         internal static string CreateInspectionAppointment(InspectionAppointment R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
//                 @UserId bigint='',    
// @OrgId varchar(50),    
// @PortId varchar(50),    
    
// @DONumber nvarchar(50)='',    
// @SecurityCode nvarchar(50)='',    
// @SelectedVehicleList varchar(500),    
// @InspectionDate NVARCHAR(50),    
// @SelectedRoundId int     
// ,@DeclarationId varchar(500)=''    
// --,@RoundID     
// --,@RequesterUserId    
// ,@RequesterType varchar(500)=''    
// --,@RequesterId     
// ,@RequestSubmissionDateTime  varchar(500)=''    
// ,@StateId varchar(500)=''    
// ,@DateCreated varchar(500)=''    
// ,@CreatedBy varchar(500)=''    
// ,@OwnerOrgId varchar(500)=''    
// ,@OwnerLocId varchar(500)=''    
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.CraeteSchedule", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (String.IsNullOrEmpty(R.UserId))
                            sCmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = R.UserId;
                        if (String.IsNullOrEmpty(R.OrgId))
                            sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = R.OrgId;
                        if (String.IsNullOrEmpty(R.PortId))
                            sCmd.Parameters.Add("@PortId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@PortId", SqlDbType.VarChar).Value = R.PortId;
                        if (String.IsNullOrEmpty(R.DONumber))
                            sCmd.Parameters.Add("@DONumber", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DONumber", SqlDbType.VarChar).Value = R.DONumber;
                        if (String.IsNullOrEmpty(R.SecurityCode))
                            sCmd.Parameters.Add("@SecurityCode", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@SecurityCode", SqlDbType.VarChar).Value = R.SecurityCode;
                        if (String.IsNullOrEmpty(R.SelectedVehicleList))
                            sCmd.Parameters.Add("@SelectedVehicleList", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@SelectedVehicleList", SqlDbType.VarChar).Value = R.SelectedVehicleList;

                        string year = DateTime.ParseExact(R.InspectionDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")).Year.ToString();
                        string month = DateTime.ParseExact(R.InspectionDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")).Month.ToString();
                        string day = DateTime.ParseExact(R.InspectionDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")).Day.ToString();

                        R.InspectionDate = year + "-" + month + "-" + day;

                        if (String.IsNullOrEmpty(R.InspectionDate))
                            sCmd.Parameters.Add("@InspectionDate", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@InspectionDate", SqlDbType.VarChar).Value = R.InspectionDate;
                        // if (String.IsNullOrEmpty(R.InspectionRound))
                        //     sCmd.Parameters.Add("@SelectedRoundId", SqlDbType.VarChar).Value = DBNull.Value;
                        // else

                        sCmd.Parameters.Add("@SelectedZoneId", SqlDbType.VarChar).Value = R.InspectionZone;
                        sCmd.Parameters.Add("@SelectedTerminalId", SqlDbType.VarChar).Value = R.InspectionTerminal;
                        sCmd.Parameters.Add("@SelectedRoundId", SqlDbType.VarChar).Value = R.InspectionRound;

                        if (String.IsNullOrEmpty(R.DeclarationType))
                            sCmd.Parameters.Add("@DeclarationType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DeclarationType", SqlDbType.VarChar).Value = R.DeclarationType;
                        if (String.IsNullOrEmpty(R.TempDeclarationId))
                            sCmd.Parameters.Add("@TempDeclarationId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@TempDeclarationId", SqlDbType.VarChar).Value = R.TempDeclarationId;

                        if (String.IsNullOrEmpty(R.DeclarationId))
                            sCmd.Parameters.Add("@DeclarationId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DeclarationId", SqlDbType.VarChar).Value = R.DeclarationId;
                        if (String.IsNullOrEmpty(R.RequesterType))
                            sCmd.Parameters.Add("@RequesterType", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@RequesterType", SqlDbType.VarChar).Value = R.RequesterType;



                             if (String.IsNullOrEmpty(R.RequestSubmissionDateTime))
                            sCmd.Parameters.Add("@RequestSubmissionDateTime", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@RequestSubmissionDateTime", SqlDbType.VarChar).Value = R.RequestSubmissionDateTime;
                        if (String.IsNullOrEmpty(R.Status))
                            sCmd.Parameters.Add("@StateId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@StateId", SqlDbType.VarChar).Value = R.Status;
                             if (String.IsNullOrEmpty(R.DateCreated))
                            sCmd.Parameters.Add("@DateCreated", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@DateCreated", SqlDbType.VarChar).Value = R.DateCreated;
                             if (String.IsNullOrEmpty(R.CreatedBy))
                            sCmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = R.CreatedBy;
                        if (String.IsNullOrEmpty(R.OwnerOrgId))
                            sCmd.Parameters.Add("@OwnerOrgId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OwnerOrgId", SqlDbType.VarChar).Value = R.OwnerOrgId;
                        if (String.IsNullOrEmpty(R.OwnerLocId))
                            sCmd.Parameters.Add("@OwnerLocId", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@OwnerLocId", SqlDbType.VarChar).Value = R.OwnerLocId;

                        // if (String.IsNullOrEmpty(R.lang))
                        //     sCmd.Parameters.Add("@RequestSubmissionDateTime", SqlDbType.VarChar).Value = DBNull.Value;
                        // else
                        //     sCmd.Parameters.Add("@RequestSubmissionDateTime", SqlDbType.VarChar).Value = R.lang;
                        // if (String.IsNullOrEmpty(R.lang))
                        //     sCmd.Parameters.Add("@StateId", SqlDbType.VarChar).Value = DBNull.Value;
                        // else
                        //     sCmd.Parameters.Add("@StateId", SqlDbType.VarChar).Value = R.lang;
                        // if (String.IsNullOrEmpty(R.lang))
                        //     sCmd.Parameters.Add("@DateCreated", SqlDbType.VarChar).Value = DBNull.Value;
                        // else
                        //     sCmd.Parameters.Add("@DateCreated", SqlDbType.VarChar).Value = R.lang;
                        // if (String.IsNullOrEmpty(R.lang))
                        //     sCmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = DBNull.Value;
                        // else
                        //     sCmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = R.lang;
                        // if (String.IsNullOrEmpty(R.lang))
                        //     sCmd.Parameters.Add("@OwnerOrgId", SqlDbType.VarChar).Value = DBNull.Value;
                        // else
                        //     sCmd.Parameters.Add("@OwnerOrgId", SqlDbType.VarChar).Value = R.lang;
                        // if (String.IsNullOrEmpty(R.lang))
                        //     sCmd.Parameters.Add("@OwnerLocId", SqlDbType.VarChar).Value = DBNull.Value;
                        // else
                        //     sCmd.Parameters.Add("@OwnerLocId", SqlDbType.VarChar).Value = R.lang;


                        // sCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@PortId", SqlDbType.NVarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@DONumber", SqlDbType.NVarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@SecurityCode", SqlDbType.NVarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@SelectedVehicleList", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@InspectionDate", SqlDbType.NVarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@SelectedRoundId", SqlDbType.Int).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@DeclarationId", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@RequesterType", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@RequestSubmissionDateTime", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@StateId", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@DateCreated", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@OwnerOrgId", SqlDbType.VarChar).Value = R.RequesterUserId;
                        // sCmd.Parameters.Add("@OwnerLocId", SqlDbType.VarChar).Value = R.RequesterUserId;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("CreateInspectionAppointment", "", "", "", "", ex.Message.ToString());
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
         internal static string UpdateInspectionAppointment(InspectionAppointment R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.UpdateInspectionAppointment", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@InspectionAppointmentRequestId", SqlDbType.BigInt).Value = R.AppointmentId;
                        sCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = R.UserId;
                        sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = R.OrgId;

                        string year = DateTime.ParseExact(R.InspectionDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")).Year.ToString();
                        string month = DateTime.ParseExact(R.InspectionDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")).Month.ToString();
                        string day = DateTime.ParseExact(R.InspectionDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")).Day.ToString();

                        R.InspectionDate = year + "-" + month + "-" + day;

                        sCmd.Parameters.Add("@InspectionDate", SqlDbType.DateTime).Value = R.InspectionDate;
                        sCmd.Parameters.Add("@SelectedZoneId", SqlDbType.Int).Value = R.InspectionZone;
                        sCmd.Parameters.Add("@SelectedTerminalId", SqlDbType.Int).Value = R.InspectionTerminal;
                        sCmd.Parameters.Add("@SelectedRoundId", SqlDbType.Int).Value = R.InspectionRound;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("UpdateInspectionAppointment", "", "", "", "", ex.Message.ToString());
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
         internal static string GetInspectionAppointmentDetails(ReqObj R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetInspectionAppointmentDetails", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@InspectionAppointmentRequestId", SqlDbType.BigInt).Value = R.CommonData;
                        sCmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = R.CommonData2;
                        sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = R.OrgID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("GetInspectionAppointmentDetails", "", "", "", "", ex.Message.ToString());
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static string CancelInspectionAppointment(ReqObj R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.CancelInspectionAppointment", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@InspectionAppointmentRequestId", SqlDbType.BigInt).Value = R.CommonData;
                        sCmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = R.CommonData2;
                        sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = R.OrgID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("CancelInspectionAppointment", "", "", "", "", ex.Message.ToString());
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static string GetInspectionRounds(ReqObj R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetInspectionRounds", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@InspectionAppointmentRequestId", SqlDbType.BigInt).Value = R.CommonData;
                        sCmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = R.CommonData2;
                        sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = R.OrgID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("GetInspectionRounds", "", "", "", "", ex.Message.ToString());
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
         internal static string GetInspectionAppointments(ReqObj R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GetInspectionAppointments", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = R.CommonData2;
                        sCmd.Parameters.Add("@OrgId", SqlDbType.VarChar).Value = R.OrgID;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("GetInspectionAppointments", "", "", "", "", ex.Message.ToString());
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
         internal static string getVehicleListFromDO(ReqObj R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.getVehicleListFromDO", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@DeclNumber", SqlDbType.VarChar).Value = R.CommonData;
                        sCmd.Parameters.Add("@TempDeclNumber", SqlDbType.VarChar).Value = R.CommonData1;
                        sCmd.Parameters.Add("@OrgId", SqlDbType.BigInt).Value = R.OrgID;
                        sCmd.Parameters.Add("@MobileUserId", SqlDbType.BigInt).Value = R.CommonData2;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("getVehicleListFromDO", "", "", "", "", ex.Message.ToString());
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static string UniqueTradeLicenseCheck(String TradeLicense, string OrgRequestId, string OrgId)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.CheckforUniqueTradeLicense", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@LicenseNumber", SqlDbType.NVarChar).Value = TradeLicense;
                        sCmd.Parameters.Add("@OrgRequestId", SqlDbType.Int).Value = Int32.Parse(OrgRequestId);
                        sCmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = Int32.Parse(OrgId);

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        internal static string GETRequestDetailsfortheRequest(EserviceRequest R)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.GETRequestDetailsfortheRequest", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@ESERVICEREQUESTNUMBER", SqlDbType.VarChar).Value = R.ESERVICEREQUESTNUMBER;
                        //mobile user id added post  security request by ubais 

                        //if(R.RequesterUserId==null)
                        //{ 
                        //sCmd.Parameters.Add("@MobileUserid", SqlDbType.Int).Value = R.RequesterUserId;
                        //}


                        if (String.IsNullOrEmpty(R.RequesterUserId.ToString()))
                            sCmd.Parameters.Add("@MobileUserid", SqlDbType.Int).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@MobileUserid", SqlDbType.Int).Value = R.RequesterUserId; ;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }



        internal static bool Notify(DataSet Ds, string ESreqNum = "")
        {
            try
            {
                if (Ds.Tables.Contains("EServiceRequests"))
                {
                    if (Ds.Tables["EServiceRequests"].Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Ds.Tables["EServiceRequests"].Rows[0]["eservicerequestnumber"].ToString()))// && Ds.Tables["EServiceRequests"].Rows[0]["eservicerequestnumber"].ToString()== "EServiceRequestSubmittedState")
                        {
                            ESreqNum = Ds.Tables["EServiceRequests"].Rows[0]["eservicerequestnumber"].ToString();
                            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                            DataSet Ds1 = new DataSet();

                            using (var sCon = new SqlConnection(connectionStr))
                            {
                                using (var sCmd = new SqlCommand("etrade.GetNotificDetails", sCon))
                                {
                                    sCmd.CommandType = CommandType.StoredProcedure;

                                    sCmd.Parameters.Add("@ESERVICEREQUESTNUMBER", SqlDbType.VarChar).Value = ESreqNum;

                                    SqlDataAdapter da = new SqlDataAdapter(sCmd);
                                    da.Fill(Ds1);
                                    if (Ds1.Tables.Count > 0)
                                    {
                                        if (Ds1.Tables[0].Rows.Count > 0)
                                        {
                                            if (Ds1.Tables[0].Rows[0]["Notify"].ToString() == "1" && Ds1.Tables[0].Rows[0]["StatusEng"].ToString() == "Submitted")
                                            {
                                                String EngSVCNames = String.Join(",", Ds1.Tables[0].AsEnumerable().Select(x => x.Field<string>("ServiceNameEng").ToString()).ToArray());
                                                String AraSVCNames = String.Join(",", Ds1.Tables[0].AsEnumerable().Select(x => x.Field<string>("ServiceNameAra").ToString()).ToArray());


                                                sendEmailViaWebApi(Ds1.Tables[0].Rows[0]["RequestForEmail"].ToString(),
                                                    Ds1.Tables[0].Rows[0]["ESERVICEREQUESTNUMBER"].ToString(),
                                                    Ds1.Tables[0].Rows[0]["Name"].ToString(),
                                                    "",//Notification type param not used
                                                   EngSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameEng"].ToString(),
                                                   AraSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameAra"].ToString(),
                                                    Ds1.Tables[0].Rows[0]["StatusEng"].ToString(), Ds1.Tables[0].Rows[0]["StatusAra"].ToString()
                                                    , "EService Request " + Ds1.Tables[0].Rows[0]["StatusEng"].ToString());
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        internal static bool NotifyEmail(string ESreqNum, string OrgReqNum, int? MobileUserId)
        {
            try
            {

                //ESreqNum = Ds.Tables["EServiceRequests"].Rows[0]["eservicerequestnumber"].ToString();
                connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                DataSet Ds1 = new DataSet();

                using (var sCon = new SqlConnection(connectionStr))
                {
                    if (OrgReqNum == "")
                    {
                        using (var sCmd = new SqlCommand("etrade.GetNotificDetails", sCon))
                        {
                            sCmd.CommandType = CommandType.StoredProcedure;

                            sCmd.Parameters.Add("@ESERVICEREQUESTNUMBER", SqlDbType.VarChar).Value = ESreqNum;

                            SqlDataAdapter da = new SqlDataAdapter(sCmd);
                            da.Fill(Ds1);
                            if (Ds1 != null)
                            {
                                if (Ds1.Tables.Count > 0)
                                {
                                    if (Ds1.Tables[0].Rows.Count > 0)
                                    {
                                        if (Ds1.Tables[0].Rows[0]["Notify"].ToString() == "1" && Ds1.Tables[0].Rows[0]["StatusEng"].ToString() == "Submitted")
                                        {
                                            //string EngSVCNames = String.Join(",", Ds1.Tables[0].AsEnumerable().Select(x => x.Field<string>("ServiceNameEng").ToString()).ToArray());
                                            //string AraSVCNames = String.Join(",", Ds1.Tables[0].AsEnumerable().Select(x => x.Field<string>("ServiceNameAra").ToString()).ToArray());


                                            //sendEmailViaWebApi(Ds1.Tables[0].Rows[0]["RequestForEmail"].ToString(),
                                            //    Ds1.Tables[0].Rows[0]["ESERVICEREQUESTNUMBER"].ToString(),
                                            //    Ds1.Tables[0].Rows[0]["Name"].ToString(),
                                            //    "",//Notification type param not used
                                            //   EngSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameEng"].ToString(),
                                            //   AraSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameAra"].ToString(),
                                            //    Ds1.Tables[0].Rows[0]["StatusEng"].ToString(), Ds1.Tables[0].Rows[0]["StatusAra"].ToString()
                                            //    , "EService Request " + Ds1.Tables[0].Rows[0]["StatusEng"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var sCmd = new SqlCommand("ETRADE.GETESERVICEREQUESTNUMBER", sCon))
                        {
                            sCmd.CommandType = CommandType.StoredProcedure;

                            sCmd.Parameters.Add("@OrgReqId", SqlDbType.Int).Value = Convert.ToInt32(OrgReqNum);
                            sCmd.Parameters.Add("@MobileUserId", SqlDbType.Int).Value = MobileUserId;

                            SqlDataAdapter da = new SqlDataAdapter(sCmd);
                            da.Fill(Ds1);
                            if (Ds1 != null)
                            {
                                if (Ds1.Tables.Count > 0)
                                {
                                    if (Ds1.Tables[0].Rows.Count > 0)
                                    {
                                        if (Ds1.Tables[0].Rows[0]["Notify"].ToString() == "1" && Ds1.Tables[0].Rows[0]["StatusEng"].ToString() == "Submitted")
                                        {
                                            //string EngSVCNames = String.Join(",", Ds1.Tables[0].AsEnumerable().Select(x => x.Field<string>("ServiceNameEng").ToString()).ToArray());
                                            //string AraSVCNames = String.Join(",", Ds1.Tables[0].AsEnumerable().Select(x => x.Field<string>("ServiceNameAra").ToString()).ToArray());


                                            //sendEmailViaWebApi(Ds1.Tables[0].Rows[0]["RequestForEmail"].ToString(),
                                            //    Ds1.Tables[0].Rows[0]["ESERVICEREQUESTNUMBER"].ToString(),
                                            //    Ds1.Tables[0].Rows[0]["Name"].ToString(),
                                            //    "",//Notification type param not used
                                            //   EngSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameEng"].ToString(),
                                            //   AraSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameAra"].ToString(),
                                            //    Ds1.Tables[0].Rows[0]["StatusEng"].ToString(), Ds1.Tables[0].Rows[0]["StatusAra"].ToString()
                                            //    , "EService Request " + Ds1.Tables[0].Rows[0]["Status"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                        //Ds1 = new DataSet();
                        //if (!string.IsNullOrEmpty(ESreqNum))
                        //    using (var sCmd = new SqlCommand("etrade.GetNotificDetails", sCon))
                        //    {
                        //        sCmd.CommandType = CommandType.StoredProcedure;

                        //        sCmd.Parameters.Add("@ESERVICEREQUESTNUMBER", SqlDbType.Int).Value = Convert.ToInt16(ESreqNum);

                        //        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        //        da.Fill(Ds1);
                        //        if (Ds1.Tables.Count > 0)
                        //        {
                        //            if (Ds1.Tables[0].Rows.Count > 0)
                        //            {
                        //                if (Ds1.Tables[0].Rows[0]["Notify"].ToString() == "1" && Ds1.Tables[0].Rows[0]["StatusEng"].ToString() == "Submitted")
                        //                {
                        //                    string EngSVCNames = String.Join(",", Ds1.Tables[0].AsEnumerable().Select(x => x.Field<string>("ServiceNameEng").ToString()).ToArray());
                        //                    string AraSVCNames = String.Join(",", Ds1.Tables[0].AsEnumerable().Select(x => x.Field<string>("ServiceNameAra").ToString()).ToArray());


                        //                    sendEmailViaWebApi(Ds1.Tables[0].Rows[0]["RequestForEmail"].ToString(),
                        //                        Ds1.Tables[0].Rows[0]["ESERVICEREQUESTNUMBER"].ToString(),
                        //                        Ds1.Tables[0].Rows[0]["Name"].ToString(),
                        //                        "",//Notification type param not used
                        //                       EngSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameEng"].ToString(),
                        //                       AraSVCNames,// Ds1.Tables[0].Rows[0]["ServiceNameAra"].ToString(),
                        //                        Ds1.Tables[0].Rows[0]["StatusEng"].ToString(), Ds1.Tables[0].Rows[0]["StatusAra"].ToString()
                        //                        , "EService Request " + Ds1.Tables[0].Rows[0]["Status"].ToString());
                        //                }
                        //            }
                        //        }
                        //    }
                        //else
                        //{
                        //    return false;
                        //}
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }



        public static String getUserDashBoard(User user)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.getUserDashBoard", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        sCmd.Parameters.Add("@userId", SqlDbType.BigInt).Value = user.mUserid;


                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);

                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }

        //GetBrokerAffairsDocsCheck

        #region Update Request
        internal static String updateRequest(Dictionary<String, String> requestToUpdate)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {

                if (requestToUpdate.Count > 0)
                {
                    using (var sCon = new SqlConnection(connectionStr))
                    {
                        using (var sCmd = new SqlCommand("etrade.SP_UpdateEserviceRequest", sCon))
                        {
                            sCmd.CommandType = CommandType.StoredProcedure;

                            sCmd.Parameters.Add("@serviceRequestNumber", SqlDbType.NVarChar).Value = requestToUpdate["serviceRequestNumber"];
                            sCmd.Parameters.Add("@EServiceRequestStateid", SqlDbType.VarChar).Value = requestToUpdate["EServiceRequestStateid"];
                            sCmd.Parameters.Add("@EServiceRequestDetailsStateid", SqlDbType.VarChar).Value = requestToUpdate["EServiceRequestDetailsStateid"];

                            SqlDataAdapter da = new SqlDataAdapter(sCmd);
                            da.Fill(Ds);

                            for (int i = 0; i < Ds.Tables.Count; i++)
                            {
                                if (Ds.Tables[i].Columns.Contains("TableName"))
                                {
                                    if (Ds.Tables[i].Rows.Count > 0)
                                        Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                                }
                                else
                                {
                                    Ds.Tables.RemoveAt(i);
                                    i--;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }
        #endregion Update Request

        public static String GetBrokerAffairsDocsCheck(BrokerUpdateModel frdata)
        {
            connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataSet Ds = new DataSet();
            try
            {
                using (var sCon = new SqlConnection(connectionStr))
                {
                    using (var sCmd = new SqlCommand("etrade.Sp_checkDocsUploaddedForBrsEservices", sCon))
                    {
                        sCmd.CommandType = CommandType.StoredProcedure;

                        if (String.IsNullOrEmpty(frdata.Eservicerequestid))
                            sCmd.Parameters.Add("@EServiceRequestid", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EServiceRequestid", SqlDbType.VarChar).Value = frdata.Eservicerequestid;

                        if (String.IsNullOrEmpty(frdata.Referenceprofile))
                            sCmd.Parameters.Add("@EServiceReferenceProfile", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            sCmd.Parameters.Add("@EServiceReferenceProfile", SqlDbType.VarChar).Value = frdata.Referenceprofile;



                        SqlDataAdapter da = new SqlDataAdapter(sCmd);
                        da.Fill(Ds);
                        // frdata.NewFileName = sCmd.Parameters["@NewName"].Value.ToString();
                        for (int i = 0; i < Ds.Tables.Count; i++)
                        {
                            if (Ds.Tables[i].Columns.Contains("TableName"))
                            {
                                if (Ds.Tables[i].Rows.Count > 0)
                                    Ds.Tables[i].TableName = Ds.Tables[i].Rows[0]["TableName"].ToString();
                            }
                            else
                            {
                                Ds.Tables.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return Ds;
            return JsonConvert.SerializeObject(Ds, Formatting.None);
        }



    }
}