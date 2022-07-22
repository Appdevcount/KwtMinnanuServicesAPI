using System;
using System.Collections.Generic;

namespace ETradeAPI.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel : AdditionalRequestInfo
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel : AdditionalRequestInfo
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel : AdditionalRequestInfo
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel : AdditionalRequestInfo
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
