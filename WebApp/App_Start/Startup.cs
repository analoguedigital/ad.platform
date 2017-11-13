﻿using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Services.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using WebApi.Providers;
using Hangfire;

[assembly: OwinStartupAttribute(typeof(WebApi.Startup))]
namespace WebApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            AttachmentsRepository.RootFolderPath = HttpContext.Current.Server.MapPath("~/Attachments/");
            
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SurveyContext, LightMethods.Survey.Models.Migrations.Configuration>());
            AutoMapperConfig.Config();

            HangFireConfig.Config(app);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {

            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(SurveyContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new AuthorizationServerProvider()
            };

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }

    }
}
