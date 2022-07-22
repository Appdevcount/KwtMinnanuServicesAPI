//using e24PaymentPipeLib;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Linq;

namespace KnetPayment
{
    //Prd
    public class ActivityHandler
    {
        public void LoggerCall<T>(Activity<T> activity, LogLevel LogLevel, Exception ex, string EToeknId, string AdditionalInfo, KnetPayment.ErrorAt ErrorAt, T PayReqOrResp)
        {
            StackTrace ST = new StackTrace();
            StackFrame SF = ST.GetFrame(1);
            MethodBase MB = SF.GetMethod();
            activity.CallingMethod = MB.Name;
            activity.LogLevel = LogLevel;
            activity.ex = ex;
            activity.TokenId = EToeknId;
            activity.AdditionalInfo = AdditionalInfo;
            activity.ErrorAt = ErrorAt;

            activity.ActivityToLog = PayReqOrResp;

            //activity.LogDestination = KnetPayment.LogDestination.Database;
            //activity.NotifyingMethod = KnetPayment.LogNotifyingMode.None;


            Log<T>(activity);
        }
        public void Log<T>(Activity<T> Activity)
        {
            try
            {
                bool AllowLog = Convert.ToBoolean(ConfigurationManager.AppSettings["AllowLog"].ToString());
                bool AllowLogNotifier = Convert.ToBoolean(ConfigurationManager.AppSettings["AllowLogNotifier"].ToString());
                String NLogDestination = ConfigurationManager.AppSettings["LogDestination"].ToString();
                String NLogNotifyingMode = ConfigurationManager.AppSettings["LogNotifyingMode"].ToString();
                string LogMode = (ConfigurationManager.AppSettings["LogMode"].ToString());
                string AppName = (ConfigurationManager.AppSettings["LoggingAppName"].ToString());

                if (AllowLog)
                {
                    string XMLString = (Activity.ActivityToLog != null) ? XMLSerialize(Activity.ActivityToLog) : "";

                    string ReqLog, RespLog;
                    Exception Ex;

                    ReqLog = (Activity.ActivityToLog != null & (Activity.ActivityType == ActivityType.ReqLog | Activity.ActivityType == ActivityType.ExtPay)) ?
                    XMLString : null;
                    RespLog = (Activity.ActivityToLog != null & Activity.ActivityType == ActivityType.RespLog) ?
                    XMLString : null;
                    Activity.Other = (Activity.ActivityToLog != null & Activity.ActivityType == ActivityType.Other) ?
                    XMLString : null;

                    Ex = (Activity.ErrorAt == ErrorAt.PaymentReq
                        | Activity.ErrorAt == ErrorAt.PaymentResp | Activity.ErrorAt == ErrorAt.ReceiptLookup) ? Activity.ex : null;

                    ExceptionTracker et = new ExceptionTracker();
                    string ErrMsgDet = Ex != null ? et.GetAllErrMessages(Ex) : null;

                    LogManager.Configuration.Variables["App"].Text = AppName;
                    LogManager.Configuration.Variables["ReqLog"].Text = ReqLog;
                    LogManager.Configuration.Variables["RespLog"].Text = RespLog;
                    LogManager.Configuration.Variables["Other"].Text = Activity.Other;
                    LogManager.Configuration.Variables["ErrorAt"].Text = Activity.ErrorAt.ToString();
                    LogManager.Configuration.Variables["ExceptionDetails"].Text = ErrMsgDet; //ExceptionDetails;
                    LogManager.Configuration.Variables["AdditionalInfo"].Text = Activity.AdditionalInfo;// AdditionalInfo;
                    LogManager.Configuration.Variables["TokenId"].Text = Activity.TokenId; // TokenId;

                    LogManager.Configuration.Variables["CustomLogger"].Text = Activity.CallingMethod;

                    string ReqTime = (Activity.ActivityType == ActivityType.ReqLog | Activity.ActivityType == ActivityType.ExtPay) ? DateTime.Now.ToString() : null;
                    string RespTime = (Activity.ActivityType == ActivityType.RespLog | Activity.ActivityType == ActivityType.Other) ? DateTime.Now.ToString() : null;


                    LogManager.Configuration.Variables["ReqTime"].Text = ReqTime;
                    LogManager.Configuration.Variables["RespTime"].Text = RespTime;


                    NLog.Logger NLoggerD = NLog.LogManager.GetLogger(NLogDestination);

                    //LogDestination and LogNotifyingMode passed in Activity object has been overidden here 


                    if (LogMode == "Manual")
                    {

                        logging l = new logging();

                        string URL = HttpContext.Current.Request.Url.PathAndQuery;
                        string ExceptionType = Ex != null ? Ex.GetType().ToString() : null;// Ex.GetType().ToString();
                        string stackTrace = Ex != null ? Ex.StackTrace : null;
                        string InnerExceptionMessage = Ex != null ? Ex.InnerException.Message : null;
                        string ExMessage = Ex != null ? Ex.Message : null;



                        switch (Activity.LogLevel)
                        {
                            case LogLevel.Trace:
                                //NLoggerD.Info(Activity.AdditionalInfo);
                                l.Nlogmanual(Activity.TokenId, Enum.GetName(typeof(LogLevel), LogLevel.Info), "KnetPayment.ActivityHandler.Log", ExceptionType,
                                   stackTrace, InnerExceptionMessage, ExMessage,
                                     ReqTime, RespTime, GetHostIPAddress(), GetClientIPAddress(), URL, Activity.CallingMethod, AppName,
                                    ReqLog, RespLog, Activity.Other, Activity.ErrorAt.ToString(), ErrMsgDet, Activity.AdditionalInfo);
                                break;
                            case LogLevel.Debug:
                                //NLoggerD.Debug(Activity.AdditionalInfo);
                                l.Nlogmanual(Activity.TokenId, Enum.GetName(typeof(LogLevel), LogLevel.Debug), "KnetPayment.ActivityHandler.Log", ExceptionType,
                                  stackTrace, InnerExceptionMessage, ExMessage,
                                    ReqTime, RespTime, GetHostIPAddress(), GetClientIPAddress(), URL, Activity.CallingMethod, AppName,
                                   ReqLog, RespLog, Activity.Other, Activity.ErrorAt.ToString(), ErrMsgDet, Activity.AdditionalInfo);
                                break;
                            case LogLevel.Info:
                                //NLoggerD.Info(Activity.AdditionalInfo);
                                l.Nlogmanual(Activity.TokenId, Enum.GetName(typeof(LogLevel), LogLevel.Info), "KnetPayment.ActivityHandler.Log", ExceptionType,
                                   stackTrace, InnerExceptionMessage, ExMessage,
                                     ReqTime, RespTime, GetHostIPAddress(), GetClientIPAddress(), URL, Activity.CallingMethod, AppName,
                                    ReqLog, RespLog, Activity.Other, Activity.ErrorAt.ToString(), ErrMsgDet, Activity.AdditionalInfo);
                                break;
                            case LogLevel.Warn:
                                //NLoggerD.Warn(Activity.AdditionalInfo);
                                l.Nlogmanual(Activity.TokenId, Enum.GetName(typeof(LogLevel), LogLevel.Warn), "KnetPayment.ActivityHandler.Log", ExceptionType,
                                  stackTrace, InnerExceptionMessage, ExMessage,
                                    ReqTime, RespTime, GetHostIPAddress(), GetClientIPAddress(), URL, Activity.CallingMethod, AppName,
                                   ReqLog, RespLog, Activity.Other, Activity.ErrorAt.ToString(), ErrMsgDet, Activity.AdditionalInfo);
                                break;
                            case LogLevel.Error:
                                //NLoggerD.Error(Activity.ex, Activity.AdditionalInfo);
                                l.Nlogmanual(Activity.TokenId, Enum.GetName(typeof(LogLevel), LogLevel.Error), "KnetPayment.ActivityHandler.Log", ExceptionType,
                                   stackTrace, InnerExceptionMessage, ExMessage,
                                     ReqTime, RespTime, GetHostIPAddress(), GetClientIPAddress(), URL, Activity.CallingMethod, AppName,
                                    ReqLog, RespLog, Activity.Other, Activity.ErrorAt.ToString(), ErrMsgDet, Activity.AdditionalInfo);
                                break;
                            case LogLevel.Fatal:
                                //NLoggerD.Fatal(Activity.AdditionalInfo);
                                l.Nlogmanual(Activity.TokenId, Enum.GetName(typeof(LogLevel), LogLevel.Fatal), "KnetPayment.ActivityHandler.Log", ExceptionType,
                                    stackTrace, InnerExceptionMessage, ExMessage,
                                      ReqTime, RespTime, GetHostIPAddress(), GetClientIPAddress(), URL, Activity.CallingMethod, AppName,
                                     ReqLog, RespLog, Activity.Other, Activity.ErrorAt.ToString(), ErrMsgDet, Activity.AdditionalInfo);
                                break;

                        }
                    }
                    else if (LogMode == "NLog")
                    {
                        switch (Activity.LogLevel)
                        {
                            case LogLevel.Trace:
                                NLoggerD.Info(Activity.AdditionalInfo);

                                break;
                            case LogLevel.Debug:
                                NLoggerD.Debug(Activity.AdditionalInfo);
                                break;
                            case LogLevel.Info:
                                NLoggerD.Info(Activity.AdditionalInfo);
                                break;
                            case LogLevel.Warn:
                                NLoggerD.Warn(Activity.AdditionalInfo);
                                break;
                            case LogLevel.Error:
                                NLoggerD.Error(Activity.ex, Activity.AdditionalInfo);
                                break;
                            case LogLevel.Fatal:
                                NLoggerD.Fatal(Activity.AdditionalInfo);
                                break;

                        }
                    }
                    //FileLogger Test
                    //NLog.Logger NLoggerFL = NLog.LogManager.GetLogger("FileLogger");
                    //NLoggerFL.Info(Activity.ex, Activity.AdditionalInfo + " , Exception : " + ErrMsgDet + " , " + " {ReqRespParam}", Activity.ActivityToLog);

                    //EventLogger Test
                    //NLog.Logger NLoggerEL = NLog.LogManager.GetLogger("EventLogger");
                    //NLoggerEL.Info(Activity.ex, Activity.AdditionalInfo +" , Exception : "+ ErrMsgDet+" , "+ " {ReqRespParam}", Activity.ActivityToLog);


                    if (AllowLogNotifier)//Not implemented
                    {
                        LogNotifier(NLogNotifyingMode, Activity.AdditionalInfo);

                    }

                    //NLog.LogManager.Shutdown();
                }
            }
            catch (Exception ex)
            {
                //NLog.Logger NLoggerD = NLog.LogManager.GetLogger("FileLogger");
                //This is just for logging, ignore if there is any issue and proceed
                //NLog.Logger NLoggerFL = NLog.LogManager.GetLogger("FileLogger");
                //NLoggerFL.Error(ex, Activity.AdditionalInfo  + " {ReqRespParam}", Activity.ActivityToLog);

            }
            finally
            {
                //LogManager.Flush();
                //LogManager.Shutdown();
            }
        }

        public string XMLSerialize<T>(T Obj)
        {
            StringWriter sw = new StringWriter();
            XmlSerializer s = new XmlSerializer(Obj.GetType());
            s.Serialize(sw, Obj);
            return sw.ToString();
        }

        public void LogNotifier(string NLogNotifyingMode, string Message)
        {

        }
        public string GetHostIPAddress()
        {
            //string strHostName = System.Net.Dns.GetHostName();
            ////IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName()); <-- Obsolete
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(strHostName);
            //IPAddress ipAddress = ipHostInfo.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);//.AddressList[0];

            IPAddress ipAddress = Dns.GetHostAddresses(Dns.GetHostName()).Where(address =>
                                address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First();

            return ipAddress.ToString();
        }
        public string GetClientIPAddress()
        {
            if (HttpContext.Current.Request.Headers["CF-CONNECTING-IP"] != null)
                return HttpContext.Current.Request.Headers["CF-CONNECTING-IP"].ToString();

            string strIpAddress;
            strIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (strIpAddress == null)
            {
                strIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return strIpAddress;
        }


    }

    public class Activity<T>
    {
        public string TokenId { get; set; }
        public string PaymentId { get; set; }
        public T ActivityToLog { get; set; }
        public ActivityType ActivityType { get; set; }
        public ErrorAt ErrorAt { get; set; }
        public Exception ex { get; set; }
        public LogDestination LogDestination { get; set; }
        public LogNotifyingMode NotifyingMethod { get; set; }
        public string Other { get; set; }
        public string AdditionalInfo { get; set; }
        //public MsgCode MsgCode { get; set; }
        public LogLevel LogLevel { get; set; }
        public string CallingMethod { get; set; }

        // TokenId | ReqLog | RespLog | Other | ErrorAt | ReqTime | RespTime | level |  url | serverAddress | remoteAddress | callsite | type | stackTrace | ExceptionDetails
    }

    public class PayReq
    {
        public GCSPayReq GCSPayReq { get; set; }
        public KNETPayReq KNETPayReq { get; set; }
    }
    public class GCSPayReq
    {
        public string ReferenceId { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReferenceType { get; set; }
        public string Amount { get; set; }
        public string OLPaymentId { get; set; }
        public string PortalLoginId { get; set; }
        public string LogInPortId { get; set; }
        public string OrganizationId { get; set; }
        public string PaymentFor { get; set; }
        public string PaidByType { get; set; }
        public string lang { get; set; }
        public string ReceiptId { get; set; }
        public string KNETAccType { get; set; }
        public string BrPaymentTransactionId { get; set; }
        public string PostDate { get; set; }
        public string OLTransId { get; set; }
        public string UserId { get; set; }
        public string ReceiptNumber { get; set; }
        public string PaidByName { get; set; }
        public string PayeeMailId { get; set; }
        public string PayeeOrgMailId { get; set; }
        public string TrackId { get; set; }
        public string TranStopDateTime { get; set; }


    }
    public class KNETPayReq
    {
        public string ErrorMsg { get; set; }//{ get; }
        public string Result { get; set; }//{ get; }
        public string Auth { get; set; }//{ get; }
        public string ResponseUrl { get; set; }
        public string Avr { get; set; }//{ get; }
        public string Date { get; set; }//{ get; }
        public string Ref { get; set; }//{ get; }
        public string TransId { get; set; }
        public string TrackId { get; set; }
        public string KAction { get; set; }
        public string Amt { get; set; }
        public string Udf1 { get; set; }
        public string Udf2 { get; set; }
        public string Udf3 { get; set; }
        public string Udf4 { get; set; }
        public string Udf5 { get; set; }
        public string PaymentPage { get; set; }//{ get; }
        public string PaymentId { get; set; }
        public string RawResponse { get; set; }//{ get; }
        public string Currency { get; set; }
        public string ErrorUrl { get; set; }
        public string Language { get; set; }
        public string Timeout { get; set; }//{ set; }
        public string Alias { get; set; }//{ set; }
        public string ResourcePath { get; set; }//{ set; }

    }
    public class PayResp
    {
        public string paymentID { get; set; }
        public string result { get; set; }
        public string postdate { get; set; }
        public string tranid { get; set; }
        public string auth { get; set; }
        public string refn { get; set; }
        public string trackid { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
    }

    public class ExceptionTracker
    {
        public string GetAllErrMessages(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            for (Exception eCurrent = ex; eCurrent != null; eCurrent = eCurrent.InnerException)
            {
                sb.AppendLine(eCurrent.Message + "\n at " + eCurrent.Source + " , \n Type : " + eCurrent.GetType().Name + " , \n Trace: " + eCurrent.StackTrace);
            }
            return sb.ToString();
        }

    }
    public enum LogDestination
    {
        Database,
        File,
        EventLog,
        None
    }
    public enum LogNotifyingMode
    {
        Email,
        SMS,
        None
    }
    public enum ActivityType
    {
        ReqLog,
        RespLog,
        ExtPay,
        Other
    }
    public enum ErrorAt
    {
        PaymentReq,
        PaymentResp,
        ReceiptLookup,
        Other,
        None
    }
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
    public enum MsgCode
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
    public class InternalError
    {
        public string TokenId { get; set; }
        public string KNETPayID { get; set; }
        public string ErrorCode { get; set; }
        public string result { get; set; }
        public string postdate { get; set; }
        public string tranid { get; set; }
        public string refr { get; set; }
        public string trackid { get; set; }
        public string checkId { get; set; }
        public string Amount { get; set; }
        public string ReferenceNumber { get; set; }
        //public string tkId { get; set; }
        public string TempDecNo { get; set; }
        public string Cause { get; set; }


    }
    public class ExternalPay
    {
        public string TempReceiptNumber { get; set; }
        public string Mobile { get; set; }
        public string SecurityCode { get; set; }
        public string Email { get; set; }
    }

}

