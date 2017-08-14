using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Services.Identity;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using LightMethods.Survey.Models.Entities;

namespace WebApi
{

    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        HttpContext HttpContext { get { return HttpContext.Current; } }

        public ApplicationUserManager UserManager
        {
            get
            {
                return ServiceContext.UserManager;
            }
        }

        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            if (context.IsTokenEndpoint && context.Request.Method == "OPTIONS")
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "content-type", "timezoneoffset" });
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "*" });
                context.RequestCompleted();
                return Task.FromResult(0);
            }

            return base.MatchEndpoint(context);
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var result = await UserManager.FindAsync(context.UserName, context.Password);
            if (result == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (result is OrgUser)
            {
                if (OrgUserHasAccess(result as OrgUser))
                    await GenerateUserIdentity(context, result as OrgUser);
                else
                {
                    context.SetError("invalid_grant", "You do not have access to this software.");
                    return;
                }
            }

            await GenerateUserIdentity(context, result);
        }

        private async Task GenerateUserIdentity(OAuthGrantResourceOwnerCredentialsContext context, User user)
        {
            var identity = await user.GenerateUserIdentityAsync(UserManager);
            identity.AddClaim(new Claim("email", user.Email));
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);
        }

        private bool OrgUserHasAccess(OrgUser user)
        {
            Uri referer = this.HttpContext.Request.UrlReferrer;
            if (referer != null)
            {
                if (referer.Host == this.HttpContext.Request.Url.Host)
                    return user.IsWebUser;

                return user.IsMobileUser;
            }

            return user.IsMobileUser;
        }
    }
}