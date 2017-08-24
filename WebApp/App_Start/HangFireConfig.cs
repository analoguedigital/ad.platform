﻿using Hangfire;
using Owin;

namespace WebApi
{
    public class HangFireConfig
    {
        public static void Config(IAppBuilder app)
        {
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("LightSurveys");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            var emailService = new WebApi.Services.EmailService();
            RecurringJob.AddOrUpdate("process-emails", () => emailService.ProcessEmails(), Cron.MinuteInterval(1));
        }
    }
}