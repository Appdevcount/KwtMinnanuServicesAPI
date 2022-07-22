using System;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Configuration;

using System.Security.Cryptography;

using System.Text;
using System.IO;
using System.Web;
using System.Threading;

namespace KnetPayment
{
    public class logging
    {
        private EventLog myLog;

        bool DBActivityLog = Convert.ToBoolean(ConfigurationManager.AppSettings["DBActivityLog"].ToString());
        public logging()
        {
            myLog = new EventLog("MyGCSNewLog");
        }

        ActivityHandler AH = new ActivityHandler();
        Activity<PayReq> activityRQ = new Activity<PayReq>();
        Activity<PayResp> activityRS = new Activity<PayResp>();
        PayResp Prs = new PayResp();
        PayReq Prq = new PayReq();

        #region Initialize Payment

        public void InitializePaymentDetail(KnetVariables k)
        {
            InitializePaymentDetails
                (k.ReferenceId.ToString(), k.TranStatus,
                 k.TranSttDateTime, k.ClientIPAddress,
                 k.SessionId, k.PortalLoginId, k.LogInPortId.ToString(),
                 k.Amount.ToString(), k.OrganizationId, k.PaymentFor,
                 k.ReferenceNumber, k.ReferenceType, k.CheckId.ToString(), k.PaidByType, k.ErrorMsg, k.response,
                 k.ReceiptId, k.PaymentId, k.TrackId, "0", "Mobile", "Email");
        }

        public bool InitializePaymentDetails
            (String ReferenceId, String TranStatus,
             DateTime TranSttDateTime, String ClientIPAddress,
             String SessionId, String PortalLoginId, String LogInPortId,
             String Amount, String OrganizationId, String PaymentFor,
             String ReferenceNumber, String ReferenceType, String BrPaymentTransactionId, char PaidByType,
            String error, String response, String ReceiptId, String PaymentId, String TrackId, string ETokenId, string MobileNum, string CustEmail)//, Int64 tokenId)
        {

            bool success = false;

            success = initializePaymentDetails(ReferenceId, TranStatus, TranSttDateTime,
                ClientIPAddress, SessionId, PortalLoginId, LogInPortId,
                 Amount, OrganizationId, PaymentFor, ReferenceNumber, ReferenceType, BrPaymentTransactionId,
                 PaidByType, error, response, ReceiptId, PaymentId, TrackId, ETokenId, MobileNum, CustEmail);

            return success;
        }


        private bool initializePaymentDetails
            (String ReferenceId, String TranStatus,
             DateTime TranSttDateTime, String ClientIPAddress,
             String SessionId, String PortalLoginId, String LogInPortId,
             String Amount, String OrganizationId, String PaymentFor,
             String ReferenceNumber, String ReferenceType, String BrPaymentTransactionId, char PaidByType, String error, String response,
             String ReceiptId, String PaymentId, String TrackId, string ETokenId, string MobileNum, string CustEmail)//, Int64 tokenId)
        {

            bool success = false;
            String str = "";
            // //try
            {

                String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
                using (SqlConnection connect = new SqlConnection(connectStr))
                {
                    using (SqlCommand command = new SqlCommand("LogInitialPaymentDetailsGCSReceiptsKnet", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        //command.Parameters.Add("@TrackId", SqlDbType.VarChar).Value = TrackId;
                        command.Parameters.Add("@ReferenceId", SqlDbType.BigInt).Value = Int64.Parse(ReferenceId);
                        command.Parameters.Add("@TranStatus", SqlDbType.VarChar).Value = TranStatus;
                        command.Parameters.Add("@TranSttDateTime", SqlDbType.DateTime).Value = TranSttDateTime;
                        command.Parameters.Add("@ClientIPAddress", SqlDbType.VarChar).Value = ClientIPAddress;
                        command.Parameters.Add("@SessionId", SqlDbType.VarChar).Value = SessionId;
                        command.Parameters.Add("@PortalLoginId", SqlDbType.VarChar).Value = PortalLoginId;
                        command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Decimal.Parse(Amount);
                        command.Parameters.Add("@OrganizationId", SqlDbType.Int).Value = Int32.Parse(OrganizationId);
                        command.Parameters.Add("@PaymentFor", SqlDbType.VarChar).Value = PaymentFor;
                        command.Parameters.Add("@ReferenceNumber", SqlDbType.VarChar).Value = ReferenceNumber;
                        command.Parameters.Add("@ReferenceType", SqlDbType.VarChar).Value = ReferenceType;
                        command.Parameters.Add("@BrPaymentTransactionId", SqlDbType.Int).Value = Int32.Parse(BrPaymentTransactionId);
                        command.Parameters.Add("@PaidByType", SqlDbType.Char).Value = PaidByType;
                        command.Parameters.Add("@LogInPortId", SqlDbType.Int).Value = Int32.Parse(LogInPortId);
                        command.Parameters.Add("@error", SqlDbType.VarChar).Value = error;
                        command.Parameters.Add("@response", SqlDbType.VarChar).Value = response;
                        command.Parameters.Add("@ReceiptId", SqlDbType.VarChar).Value = ReceiptId;
                        //command.Parameters.Add("@OLPaymentId", SqlDbType.VarChar).Value = PaymentId;

                        Int64 PaymentIdInteger; //There can be case with string value for [track id issue] => ERROR - CGW000185-Track ID Invalid, so chekcing if its integer and assign
                        bool res = Int64.TryParse(PaymentId, out PaymentIdInteger);
                        //if (!res)
                        //{
                        //    PaymentId = "0";
                        //}
                        PaymentId = res ? PaymentId : "0";
                        command.Parameters.Add("@OLPaymentId", SqlDbType.BigInt).Value = Int64.Parse(PaymentId);

                        command.Parameters.Add("@OLTransId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(TrackId) ? 0 : Int64.Parse(TrackId);


                        command.Parameters.Add("@ETokenId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ETokenId) ? 0 : Int64.Parse(ETokenId);

                        command.Parameters.Add("@MobileNum", SqlDbType.VarChar).Value = MobileNum;


                        command.Parameters.Add("@CustEmail", SqlDbType.VarChar).Value = CustEmail;


                        foreach (SqlParameter sp in command.Parameters)
                        {
                            sp.Value = sp.Value == null ? "" : sp.Value;
                            str += sp.ParameterName + "=" + sp.Value + "\t";
                        }

                        //command.Parameters.Add("@tokenId", SqlDbType.Int).Value = tokenId;

                        connect.Open();
                        int successQuery = command.ExecuteNonQuery();

                        activityRQ.ActivityType = ActivityType.ReqLog;
                        if (successQuery > 0)
                        {
                            success = true;
                            if (DBActivityLog)
                                AH.LoggerCall<PayReq>(activityRQ, LogLevel.Trace, null, ETokenId, "Success - SP-EXEC LogInitialPaymentDetailsGCSReceiptsKnet , Parameters are => " + str, ErrorAt.None, Prq);

                        }
                        else
                        {
                            if (DBActivityLog)
                                AH.LoggerCall<PayReq>(activityRQ, LogLevel.Warn, null, ETokenId, "Failure - SP-EXEC LogInitialPaymentDetailsGCSReceiptsKnet , Parameters are => " + str, ErrorAt.PaymentReq, Prq);

                            SaveLog("LogInitialPaymentDetailsReceiptsKnet-SP-GCS", " Error in LogInit SP, Parameters are => " + str, EventLogEntryType.Error);
                        }
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("InitializePaymentDetails-SPGCS", " Error => " + ex.ToString() + "\n Parameters are => " + str, EventLogEntryType.Error);
            //}
            return success;
        }

        #endregion Initialize Payment

        #region Update Payment Success

        public String UpdatePaymentDetails
            (String PaymentId, String error, String response, String TranStatus, String BrPaymentTransactionId, Int64 DeclarationId,
             String TransId, DateTime TranStpDateTime,
             String Result, String PostDate, String AuthByBank, String RefByBank, String ReferenceNumber,
            String ReferenceType, String ReceiptId, String PaymentType, string ETokenId)
        {
            String RcptNo = String.Empty;


            //SaveLog("UpdatePaymentDetails-SP","P " +  PaymentId + "E " + error + "R " + response + "T " + TranStatus +
            //    "C " + CheckId + "D " + DeclarationId.ToString() + "Tr " + TransId +  "Re " + Result + "Po " + PostDate + 
            //    "Au " + AuthByBank + "Ref  " + RefByBank + "Tem " + TempDecNo + "Pay " + PayID + "Cu " +  CustomsDuty +
            //    "Ha " + HandlingCharges + "St "  + Storage + "Pene " + Penalties +  "Oth " + Others + "Cert " + Certificates +
            //    "Pri" + Printing + "Gu " + Guarantees         , EventLogEntryType.Information);

            RcptNo = updatePaymentDetails(PaymentId, error, response, TranStatus, BrPaymentTransactionId,
                   DeclarationId, TransId, TranStpDateTime, Result, PostDate, AuthByBank,
                   RefByBank, ReferenceNumber, ReferenceType, ReceiptId, PaymentType, ETokenId);

            return RcptNo;
        }

        private String updatePaymentDetails
            (String PaymentId, String error, String response, String TranStatus, String BrPaymentTransactionId, Int64 ReferenceId,
             String TransId, DateTime TranStpDateTime,
             String Result, String PostDate, String AuthByBank, String RefByBank, String ReferenceNumber,
            String ReferenceType, String ReceiptId, String PaymentType, string ETokenId)
        {
            String RcptNo = String.Empty;
            String str = "UpdatePaymentDetailsGCSReceiptsKnet ";
            //try
            {
                String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
                using (SqlConnection connect = new SqlConnection(connectStr))
                {
                    using (SqlCommand command = new SqlCommand("UpdatePaymentDetailsGCSReceiptsKnet", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@PaymentId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(PaymentId) ? 0 : Int64.Parse(PaymentId);
                        command.Parameters.Add("@error", SqlDbType.VarChar).Value = error;
                        command.Parameters.Add("@response", SqlDbType.VarChar).Value = response;
                        command.Parameters.Add("@TranStatus", SqlDbType.VarChar).Value = TranStatus;
                        command.Parameters.Add("@TranStpDateTime", SqlDbType.DateTime).Value = TranStpDateTime;
                        command.Parameters.Add("@TransId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(TransId) ? 0 : Int64.Parse(TransId);


                        command.Parameters.Add("@Result", SqlDbType.VarChar).Value = Result;
                        // amended as Post Date is showing only MMYY.
                        command.Parameters.Add("@PostDate", SqlDbType.DateTime).Value = TranStpDateTime;
                        command.Parameters.Add("@AuthByBank", SqlDbType.VarChar).Value = AuthByBank;
                        command.Parameters.Add("@RefByBank", SqlDbType.VarChar).Value = RefByBank;
                        command.Parameters.Add("@PaymentFor", SqlDbType.VarChar).Value = PaymentType;

                        // Where
                        command.Parameters.Add("@BrPaymentTransactionId", SqlDbType.Int).Value = String.IsNullOrEmpty(BrPaymentTransactionId) ? 0 : Int32.Parse(BrPaymentTransactionId);
                        command.Parameters.Add("@ReferenceId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ReferenceId.ToString()) ? 0 : ReferenceId;


                        command.Parameters.Add("@ReceiptId", SqlDbType.Int).Value = String.IsNullOrEmpty(ReceiptId) ? 0 : Int32.Parse(ReceiptId);

                        command.Parameters.Add("@RcptNum", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                        //if (ETokenId == "") ETokenId = "0";
                        command.Parameters.Add("@ETokenId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ETokenId) ? 0 : Int64.Parse(ETokenId);

                        foreach (SqlParameter sp in command.Parameters)
                        {
                            sp.Value = sp.Value == null ? "" : sp.Value;
                            str += sp.ParameterName + "=" + sp.Value + "\t ,";
                        }

                        connect.Open();

                        int successQuery = command.ExecuteNonQuery();

                        activityRS.ActivityType = ActivityType.RespLog;
                        if (successQuery > 0)
                        {
                            if (command.Parameters["@RcptNum"].Value != null)
                            {
                                RcptNo = command.Parameters["@RcptNum"].Value.ToString();
                                SaveLog("UpdatePaymentDetails-SP-GCS", "Rcecipt No. => " + RcptNo + "\n Parameters are => " + str, EventLogEntryType.Information);
                                if (DBActivityLog)
                                    AH.LoggerCall<PayResp>(activityRS, LogLevel.Trace, null, ETokenId, "Success - SP-EXEC UpdatePaymentDetailsGCSReceiptsKnet on Payment Success , Parameters are => " + str, ErrorAt.None, Prs);
                            }
                        }
                        else
                        {
                            if (DBActivityLog)
                                AH.LoggerCall<PayResp>(activityRS, LogLevel.Warn, null, ETokenId, "Failure - SP-EXEC UpdatePaymentDetailsGCSReceiptsKnet on Payment Success , Parameters are => " + str, ErrorAt.PaymentResp, Prs);
                            SaveLog("UpdatePaymentDetailsReceiptsKnet-SP-GCS", " Error in Update Payment details Parameters are => " + str, EventLogEntryType.Error);
                        }

                    }
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("UpdatePaymentDetailsReceiptsKnet-SP-GCS", " Error => " + ex.ToString() + "\n Parameters are => " + str, EventLogEntryType.Error);
            //}
            return RcptNo;
        }
        #endregion Update Payment Success

        #region get Payment Details from Database
        public DataSet getPaymentDetails(String TokenId)
        {
            DataSet paymentDetails = new DataSet();
            //try
            {
                if (!String.IsNullOrEmpty(TokenId))
                {
                    paymentDetails = GetPaymentDetails(TokenId);
                }
                else
                {
                    SaveLog("getPaymentDetailsGCS", "TokenId Value received as Null or Empty , TokenId=" + TokenId.ToString(), EventLogEntryType.Error);
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("getPaymentDetailsGCS", "Error => " + ex.ToString(), EventLogEntryType.Error);
            //}

            return paymentDetails;
        }

        private DataSet GetPaymentDetails(String TokenId)
        {
            DataSet paymentDetails = new DataSet();
            //try
            {
                // String sTok = TokenId.Replace('-', '+').Replace('_', '/');


                Int64 tokenIdCheck1 = 0;
                Boolean IsInteger1 = Int64.TryParse(TokenId, out tokenIdCheck1);
                String DeccryptedToken = "";
                if (IsInteger1)
                {
                    DeccryptedToken = TokenId;
                }
                else
                    DeccryptedToken = Decrypt(TokenId);


                //String DeccryptedToken = TokenId;
                //String DeccryptedToken = "";
                //DeccryptedToken = "4155493223203870103";

                Int64 tokenIdCheck = 0;
                Boolean IsInteger = Int64.TryParse(DeccryptedToken, out tokenIdCheck);

                if (IsInteger)
                {
                    Int64 tokenIdLong = new Int64();
                    tokenIdLong = Convert.ToInt64(DeccryptedToken);

                    String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

                    SqlConnection conection = new SqlConnection(connectStr);
                    SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter("GetOnlinePaymentDetailsGCSReceiptsKnet", conection);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@TokenId", tokenIdLong);
                    adapter.Fill(paymentDetails);
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("GetPaymentDetailsGCS", "Error => " + ex.ToString(), EventLogEntryType.Error);

            //}
            return paymentDetails;
        }

        public void Nlogmanual(String TokenId, string ErrLevel, string Callsite, string ExceptionType, string stackTrace,
            string InnerExceptionDetail, string ExceptionMessage, string ReqTime, string RespTime, string ServerAddress,
            string RemoteAddress, string URL, string Logger, string App,
            string ReqLog, string RespLog, string Other, string errorAt, string
            ExceptionDetails, string AdditionalInfo)
        {

            String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

            SqlConnection conection = new SqlConnection(connectStr);
            conection.Open();
            SqlCommand CMD = new System.Data.SqlClient.SqlCommand("LogActivity", conection);
            CMD.CommandType = CommandType.StoredProcedure;

            CMD.Parameters.AddWithValue("@TokenId", TokenId);
            CMD.Parameters.AddWithValue("@ErrLevel", ErrLevel);
            CMD.Parameters.AddWithValue("@Callsite", Callsite);
            CMD.Parameters.AddWithValue("@ExceptionType", ExceptionType);
            CMD.Parameters.AddWithValue("@StackTrace", stackTrace);
            CMD.Parameters.AddWithValue("@InnerExceptionDetail", InnerExceptionDetail);
            CMD.Parameters.AddWithValue("@ExceptionMessage", ExceptionMessage);
            CMD.Parameters.AddWithValue("@ReqTime", ReqTime);
            CMD.Parameters.AddWithValue("@RespTime", RespTime);
            CMD.Parameters.AddWithValue("@ServerAddress", ServerAddress);
            CMD.Parameters.AddWithValue("@RemoteAddress", RemoteAddress);
            CMD.Parameters.AddWithValue("@URL", URL);
            CMD.Parameters.AddWithValue("@Logger", Logger);
            CMD.Parameters.AddWithValue("@App", App);
            CMD.Parameters.AddWithValue("@ReqLog", ReqLog);
            CMD.Parameters.AddWithValue("@RespLog", RespLog);
            CMD.Parameters.AddWithValue("@Other", Other);
            CMD.Parameters.AddWithValue("@ErrorAt", errorAt);
            CMD.Parameters.AddWithValue("@ExceptionDetails", ExceptionDetails);
            CMD.Parameters.AddWithValue("@AdditionalInfo", AdditionalInfo);

            CMD.ExecuteNonQuery();
            conection.Close();

        }



        #region break Down

        public DataSet getPaymentBreakDown(String PaymentId)
        {
            DataSet paymentBDD = new DataSet();
            //try
            {
                if (!String.IsNullOrEmpty(PaymentId))
                {
                    paymentBDD = GetPaymentBreakDownDetails(PaymentId);
                }
                else
                {
                    SaveLog("getPaymentBreakDownDetailsGCS", "PaymentId Value received as Null or Empty", EventLogEntryType.Error);
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("getPaymentBreakDownDetailsGCS", "Error => " + ex.ToString(), EventLogEntryType.Error);
            //}

            return paymentBDD;
        }

        private DataSet GetPaymentBreakDownDetails(String PaymentId)
        {
            DataSet PayBrkDownDet = new DataSet();
            //try
            {
                Int64 tokenIdCheck = 0;
                Boolean IsInteger = Int64.TryParse(PaymentId, out tokenIdCheck);

                if (IsInteger)
                {
                    Int64 tokenIdLong = new Int64();
                    tokenIdLong = Convert.ToInt64(PaymentId);

                    String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

                    SqlConnection conection = new SqlConnection(connectStr);
                    SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter("GetOnlinePaymentExtendDetails", conection);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@PaymentId", PaymentId);
                    adapter.Fill(PayBrkDownDet);
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("getPaymentBreakDownDetailsGCS", "Error => " + ex.ToString(), EventLogEntryType.Error);
            //}
            return PayBrkDownDet;
        }

        #endregion break Down

        #endregion get Payment Details from Database


        #region DeCrypt

        public string ExplicitDecryptTokenCall(string TokenId)
        {
            string DeccryptedToken = Decrypt(TokenId);
            return DeccryptedToken;
        }

        private string Decrypt(string cipherText)
        {

            //SaveLog("Decrypt", cipherText, EventLogEntryType.Information);

            string EncryptionKey = getPrivateKey(); //"MAKقصV2ضSPBعهخحNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText); //Encoding.Unicode.GetBytes(cipherText); 
            using (Rijndael encryptor = Rijndael.Create())
            {
                //encryptor.Padding = PaddingMode.PKCS7;

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
        private String getPrivateKey()
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
            //catch (Exception ex)
            //{
            //    SaveLog("privateKeyGCS", " Error => " + ex.ToString(), EventLogEntryType.Error);
            //}

            return privateKey;
        }
        #endregion get Private Key



        #region getPaymentstatus
        public String getPaymentStatus(int checkId)
        {
            String status = String.Empty;
            //try
            {
                String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
                using (SqlConnection connect = new SqlConnection(connectStr))
                {
                    using (SqlCommand command = new SqlCommand("getOnlinePaymentStatusbyCheckId", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        command.Parameters.Add("@checkId", SqlDbType.Int).Value = checkId;
                        command.Parameters.Add("@paymentStatus", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;

                        connect.Open();

                        command.ExecuteNonQuery();


                        if (command.Parameters["@paymentStatus"].Value != null)
                        {
                            status = command.Parameters["@paymentStatus"].Value.ToString();
                            //SaveLog("getPaymentStatus", status, EventLogEntryType.Information);
                        }

                    }
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("getPaymentStatusGCS", " Error => " + ex.ToString(), EventLogEntryType.Error);
            //}

            return status;
        }
        #endregion getPaymentstatus



        #region Update Payment Failed or Cancelled

        public void UpdatePaymentDetailsCanceledOrFailed
            (String PaymentId, String error, String response, String TranStatus, String BrPaymentTransactionId, Int64 ReferenceId,
             String TransId, DateTime TranStpDateTime,
             String Result, String PostDate, String AuthByBank, String RefByBank, String ReferenceNumber,
            String ReferenceType, String ReceiptId, String PaymentType, string ETokenId)
        {
            //SaveLog("UpdatePaymentDetails-SP","P " +  PaymentId + "E " + error + "R " + response + "T " + TranStatus +
            //    "C " + CheckId + "D " + DeclarationId.ToString() + "Tr " + TransId +  "Re " + Result + "Po " + PostDate + 
            //    "Au " + AuthByBank + "Ref  " + RefByBank + "Tem " + TempDecNo + "Pay " + PayID + "Cu " +  CustomsDuty +
            //    "Ha " + HandlingCharges + "St "  + Storage + "Pene " + Penalties +  "Oth " + Others + "Cert " + Certificates +
            //    "Pri" + Printing + "Gu " + Guarantees         , EventLogEntryType.Information);

            updatePaymentDetailsCanceledOrFailed(PaymentId, error, response, TranStatus, BrPaymentTransactionId,
                   ReferenceId, TransId, TranStpDateTime, Result, PostDate, AuthByBank,
                   RefByBank, ReferenceNumber, ReferenceType, ReceiptId, PaymentType, ETokenId);


        }

        private void updatePaymentDetailsCanceledOrFailed
            (String PaymentId, String error, String response, String TranStatus, String BrPaymentTransactionId, Int64 ReferenceId,
             String TransId, DateTime TranStpDateTime,
             String Result, String PostDate, String AuthByBank, String RefByBank, String ReferenceNumber,
            String ReferenceType, String ReceiptId, String PaymentType, string ETokenId)
        {

            String str = "UpdatePaymentDetailsGCSReceiptsKnetCanceledOrFailed ";
            //activityRS.ActivityType = ActivityType.RespLog;
            //try
            {
                String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
                using (SqlConnection connect = new SqlConnection(connectStr))
                {
                    using (SqlCommand command = new SqlCommand("UpdatePaymentDetailsGCSReceiptsKnetCanceledOrFailed", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@PaymentId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(PaymentId) ? 0 : Int64.Parse(PaymentId);
                        command.Parameters.Add("@error", SqlDbType.VarChar).Value = error;
                        command.Parameters.Add("@response", SqlDbType.VarChar).Value = response;
                        command.Parameters.Add("@TranStatus", SqlDbType.VarChar).Value = TranStatus;
                        command.Parameters.Add("@TranStpDateTime", SqlDbType.DateTime).Value = TranStpDateTime;
                        command.Parameters.Add("@TransId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(TransId) ? 0 : Int64.Parse(TransId);


                        command.Parameters.Add("@Result", SqlDbType.VarChar).Value = Result;
                        command.Parameters.Add("@PostDate", SqlDbType.DateTime).Value = TranStpDateTime;
                        command.Parameters.Add("@AuthByBank", SqlDbType.VarChar).Value = AuthByBank;
                        command.Parameters.Add("@RefByBank", SqlDbType.VarChar).Value = RefByBank;

                        // Where

                        command.Parameters.Add("@BrPaymentTransactionId", SqlDbType.Int).Value = String.IsNullOrEmpty(BrPaymentTransactionId) ? 0 : Int32.Parse(BrPaymentTransactionId);
                        command.Parameters.Add("@ReferenceId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ReferenceId.ToString()) ? 0 : ReferenceId;
                        //        command.Parameters.Add("@ReferenceNumber", SqlDbType.VarChar).Value = ReferenceNumber;
                        //        command.Parameters.Add("@ReferenceType", SqlDbType.VarChar).Value = ReferenceType;
                        command.Parameters.Add("@ReceiptId", SqlDbType.Int).Value = String.IsNullOrEmpty(ReceiptId) ? 0 : Int32.Parse(ReceiptId);
                        //command.Parameters.Add("@tokenId", SqlDbType.BigInt).Value = tokenId;

                        command.Parameters.Add("@ETokenId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ETokenId) ? 0 : Int64.Parse(ETokenId);

                        foreach (SqlParameter sp in command.Parameters)
                        {
                            sp.Value = sp.Value == null ? "" : sp.Value;
                            str += sp.ParameterName + "=" + sp.Value + "\t ,";
                        }

                        connect.Open();

                        int successQuery = command.ExecuteNonQuery();
                        activityRS.ActivityType = ActivityType.RespLog;
                        if (successQuery <= 0)
                        {
                            SaveLog("UpdatePaymentDetailsCanceledOrFailed-SPGCS", " Error occurred with Parameters are => " + str, EventLogEntryType.Error);
                            if (DBActivityLog)
                                AH.LoggerCall<PayResp>(activityRS, LogLevel.Warn, null, ETokenId, "Failure - SP-EXEC UpdatePaymentDetailsGCSReceiptsKnetCanceledOrFailed  , Parameters are => " + str, ErrorAt.PaymentResp, Prs);

                        }
                        else
                        {
                            if (DBActivityLog)
                                AH.LoggerCall<PayResp>(activityRS, LogLevel.Trace, null, ETokenId, "Success - SP-EXEC UpdatePaymentDetailsGCSReceiptsKnetCanceledOrFailed  , Parameters are => " + str, ErrorAt.None, Prs);

                        }

                    }
                }
            }
            //catch (Exception ex)
            //{
            //    AH.LoggerCall<PayResp>(activityRS, LogLevel.Warn, null, ETokenId, "Failure - SP-EXEC UpdatePaymentDetailsGCSReceiptsKnetCanceledOrFailed  , Parameters are => " + PaymentId+"  |  "+
            //        error + "  |  " +response + "  |  " +TranStatus + "  |  " +BrPaymentTransactionId + "  |  " +ReferenceId + "  |  " +
            //        Result + "  |  " +PostDate + "  |  " +AuthByBank + "  |  " +RefByBank + "  |  " +ReferenceNumber + "  |  " +ReferenceType + "  |  " +ReceiptId + "  |  " +PaymentType + "  |  " +ETokenId, ErrorAt.PaymentResp, Prs);
            //    //SaveLog("UpdatePaymentDetailsCanceledOrFailed-SPGCS", " Error => " + ex.ToString() + "\n Parameters are => " + str, EventLogEntryType.Error);
            //}

        }
        #endregion Update Payment Failed or Cancelled




        /// <summary>
        /// Used to Log events in Windows Event Viewer
        /// </summary>
        /// <param name="source">Define The Page or Method Name that Used to Log Event</param>
        /// <param name="Message">Define The Message Body that Will be Saved in Events File</param>
        /// <param name="MessageType">Information , Error, Audit</param>
        public void SaveLog(String source, String Message, EventLogEntryType MessageType)
        {

            bool EventLog = Convert.ToBoolean(ConfigurationManager.AppSettings["EventLog"].ToString());
            if (EventLog)
            {
                myLog.Source = source;
                myLog.WriteEntry("GCSK : " + Message, MessageType);
            }
        }

        public void ReceiptAutoSubmitOnPaymentSuccess(String PaymentId, String error, String response, String TranStatus, String BrPaymentTransactionId, Int64 ReferenceId,
             String TransId, DateTime TranStpDateTime,
             String Result, String PostDate, String AuthByBank, String RefByBank, String ReferenceNumber,
            String ReferenceType, String ReceiptId, String PaymentType, string ETokenId)
        {
            String RcptNo = String.Empty;
            String str = "ReceiptAutoSubmitOnPaymentSuccess ";
            //try
            {
                String connectStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
                using (SqlConnection connect = new SqlConnection(connectStr))
                {
                    using (SqlCommand command = new SqlCommand("ReceiptAutoSubmitOnPaymentSuccess", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@PaymentId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(PaymentId) ? 0 : Int64.Parse(PaymentId);
                        command.Parameters.Add("@error", SqlDbType.VarChar).Value = error;
                        command.Parameters.Add("@response", SqlDbType.VarChar).Value = response;
                        command.Parameters.Add("@TranStatus", SqlDbType.VarChar).Value = TranStatus;
                        command.Parameters.Add("@TranStpDateTime", SqlDbType.DateTime).Value = TranStpDateTime;
                        command.Parameters.Add("@TransId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(TransId) ? 0 : Int64.Parse(TransId);


                        command.Parameters.Add("@Result", SqlDbType.VarChar).Value = Result;
                        // amended as Post Date is showing only MMYY.
                        command.Parameters.Add("@PostDate", SqlDbType.DateTime).Value = TranStpDateTime;
                        command.Parameters.Add("@AuthByBank", SqlDbType.VarChar).Value = AuthByBank;
                        command.Parameters.Add("@RefByBank", SqlDbType.VarChar).Value = RefByBank;
                        command.Parameters.Add("@PaymentFor", SqlDbType.VarChar).Value = PaymentType;

                        // Where
                        command.Parameters.Add("@BrPaymentTransactionId", SqlDbType.Int).Value = String.IsNullOrEmpty(BrPaymentTransactionId) ? 0 : Int32.Parse(BrPaymentTransactionId);
                        command.Parameters.Add("@ReferenceId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ReferenceId.ToString()) ? 0 : ReferenceId;


                        command.Parameters.Add("@ReceiptId", SqlDbType.Int).Value = String.IsNullOrEmpty(ReceiptId) ? 0 : Int32.Parse(ReceiptId);

                        command.Parameters.Add("@RcptNum", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                        //if (ETokenId == "") ETokenId = "0";
                        command.Parameters.Add("@ETokenId", SqlDbType.BigInt).Value = String.IsNullOrEmpty(ETokenId) ? 0 : Int64.Parse(ETokenId);

                        foreach (SqlParameter sp in command.Parameters)
                        {
                            sp.Value = sp.Value == null ? "" : sp.Value;
                            str += sp.ParameterName + "=" + sp.Value + "\t ,";
                        }

                        connect.Open();

                        int successQuery = command.ExecuteNonQuery();

                        activityRS.ActivityType = ActivityType.RespLog;
                        if (successQuery > 0)
                        {
                            if (command.Parameters["@RcptNum"].Value != null)
                            {
                                RcptNo = command.Parameters["@RcptNum"].Value.ToString();
                                SaveLog("ReceiptAutoSubmitOnPaymentSuccess", "Rcecipt No. => " + RcptNo + "\n Parameters are => " + str, EventLogEntryType.Information);
                                if (DBActivityLog)
                                    AH.LoggerCall<PayResp>(activityRS, LogLevel.Trace, null, ETokenId, "Success - SP-EXEC ReceiptAutoSubmitOnPaymentSuccess on Payment Success , Parameters are => " + str, ErrorAt.None, Prs);
                            }
                        }
                        else
                        {
                            if (DBActivityLog)
                                AH.LoggerCall<PayResp>(activityRS, LogLevel.Warn, null, ETokenId, "Failure - SP-EXEC ReceiptAutoSubmitOnPaymentSuccess on Payment Success , Parameters are => " + str, ErrorAt.PaymentResp, Prs);
                            SaveLog("ReceiptAutoSubmitOnPaymentSuccess", " Error in Update Payment details Parameters are => " + str, EventLogEntryType.Error);
                        }

                    }
                }
            }
            //catch (Exception ex)
            //{
            //    SaveLog("UpdatePaymentDetailsReceiptsKnet-SP-GCS", " Error => " + ex.ToString() + "\n Parameters are => " + str, EventLogEntryType.Error);
            //}
        }



        #region VerifyOnlinePaymentDetailsGCSReceiptsKnet
        public bool VerifyOnlinePaymentDetailsGCSReceiptsKnet(string ETokenId, string PaymentId, string Amount)
        {
            ETokenId = string.IsNullOrEmpty(ETokenId) ? "0" : ETokenId;
            PaymentId = string.IsNullOrEmpty(PaymentId) ? "0" : PaymentId;
            Amount = string.IsNullOrEmpty(Amount) ? "0" : Amount;

            String conString = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            SqlConnection cn = new SqlConnection(conString);

            SqlCommand cmd = new SqlCommand("VerifyOnlinePaymentDetailsGCSReceiptsKnet", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter PETokenId = cmd.Parameters.Add("@TokenId", SqlDbType.BigInt, 30);
            PETokenId.Value = ETokenId;

            SqlParameter PPaymentId = cmd.Parameters.Add("@PaymentId", SqlDbType.BigInt);
            PPaymentId.Value = PaymentId;

            SqlParameter PAmount = cmd.Parameters.Add("@Amount", SqlDbType.Decimal);
            PAmount.Value = Amount;

            SqlParameter PValid = cmd.Parameters.Add("@Valid", SqlDbType.Bit);
            PValid.Direction = ParameterDirection.Output;
            PValid.Value = false;

            cn.Open();
            cmd.ExecuteNonQuery();

            bool Valid = (bool)(cmd.Parameters["@Valid"].Value);
            cn.Close();

            return Valid;
        }
        #endregion VerifyOnlinePaymentDetailsGCSReceiptsKnet

        #region GetPaymentDetailsforGCSSite
        public VerifyReceiptDetailsforGCSSite VerifyReceiptDetailsforGCSSite(String TempReceiptNumber,string ReferenceNumber, string Mobile, string CustEmail, string SecurityCode)
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

            VerifyReceiptDetailsforGCSSite VGCSRD = new VerifyReceiptDetailsforGCSSite()
            {
                Message = VerificationMessageFromMessageCode((int)(CheckReceiptcommand.Parameters["@MessageCode"].Value)),// (string)(CheckReceiptcommand.Parameters["@Message"].Value),
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
        public DataSet GetPaymentDetailsforGCSSite(String TempReceiptNumber,string ReferenceNumber, string Mobile, string CustEmail, bool PaymentInitCall)
        {
            String lang =  HttpContext.Current.Session["lang"]!=null ? HttpContext.Current.Session["lang"].ToString() : "eng";

            DataSet paymentDetails = new DataSet();

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
            adapter.Fill(paymentDetails);

            return paymentDetails;
        }
        #endregion GetPaymentDetailsforGCSSite

        public string Encrypt(string clearText)
        {
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

        public string VerificationMessageFromMessageCode(int MessageCode)
        {
            String ThreadLang = HttpContext.Current.Session["lang"].ToString() + " - " + Thread.CurrentThread.CurrentCulture.ToString();
            switch (MessageCode)
            {
                case 1:
                    return Resources.Resource.somethingwentwrong;
                case 2:
                    return Resources.Resource.somethingwentwrong;
                case 3:
                    return Resources.Resource.somethingwentwrong;
                case 4:
                    return Resources.Resource.somethingwentwrong;
                case 5:
                    return Resources.Resource.somethingwentwrong;
                case 13:
                    return Resources.Resource.somethingwentwrong;
                case 11:
                    return Resources.Resource.somethingwentwrong;
                default:

                    return Resources.Resource.somethingwentwrong;
            }

        }

        public PaymentWizardTokenExtractResult PaymentWizardTokenExtract(string PWKNETTUIDToken)
        {
            PaymentWizardTokenExtractResult PaymentWizardTokenExtractResult = new  PaymentWizardTokenExtractResult();
            if (PWKNETTUIDToken.Contains("PWKNETTUID"))
            {
                PaymentWizardTokenExtractResult.Token = PWKNETTUIDToken.Substring(10, PWKNETTUIDToken.Length - 10); PaymentWizardTokenExtractResult.PWRequest = true ;
                return PaymentWizardTokenExtractResult;
            }
            else
            {
                PaymentWizardTokenExtractResult.Token = PWKNETTUIDToken; PaymentWizardTokenExtractResult.PWRequest = false;
                return PaymentWizardTokenExtractResult;
            }
        }
        public class PaymentWizardTokenExtractResult
        {
            public string Token { get; set; }
            public bool PWRequest { get; set; }
        }


    }
    public class VerifyReceiptDetailsforGCSSite
    {
        public bool ReceiptValid { get; set; }
        public bool PaymentTried { get; set; }
        public string PaymentTriedStatus { get; set; }
        public int OPDetailId { get; set; }
        public bool Proceed { get; set; }
        public DateTime TranSttDateTime { get; set; }
        public string Message { get; set; }
        public int MessageCode { get; set; }
        //public string Token { get; set; }
    }

    public class ReceiptDetails
    {
        public int ReferenceId { get; set; } //(int, null)
        public string ReferenceNumber { get; set; } //(varchar(30), null)
        public string ReferenceType { get; set; } //(char(1), null)
        public string Amount { get; set; } //(decimal(18,3), null)
        public Int64? OLPaymentId { get; set; } //(varchar(7), not null)
        public string PortalLoginId { get; set; } //(varchar(7), not null)
        public int? LogInPortId { get; set; } //(varchar(7), not null)
        public int OrganizationId { get; set; } //(int, null)
        public string PaymentFor { get; set; } //(varchar(1), not null)
        public string PaidByType { get; set; } //(varchar(1), not null)
        public string lang { get; set; } //(varchar(3), not null)
        public int? ReceiptId { get; set; } //(int, not null)
        public string KNETAccType { get; set; } //(varchar(9), not null)
        public int BrPaymentTransactionId { get; set; } //(varchar(1), not null)
        public DateTime PostDate { get; set; } //(varchar(7), not null)
        public int? OLTransId { get; set; } //(varchar(7), not null)
        public string UserId { get; set; } //(varchar(30), not null)
        public string ReceiptNumber { get; set; } //(varchar(20), null)
        public string PaidByName { get; set; } //(varchar(30), not null)
        public string PayeeMailId { get; set; } //(varchar(7), not null)
        public string PayeeOrgMailId { get; set; } //(varchar(7), not null)
        public int? TrackId { get; set; } //(varchar(7), not null))
        public DateTime TranStopDateTime { get; set; } //(datetime, null)
        public DateTime ReceiptDate { get; set; } //(smalldatetime, null)
        public decimal? GCSAmount { get; set; } //(decimal(18,3), null)
        public decimal? KGACAmount { get; set; } //(decimal(18,3), null)
        public decimal? Balance { get; set; } //(decimal(18,3), null)
        public string TempReceiptNumber { get; set; } //(varchar(50), null)
        public string TokenId { get; set; } //(varchar(50), null)
        public string PayeeName { get; set; } //(varchar(50), null)
        public string Mobile { get; set; } //(varchar(50), null)
        public string CustEmail { get; set; } //(varchar(50), null)
        public DateTime TokenExpTime { get; set; }
    }
    public class ReceiptAction
    {
        public VerifyReceiptDetailsforGCSSite VerifyReceiptDetailsforGCSSite { get; set; }

        public ReceiptDetailsMinified ReceiptDetailsMinified { get; set; }
    }
    public class ReceiptDetailsMinified
    {
        //public int ReferenceId { get; set; } //(int, null)
        public string ReferenceNumber { get; set; } //(varchar(30), null)
                                                    //public string ReferenceType { get; set; } //(char(1), null)
        public string Amount { get; set; } //(decimal(18,3), null)
                                           //public decimal Amount { get { return val} set { value = Math.Round(value, 3); } } //(decimal(18,3), null)
                                           //public Int64? OLPaymentId { get; set; } //(varchar(7), not null)
                                           //public string PortalLoginId { get; set; } //(varchar(7), not null)
                                           //public int? LogInPortId { get; set; } //(varchar(7), not null)
                                           //public int OrganizationId { get; set; } //(int, null)
                                           //public string PaymentFor { get; set; } //(varchar(1), not null)
                                           //public string PaidByType { get; set; } //(varchar(1), not null)
                                           //public string lang { get; set; } //(varchar(3), not null)
                                           //public int? ReceiptId { get; set; } //(int, not null)
                                           //public string KNETAccType { get; set; } //(varchar(9), not null)
                                           //public int BrPaymentTransactionId { get; set; } //(varchar(1), not null)
                                           //public DateTime PostDate { get; set; } //(varchar(7), not null)
                                           //public int? OLTransId { get; set; } //(varchar(7), not null)
                                           //public string UserId { get; set; } //(varchar(30), not null)
                                           //public string ReceiptNumber { get; set; } //(varchar(20), null)
                                           //public string PaidByName { get; set; } //(varchar(30), not null)
                                           //public string PayeeMailId { get; set; } //(varchar(7), not null)
                                           //public string PayeeOrgMailId { get; set; } //(varchar(7), not null)
                                           //public int? TrackId { get; set; } //(varchar(7), not null))
                                           //public DateTime TranStopDateTime { get; set; } //(datetime, null)
                                           //public DateTime ReceiptDate { get; set; } //(smalldatetime, null)
                                           //public decimal? GCSAmount { get; set; } //(decimal(18,3), null)
                                           //public decimal? KGACAmount { get; set; } //(decimal(18,3), null)
                                           //public decimal? Balance { get; set; } //(decimal(18,3), null)
                                           //public string TempReceiptNumber { get; set; } //(varchar(50), null)
        public string TokenId { get; set; } //(varchar(50), null)
        public string PayeeName { get; set; } //(varchar(50), null)
        public string PaidByName { get; set; } //(varchar(50), null)
        public string UserId { get; set; } //(varchar(50), null)
        public string Mobile { get; set; } //(varchar(50), null)
        public string CustEmail { get; set; } //(varchar(50), null)

    }

}