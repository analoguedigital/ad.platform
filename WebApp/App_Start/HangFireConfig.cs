using Hangfire;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi
{
    public class HangFireConfig
    {
        public static void Config(IAppBuilder app)
        {
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("LightSurveys");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}