using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
namespace ETradeAPI.Helpers
{
    public static class JsonDataCrawler
    {
        internal static string GetTokenidFromBody(string sJsonParams)
        {
            string Tokenid = string.Empty;
            try
            {
                Match M = Regex.Match(sJsonParams, "tokenId\"?\\s*:\\s*\"([^\\\"]+)\"", RegexOptions.IgnoreCase);

                if (M.Success)
                {
                    Tokenid = M.Groups[1].Value.ToString();
                }
                if (System.Web.HttpContext.Current.Request.Form["tokenId"] != null)
                {
                    Tokenid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["tokenId"]);
                }
                
            }
            catch { }
            return Tokenid;
        }

        internal static string GetUseridFromBody(string sJsonParams)
        {
            string Tokenid = string.Empty;
            try
            {
                Match M = Regex.Match(sJsonParams, "muserid\"?\\s*:\\s*\"([^\\\"]+)\"", RegexOptions.IgnoreCase);

                if (M.Success)
                {
                    Tokenid = M.Groups[1].Value.ToString();
                }
                else if (System.Web.HttpContext.Current.Request.Form["mUserid"] !=null)
                {
                    Tokenid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["mUserid"]);
                }
            }
            catch { }
            return Tokenid;
        }

        internal static string GetANFromCategory(string category)
        {
            //Controller : " + controllerName + Environment.NewLine + "Action : 
            string Tokenid = string.Empty;
            try
            {
                Match M = Regex.Match(category, "Action : ([^\n\r]*)", RegexOptions.IgnoreCase);

                if (M.Success)
                {
                    Tokenid = M.Groups[1].Value.ToString();
                }
            }
            catch { }
            return Tokenid;
        }

        internal static string GetCNFromCategory(string category)
        {
            string Tokenid = string.Empty;
            try
            {                
                Match M = Regex.Match(category, "Controller : ([^\n\r]*)", RegexOptions.IgnoreCase);

                if (M.Success)
                {
                    Tokenid = M.Groups[1].Value.ToString();
                }
            }
            catch { }
            return Tokenid;
        }

        internal static string CleanPwds(string v)
        {
            try
            {
                return Regex.Replace(v, "(\"(pwd|pass(word)?)\"\\s*:\\s*\")([^\"]+)(\")", "$1***$5");
            }
            catch { return v; }
        }

        internal static string GetParameterFromBody(string paramName, string sJsonParams)
        {
            string Tokenid = string.Empty;
            try
            {
                Match M = Regex.Match(sJsonParams, paramName + "\"?\\s*:\\s*\"([^\\\"]+)\"", RegexOptions.IgnoreCase);

                if (M.Success)
                {
                    Tokenid = M.Groups[1].Value.ToString();
                }
                else if (System.Web.HttpContext.Current.Request.Form[paramName] != null)
                {
                    Tokenid = Convert.ToString(System.Web.HttpContext.Current.Request.Form[paramName]);
                }
            }
            catch { }
            return Tokenid;
        }
    }
}