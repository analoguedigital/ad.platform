using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services.Identity;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace WebApi
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        HttpContext HttpContext { get { return HttpContext.Current; } }

        public ApplicationUserManager UserManager
        {
            get { return ServiceContext.UserManager; }
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

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            return Task.FromResult(context.Validated());
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // enable CORS and allow all requests. this should be constrained in live production.
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var user = await UserManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (user is OrgUser)
            {
                var orgUser = user as OrgUser;
                if (OrgUserHasAccess(orgUser))
                {
                    if (!orgUser.EmailConfirmed)
                    {
                        context.SetError("email_not_verified", "You haven't confirmed your email address yet.");
                        return;
                    }

                    // if we get here, current org user has access. sign in.
                    await GenerateUserIdentity(context, orgUser);
                }
                else
                {
                    context.SetError("invalid_grant", "You do not have access to this software.");
                    return;
                }
            }

            // and if we get here, current user is a superuser. sign in.
            await GenerateUserIdentity(context, user);
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