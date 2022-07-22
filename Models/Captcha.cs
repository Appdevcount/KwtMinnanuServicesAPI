using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{
    public class Captcha : AdditionalRequestInfo
    {
        public int CaptchaId { get; set; }
        public string CaptchaImage { get; set; }
        public string CaptchaType { get; set; }
    }
}