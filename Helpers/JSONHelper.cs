using System.Web.Script.Serialization;
using System.Data;
using System.Collections.Generic;
using System;
using System.Text;

namespace ETradeAPI.Helpers
{
    public static class JSONHelper
    {
        #region Public extension methods.
        /// <summary>
        /// Extened method of object class
        /// Converts an object to a json string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(this object obj)
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                return serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                String error = ex.ToString();
                return "";
            }
        }

        /*
         {"SignupResult":[{"FirstName":"q","LastName":"q","TokenId":"070C53QK21Y00ZK0UHK8DVKOU0PZAY13","UserId":14,"Status":"0","EmailKeyValTemporary":"425527","TableName":"SignupResult"}]}
             */
        public static string GetJSONResult(string[,] data, string TableName)
        {
            StringBuilder result = new StringBuilder();
            if (TableName != null && TableName != "" && data != null && data.Length != 0)
            {
                result.Append("{\""+TableName + "\":[{");
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i,0]!=null)
                    {
                        result.Append("\"" + data[i,0] + "\":\"" + data[i,1] + "\"" + ((i + 1 < data.Length) ? "," : ""));
                    }
                }
                result.Append("}]}");
            }
            return result.ToString();
        }
        #endregion
    }
}