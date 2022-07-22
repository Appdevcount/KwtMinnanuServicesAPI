using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Tracing;
using NLog;
using System.Net.Http;
using System.Text;

using System.Text.RegularExpressions;

//using ETradeAPI.ErrorHelper;

namespace ETradeAPI.Helpers
{
    public class NLogger : ITraceWriter
    {
        #region Private member variables.  
        private static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<Dictionary<TraceLevel, Action<string>>> LoggingMap = new Lazy<Dictionary<TraceLevel, Action<string>>>(() => new Dictionary<TraceLevel, Action<string>> { { TraceLevel.Info, ClassLogger.Info }, { TraceLevel.Debug, ClassLogger.Debug }, { TraceLevel.Error, ClassLogger.Error }, { TraceLevel.Fatal, ClassLogger.Fatal }, { TraceLevel.Warn, ClassLogger.Warn } });
        #endregion

        #region Private properties.  
        /// <summary>  
        /// Get property for Logger  
        /// </summary>  
        private Dictionary<TraceLevel, Action<string>> Logger
        {
            get { return LoggingMap.Value; }
        }
        #endregion

        
        #region Public member methods.  
            /// <summary>  
            /// Implementation of TraceWriter to trace the logs.  
            /// </summary>  
            /// <param name="request"></param>  
            /// <param name="category"></param>  
            /// <param name="level"></param>  
            /// <param name="traceAction"></param>  
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            try
            {
                
                string actionName = JsonDataCrawler.GetANFromCategory(category);
                string controllerName = JsonDataCrawler.GetCNFromCategory(category);
                string ActionParams = "";
                string TokenId = JsonDataCrawler.GetTokenidFromBody(traceAction.Target.ToJSON());
                string UserId = JsonDataCrawler.GetUseridFromBody(traceAction.Target.ToJSON());
                string geoLocation = JsonDataCrawler.GetParameterFromBody("geoLocation", traceAction.Target.ToJSON());
                string deviceType = JsonDataCrawler.GetParameterFromBody("deviceType", traceAction.Target.ToJSON());
                string requestDatetime = JsonDataCrawler.GetParameterFromBody("requestDatetime", traceAction.Target.ToJSON());
                string ClientIp = ClientInfoFromRequest.GetIp(request);
                if (requestDatetime == string.Empty)
                    requestDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");

                if (level != TraceLevel.Off)
                {
                    string TraceActionTraget = traceAction.Target.ToJSON();
                    TraceActionTraget = Regex.Replace(TraceActionTraget, "\"browserIp\":null", "\"browserIp\":\"" + ClientIp + "\"");
                    TraceActionTraget = Regex.Replace(TraceActionTraget, "\"requestDatetime\":null", "\"requestDatetime\":\"" + requestDatetime + "\"");

                    if (traceAction != null && traceAction.Target != null)
                    {
                        category = category + (string.IsNullOrWhiteSpace(ClientIp) ? "" : "|" + ClientIp)
                                  + Environment.NewLine + "Action Parameters : " + JsonDataCrawler.CleanPwds(TraceActionTraget);

                    }
                    var record = new TraceRecord(request, category, level);
                    if (traceAction != null) traceAction(record);
                    Log(record, ActionParams, TokenId, ClientIp, UserId, actionName, controllerName, geoLocation, deviceType, requestDatetime);
                }
            }
            catch { }
        }
        #endregion

        #region Private member methods.  
        /// <summary>  
        /// Logs info/Error to Log file  
        /// </summary>  
        /// <param name="record"></param> 
        /// <param name="actionParams"></param>
        /// <param name="clientIp"></param>
        /// <param name="tokenId"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="userId"></param>
        private void Log(TraceRecord record, 
                         string actionParams = ""   , string tokenId = "", 
                         string clientIp = ""       , string userId="",
                         string actionName="", string controllerName="", string geoLocation = "", string deviceType = "", string requestDatetime = "")
        {
            if (requestDatetime == string.Empty)
                requestDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");

            try
            {
                var message = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(record.Message))
                    message.Append("").Append(record.Message + Environment.NewLine);

                if (record.Request != null)
                {

                    if (record.Request.Method != null)
                        message.Append("Method: " + record.Request.Method + Environment.NewLine);

                    if (record.Request.RequestUri != null)
                        message.Append("").Append("URL: " + record.Request.RequestUri + Environment.NewLine);
                }
                if (tokenId != null && tokenId != string.Empty)
                    message.Append("").Append("Token: " + tokenId + Environment.NewLine);

                if (!string.IsNullOrWhiteSpace(record.Category))
                    message.Append("").Append(record.Category);

                if (!string.IsNullOrWhiteSpace(record.Operator))
                    message.Append("").Append(record.Operator).Append(" ").Append(record.Operation);

                if (record.Exception != null && !string.IsNullOrWhiteSpace(record.Exception.GetBaseException().Message))
                {
                    var exceptionType = record.Exception.GetType();
                    message.Append(Environment.NewLine);
                    message.Append("").Append("Error: " + record.Exception.GetBaseException().Message + Environment.NewLine);
                }

                LogEventInfo theEvent = new LogEventInfo(GetLogLvl(record.Level), "ETradeAPI", Convert.ToString(message) + Environment.NewLine);
                //LogEventInfo theEvent = new LogEventInfo(LogLevel.Debug, "", Convert.ToString(message) + Environment.NewLine);
                theEvent.Properties["TokenId"] = tokenId;
                theEvent.Properties["browserIp"] = clientIp;
                theEvent.Properties["UserId"] = userId;
                theEvent.Properties["Controller"] = controllerName;
                theEvent.Properties["Action"] = actionName;
                //theEvent.Properties["record"] = record;
                theEvent.Properties["url"] = record.Request.RequestUri;
                theEvent.Properties["geoLocation"] = geoLocation;
                theEvent.Properties["deviceType"] = deviceType;
                theEvent.Properties["requestDatetime"] = requestDatetime;

                ClassLogger.Log(theEvent);

                #region Nlog Config help
                //Logger logger = LogManager.GetCurrentClassLogger();
                //LogEventInfo theEvent = new LogEventInfo(LogLevel.Debug, "", "Pass my custom value");
                //theEvent.Properties["MyValue"] = "My custom string";
                //theEvent.Properties["MyDateTimeValue"] = new DateTime(2015, 08, 30, 11, 26, 50);
                //theEvent.Properties["MyDateTimeValueWithCulture"] = new DateTime(2015, 08, 30, 11, 26, 50);
                //theEvent.Properties["MyDateTimeValueWithCultureAndFormat"] = new DateTime(2015, 08, 30, 11, 26, 50);
                //logger.Log(theEvent);
                /*
                 and in your NLog.config file:

                 ${event-properties:item=MyValue} -- renders "My custom string"
                 ${event-properties:MyDateTimeValue:format=yyyy-M-dd}"; -- renders "2015-8-30"
                 ${event-properties:MyDateTimeValueWithCulture:culture=en-US} -- renders "8/30/2015 11:26:50 AM"
                 ${event-properties:MyDateTimeValueWithCultureAndFormat:format=yyyy-M-dd HH:mm:ss:culture=en-US} -- renders "2015-8-30 11:26:50"
                 */

                #endregion
                // Logger[record.Level](Convert.ToString(message) + Environment.NewLine); // old one
            }
            catch { }
        }
        private NLog.LogLevel GetLogLvl(TraceLevel lvl)
        {

            switch (lvl)
            {
                case TraceLevel.Debug: return LogLevel.Debug;
                case TraceLevel.Error: return LogLevel.Error;
                case TraceLevel.Fatal: return LogLevel.Fatal;
                case TraceLevel.Info: return LogLevel.Info;
                case TraceLevel.Warn: return LogLevel.Warn;
                case TraceLevel.Off: return LogLevel.Off;
                default: return LogLevel.Info;
            }
        }
        #endregion



    }
    
}