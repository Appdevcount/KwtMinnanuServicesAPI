using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace ETradeAPI.Helpers
{
    public static class ClientInfoFromRequest
    {
        public static string GetIp()
        {

            return GetClientIp();
        }
        public static string GetIp(HttpRequestMessage request)
        {
            return GetClientIp(request);
        }
        private static string GetClientIp(HttpRequestMessage request = null)
        {
            if (request != null)
            {
                string IP = request.GetClientIpAddress();
                if (IP != null && IP != "")
                {
                    object property;
                    request.Properties.TryGetValue(typeof(RemoteEndpointMessageProperty).FullName, out property);
                    RemoteEndpointMessageProperty remoteProperty = property as RemoteEndpointMessageProperty;
                    return IP;
                }
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }
    }
}