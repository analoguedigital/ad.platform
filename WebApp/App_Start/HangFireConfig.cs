using Hangfire;
using Owin;

namespace WebApi
{
    public class HangFireConfig
    {
        public static void Config(IAppBuilder app)
        {
            string everyMinute = "0 * * ? * *";
            string everyHour = "0 0 * ? * *";

            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("LightSurveys");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            var emailService = new WebApi.Services.EmailService();
            RecurringJob.AddOrUpdate("process-emails", () => emailService.ProcessEmails(), cronExpression: everyMinute);

            var uploadsCleanupService = new WebApi.Services.UploadsCleanupService();
            RecurringJob.AddOrUpdate("cleanup-uploads-dir", () => uploadsCleanupService.CleanupUploadsDirectory(), cronExpression: everyHour);

            //var accountDataExportsCleanupService = new WebApi.Services.AccountDataExportsCleanupService();
            //RecurringJob.AddOrUpdate("cleanup-account-data-exports", () => accountDataExportsCleanupService.CleanupAccountDataExports(), cronExpression: everyMinute);
        }
    }
}