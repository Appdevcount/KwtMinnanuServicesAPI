using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Runtime.CompilerServices;

[assembly: SuppressIldasm()]
[assembly: OwinStartup(typeof(ETradeAPI.Startup))]

namespace ETradeAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
