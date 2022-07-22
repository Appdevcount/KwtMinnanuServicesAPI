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

 using ETradeAPI.Models;


namespace ETradeAPI
{
    public class GCSKnetDatabase
    {
        //private static String connectionStr = String.Empty;       

        public GCSKnetDatabase()
        {
            //connectionStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public static ReceiptAction IsReceiptValid(GCSReqObj ro)// (string ReceiptNumber, string ReferenceNumber, string Mobile, string Email, string SecurityCode)//GCSReqObj GCSReqObj
        {
           
            ReceiptAction ra = new ReceiptAction();
            VerifyReceiptDetailsforGCSSite Vrfr = null;
            //GCSReqObj ro = new GCSReqObj();
            try
            {
                //ro = new GCSReqObj { CommonData = ReceiptNumber, CommonData1 = ReferenceNumber, CommonData2 = Mobile, CommonData3 = Email, CommonData4 = SecurityCode };
                 Vrfr = VerifyReceiptDetailsforGCSSite(ro.CommonData,ro.CommonData1,ro.CommonData2,ro.CommonData3,ro.CommonData4);

                if (Vrfr != null)
                {

                
                ra.VerifyReceiptDetailsforGCSSite = Vrfr;

                ReceiptDetailsMinified rd = null;
                string TokenId = "0";
                    if (Vrfr.Proceed)
                    {
                        System.Data.DataSet ds = new System.Data.DataSet();
                        //ro = new GCSReqObj { CommonData = ReceiptNumber, CommonData1 = ReferenceNumber, CommonData2 = Mobile, CommonData3 = Email, CommonData4 = SecurityCode };

                        DataSet dsrd = GCSKnetDatabase.GetPaymentDetailsforGCSSite(ro.CommonData, ro.CommonData1, ro.CommonData2, ro.CommonData3, ro.CommonData5, true);
                        if (dsrd.Tables.Count > 0 && dsrd.Tables[0].Rows.Count>0)
                        {
                            dsrd.Tables[0].Rows[0]["Amount"] = Convert.ToDecimal(dsrd.Tables[0].Rows[0]["Amount"]);// + Convert.ToDecimal(0.210);

                            rd = GCSKnetDatabase.BindData<ReceiptDetailsMinified>(dsrd.Tables[0]);
                            //RandomStringGenerator4DotNet is installed to get random text -- To inject false string in token to send pseudo token to client side
                            RandomStringGenerator.StringGenerator RSG = new RandomStringGenerator.StringGenerator() { MinNumericChars = 1, MinLowerCaseChars = 2, MinUpperCaseChars = 1 };
                            string randstr = RSG.GenerateString(5);
                            rd.TokenId = "PWKNETTUID" + randstr + rd.TokenId;// Unique identifier for the token generated from Payment wizard . This identity text need to be checked and taken out with substring , but need to pass complete token to KNET, again NEW response page need to handle this token and substringout but need to pass additional identifier in sp for autosubmit

                            //rd.TokenId = "PWKNETTUID" + rd.TokenId;// Unique identifier for the token generated from Payment wizard . This identity text need to be checked and taken out with substring , but need to pass complete token to KNET, again NEW response page need to handle this token and substringout but need to pass additional identifier in sp for autosubmit
                            if (rd.TokenId.Contains("+"))
                            {

                            }

                            rd.TokenId = HttpUtility.UrlEncode(rd.TokenId);

                        }
                        ra.ReceiptDetailsMinified = rd;
                    }
                    //else
                    //{
                    //    CommonFunctions.LogUserActivity("IsReceiptValid", "", "", "", "", "no tables");

                    //    Vrfr = new VerifyReceiptDetailsforGCSSite() { Proceed = false, Message = "-1" };// Resources.Resource.somethingwentwrong };//"Some error has occured . Please Contact IT Team" };

                    //    ra.VerifyReceiptDetailsforGCSSite = Vrfr;
                    //}
                    ra.VerifyReceiptDetailsforGCSSite = Vrfr;
                }
                else
                {
                    CommonFunctions.LogUserActivity("IsReceiptValid", "", "", "", "", "no receipt");

                    Vrfr = new VerifyReceiptDetailsforGCSSite() { Proceed = false, Message = "-1" };// Resources.Resource.somethingwentwrong };//"Some error has occured . Please Contact IT Team" };

                    ra.VerifyReceiptDetailsforGCSSite = Vrfr;
                }
                return ra;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("IsReceiptValid exception", "", "", "", "", ex.Message.ToString());
                VerifyReceiptDetailsforGCSSite VerifyReceiptDetailsforGCSSite = new VerifyReceiptDetailsforGCSSite() { Proceed = false, Message = "-1" };// Resources.Resource.somethingwentwrong };//"Some error has occured . Please Contact IT Team" };

                ra.VerifyReceiptDetailsforGCSSite = VerifyReceiptDetailsforGCSSite;
                return ra;
            }

        }


        #region GetPaymentDetailsforGCSSite
        public static VerifyReceiptDetailsforGCSSite VerifyReceiptDetailsforGCSSite(String TempReceiptNumber="",string ReferenceNumber = "", string Mobile = "", string CustEmail = "", string SecurityCode = "")
        {
            VerifyReceiptDetailsforGCSSite VGCSRD = null;
            try
            {
                String conString = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            SqlConnection CheckReceiptconnection = new SqlConnection(conString);

            SqlCommand CheckReceiptcommand = new SqlCommand("VerifyReceiptDetailsforGCSSite", CheckReceiptconnection);
            CheckReceiptcommand.CommandType = CommandType.StoredProcedure;

            SqlParameter paramTempReceiptNumber = CheckReceiptcommand.Parameters.Add("@TempReceiptNumber", SqlDbType.VarChar, 100);
            paramTempReceiptNumber.Value = TempReceiptNumber;
            SqlParameter paramReferenceNumber = CheckReceiptcommand.Parameters.Add("@FormReferenceNumber", SqlDbType.VarChar, 100);
            paramReferenceNumber.Value = ReferenceNumber;
            SqlParameter paramMobile = CheckReceiptcommand.Parameters.Add("@Mobile", SqlDbType.VarChar, 100);
            paramMobile.Value = Mobile;
            SqlParameter paramCustEmail = CheckReceiptcommand.Parameters.Add("@CustEmail", SqlDbType.VarChar, 100);
            paramCustEmail.Value = CustEmail;
            SqlParameter paramSecurityCode = CheckReceiptcommand.Parameters.Add("@SecurityCode", SqlDbType.VarChar, 100);
            paramSecurityCode.Value = SecurityCode;

            SqlParameter paramCheck1 = CheckReceiptcommand.Parameters.Add("@ReceiptValid", SqlDbType.Bit);
            paramCheck1.Direction = ParameterDirection.Output;
            paramCheck1.Value = 0;
            SqlParameter paramCheck2 = CheckReceiptcommand.Parameters.Add("@PaymentTried", SqlDbType.Bit);
            paramCheck2.Direction = ParameterDirection.Output;
            paramCheck2.Value = 0;
            SqlParameter paramCheck3 = CheckReceiptcommand.Parameters.Add("@PaymentTriedStatus", SqlDbType.VarChar, 100);
            paramCheck3.Direction = ParameterDirection.Output;
            paramCheck3.Value = "";
            SqlParameter paramCheck4 = CheckReceiptcommand.Parameters.Add("@OPDetailId", SqlDbType.Int);
            paramCheck4.Direction = ParameterDirection.Output;
            paramCheck4.Value = 0;
            SqlParameter paramCheck5 = CheckReceiptcommand.Parameters.Add("@Proceed", SqlDbType.Bit);
            paramCheck5.Direction = ParameterDirection.Output;
            paramCheck5.Value = 0;
            SqlParameter paramCheck6 = CheckReceiptcommand.Parameters.Add("@TranSttDateTime", SqlDbType.DateTime);
            paramCheck6.Direction = ParameterDirection.Output;
            paramCheck6.Value = DateTime.Now;
            SqlParameter paramCheck7 = CheckReceiptcommand.Parameters.Add("@Message", SqlDbType.VarChar, 100);
            paramCheck7.Direction = ParameterDirection.Output;
            paramCheck7.Value = "";
            SqlParameter paramCheck8 = CheckReceiptcommand.Parameters.Add("@MessageCode", SqlDbType.Int);
            paramCheck8.Direction = ParameterDirection.Output;
            paramCheck8.Value = 0;

            CheckReceiptconnection.Open();
            CheckReceiptcommand.ExecuteNonQuery();
            //string oppp = (CheckReceiptcommand.Parameters["@Message"].Value).ToString() +
            //    (CheckReceiptcommand.Parameters["@OPDetailId"].Value).ToString() +
            //      (CheckReceiptcommand.Parameters["@PaymentTried"].Value).ToString() +
            //      (CheckReceiptcommand.Parameters["@PaymentTriedStatus"].Value).ToString() +
            //       (CheckReceiptcommand.Parameters["@Proceed"].Value).ToString() +
            //       (CheckReceiptcommand.Parameters["@ReceiptValid"].Value).ToString() +
            //       (CheckReceiptcommand.Parameters["@TranSttDateTime"].Value).ToString();

             VGCSRD = new VerifyReceiptDetailsforGCSSite()
            {
                Message = CheckReceiptcommand.Parameters["@MessageCode"].Value.ToString(),// VerificationMessageFromMessageCode((int)(CheckReceiptcommand.Parameters["@MessageCode"].Value)),// (string)(CheckReceiptcommand.Parameters["@Message"].Value),
                MessageCode = (int)(CheckReceiptcommand.Parameters["@MessageCode"].Value),
                OPDetailId = (int)(CheckReceiptcommand.Parameters["@OPDetailId"].Value),
                PaymentTried = (bool)(CheckReceiptcommand.Parameters["@PaymentTried"].Value),
                PaymentTriedStatus = (string)(CheckReceiptcommand.Parameters["@PaymentTriedStatus"].Value is DBNull ? "Payment Not Tried" : CheckReceiptcommand.Parameters["@PaymentTriedStatus"].Value),//This is handled in the SP also
                Proceed = (bool)(CheckReceiptcommand.Parameters["@Proceed"].Value),
                ReceiptValid = (bool)(CheckReceiptcommand.Parameters["@ReceiptValid"].Value),
                TranSttDateTime = (DateTime)(CheckReceiptcommand.Parameters["@TranSttDateTime"].Value)
            };
            CheckReceiptconnection.Close();

            return VGCSRD;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("VerifyReceiptDetailsforGCSSite", "", "", "", "", ex.Message.ToString());
                return VGCSRD;
            }
        }
        public static DataSet GetPaymentDetailsforGCSSite(String TempReceiptNumber = "", string ReferenceNumber = "", string Mobile = "", string CustEmail = "",string lang="eng", bool PaymentInitCall=true)
        {
            DataSet paymentDetails = new DataSet();
            try { 
            //String lang =  HttpContext.Current.Session["lang"]!=null ? HttpContext.Current.Session["lang"].ToString() : "eng";

           

            String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

            SqlConnection conection = new SqlConnection(connectStr);
            SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter("GetOnlinePaymentDetailsGCSReceiptsforGCSSite", conection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("@TempReceiptNumber", TempReceiptNumber);
            adapter.SelectCommand.Parameters.AddWithValue("@ReferenceNumberParam", ReferenceNumber);
            adapter.SelectCommand.Parameters.AddWithValue("@Mobile", Mobile);
            adapter.SelectCommand.Parameters.AddWithValue("@CustEmail", CustEmail);
            adapter.SelectCommand.Parameters.AddWithValue("@PaymentInitCall", PaymentInitCall);
            adapter.SelectCommand.Parameters.AddWithValue("@lang", lang);


            bool AddEPaymentFee = Convert.ToBoolean( System.Configuration.ConfigurationManager.AppSettings["AddEPaymentFee"]);
            string EPaymentFee = System.Configuration.ConfigurationManager.AppSettings["EPaymentFee"];
            Decimal EPaymentFeeValue = 0.000m;
            Decimal.TryParse(EPaymentFee, out EPaymentFeeValue);

            adapter.SelectCommand.Parameters.AddWithValue("@AddPaymentFee", AddEPaymentFee);
            adapter.SelectCommand.Parameters.AddWithValue("@PaymentFee", EPaymentFeeValue);

                adapter.SelectCommand.Parameters.AddWithValue("@AppSource", "eService");
                adapter.SelectCommand.Parameters.AddWithValue("@ReceiptAutoSubmitFromPaymentWizard", 1);

                adapter.Fill(paymentDetails);

            return paymentDetails;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("GetPaymentDetailsforGCSSite", "", "", "", "", ex.Message.ToString());
                return paymentDetails;
            }
        }
        //Using reflection to construct C# object from datatable
        public static T BindData<T>(DataTable dt)
        {
            DataRow dr = dt.Rows[0];

            List<string> columns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName);
            }

            var ob = Activator.CreateInstance<T>();

            var fields = typeof(T).GetFields();
            foreach (var fieldInfo in fields)
            {
                if (columns.Contains(fieldInfo.Name))
                {
                    fieldInfo.SetValue(ob, dr[fieldInfo.Name]);
                }
            }

            var properties = typeof(T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (columns.Contains(propertyInfo.Name))
                {
                    // Fill the data into the property
                    //Below line is to avoid exception for case - 'Object of type 'System.DBNull' cannot be converted to type 'System.Nullable`1[System.Decimal]'.'

                    var propval = dr[propertyInfo.Name] == DBNull.Value ? null : dr[propertyInfo.Name];
                    //logging LWS = new logging();
                    //Encrypting the the Token(bigint) from dataset and assigning the encrypted string to TokenId property
                    propval = propertyInfo.Name == "TokenId" ? Encrypt(propval.ToString()) : propval;
                    //if(propertyInfo.Name == "Amount")
                    //{
                    //    Convert.ToDecimal(propval) + 0.210
                    //}
                    //Below line to retain decimal value as string (18,3).. Normal decimal property round off it and removes unneccesary trailing zeroes
                    propval = propertyInfo.Name == "Amount" ? propval.ToString() : propval;
                    propertyInfo.SetValue(ob, propval, null);
                }
            }

            return ob;
        }
        #endregion GetPaymentDetailsforGCSSite


        //  public static string VerificationMessageFromMessageCode(int MessageCode)
        //         {
        //             String ThreadLang = HttpContext.Current.Session["lang"].ToString() + " - " + Thread.CurrentThread.CurrentCulture.ToString();
        //             switch (MessageCode)
        //             {
        //                 case 1:
        //                     return Resources.Resource.somethingwentwrong;
        //                 case 2:
        //                     return Resources.Resource.somethingwentwrong;
        //                 case 3:
        //                     return Resources.Resource.somethingwentwrong;
        //                 case 4:
        //                     return Resources.Resource.somethingwentwrong;
        //                 case 5:
        //                     return Resources.Resource.somethingwentwrong;
        //                 case 13:
        //                     return Resources.Resource.somethingwentwrong;
        //                 case 11:
        //                     return Resources.Resource.somethingwentwrong;
        //                 default:

        //                     return Resources.Resource.somethingwentwrong;
        //             }

        //         }

        // public void ReceiptAutoSubmitOnPaymentSuccess(String PaymentId, String error, String response, String TranStatus, String BrPaymentTransactionId, Int64 ReferenceId,
        //      String TransId, DateTime TranStpDateTime,
        //      String Result, String PostDate, String AuthByBank, String RefByBank, String ReferenceNumber,
        //     String ReferenceType, String ReceiptId, String PaymentType, string ETokenId)
        // {
        //     String RcptNo = String.Empty;
        //     String str = "ReceiptAutoSubmitOnPaymentSuccess ";
        //     //try
        //     {
        //         String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
        //         using (SqlConnection connect = new SqlConnection(connectStr))
        //         {
        //             using (SqlCommand command = new SqlCommand("ReceiptAutoSubmitOnPaymentSuccess", connect))
        //             {
        //                 command.CommandType = CommandType.StoredProcedure;

        //                 command.Parameters.Add("@PaymentId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(PaymentId) ? 0 : Int64.Parse(PaymentId);
        //                 command.Parameters.Add("@error", SqlDbType.VarChar).Value = error;
        //                 command.Parameters.Add("@response", SqlDbType.VarChar).Value = response;
        //                 command.Parameters.Add("@TranStatus", SqlDbType.VarChar).Value = TranStatus;
        //                 command.Parameters.Add("@TranStpDateTime", SqlDbType.DateTime).Value = TranStpDateTime;
        //                 command.Parameters.Add("@TransId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(TransId) ? 0 : Int64.Parse(TransId);


        //                 command.Parameters.Add("@Result", SqlDbType.VarChar).Value = Result;
        //                 // amended as Post Date is showing only MMYY.
        //                 command.Parameters.Add("@PostDate", SqlDbType.DateTime).Value = TranStpDateTime;
        //                 command.Parameters.Add("@AuthByBank", SqlDbType.VarChar).Value = AuthByBank;
        //                 command.Parameters.Add("@RefByBank", SqlDbType.VarChar).Value = RefByBank;
        //                 command.Parameters.Add("@PaymentFor", SqlDbType.VarChar).Value = PaymentType;

        //                 // Where
        //                 command.Parameters.Add("@BrPaymentTransactionId", SqlDbType.Int).Value = String.IsNullOrEmpty(BrPaymentTransactionId) ? 0 : Int32.Parse(BrPaymentTransactionId);
        //                 command.Parameters.Add("@ReferenceId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ReferenceId.ToString()) ? 0 : ReferenceId;


        //                 command.Parameters.Add("@ReceiptId", SqlDbType.Int).Value = String.IsNullOrEmpty(ReceiptId) ? 0 : Int32.Parse(ReceiptId);

        //                 command.Parameters.Add("@RcptNum", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
        //                 //if (ETokenId == "") ETokenId = "0";
        //                 command.Parameters.Add("@ETokenId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ETokenId) ? 0 : Int64.Parse(ETokenId);

        //                 foreach (SqlParameter sp in command.Parameters)
        //                 {
        //                     sp.Value = sp.Value == null ? "" : sp.Value;
        //                     str += sp.ParameterName + "=" + sp.Value + "\t ,";
        //                 }

        //                 connect.Open();

        //                 int successQuery = command.ExecuteNonQuery();

        //                 activityRS.ActivityType = ActivityType.RespLog;
        //                 if (successQuery > 0)
        //                 {
        //                     if (command.Parameters["@RcptNum"].Value != null)
        //                     {
        //                         RcptNo = command.Parameters["@RcptNum"].Value.ToString();
        //                         SaveLog("ReceiptAutoSubmitOnPaymentSuccess", "Rcecipt No. => " + RcptNo + "\n Parameters are => " + str, EventLogEntryType.Information);
        //                         if (DBActivityLog)
        //                             AH.LoggerCall<PayResp>(activityRS, LogLevel.Trace, null, ETokenId, "Success - SP-EXEC ReceiptAutoSubmitOnPaymentSuccess on Payment Success , Parameters are => " + str, ErrorAt.None, Prs);
        //                     }
        //                 }
        //                 else
        //                 {
        //                     if (DBActivityLog)
        //                         AH.LoggerCall<PayResp>(activityRS, LogLevel.Warn, null, ETokenId, "Failure - SP-EXEC ReceiptAutoSubmitOnPaymentSuccess on Payment Success , Parameters are => " + str, ErrorAt.PaymentResp, Prs);
        //                     SaveLog("ReceiptAutoSubmitOnPaymentSuccess", " Error in Update Payment details Parameters are => " + str, EventLogEntryType.Error);
        //                 }

        //             }
        //         }
        //     }
        //     //catch (Exception ex)
        //     //{
        //     //    SaveLog("UpdatePaymentDetailsReceiptsKnet-SP-GCS", " Error => " + ex.ToString() + "\n Parameters are => " + str, EventLogEntryType.Error);
        //     //}
        // }

        #region DeCrypt

        public static string ExplicitDecryptTokenCall(string TokenId)
        {
            try { 
            string DeccryptedToken = Decrypt(TokenId);
            return DeccryptedToken;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("ExplicitDecryptTokenCall", "", "", "", "", ex.Message.ToString());
                return "";
            }
        }

        private static string Decrypt(string cipherText)
        {
            string EncryptionKey = getPrivateKey(); //"MAKقصV2ضSPBعهخحNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText); //Encoding.Unicode.GetBytes(cipherText); 
            using (Rijndael encryptor = Rijndael.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        //cs.Close();
                        cs.FlushFinalBlock();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #endregion DeCrypt

        #region get Private Key
        private static String getPrivateKey()
        {
            String privateKey = "";
            //try
            {
                String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
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
            return privateKey;
        }
        #endregion get Private Key
        public static string Encrypt(string clearText)
        {
            try { 
            string EncryptionKey = getPrivateKey();// dsConfig.Tables[0].Rows[0]["configvalue"].ToString();
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
            catch (Exception ex)
            {
                CommonFunctions.LogUserActivity("Encrypt", "", "", "", "", ex.Message.ToString());
                return "";
            }
        }

    }


}