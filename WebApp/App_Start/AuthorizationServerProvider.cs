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

namespace WebApi
{

    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private const string ACCESS_TYPE_HEADER_NAME = "Access-Type";

        HttpContext HttpContext { get { return HttpContext.Current; } }

        public ApplicationUserManager UserManager
        {
            get
            {
                return ServiceContext.UserManager;
            }
        }

        public enum AccessTypes
        {
            WebApp = 0,
            MobileApp = 1
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

            var orgUser = result as LightMethods.Survey.Models.Entities.OrgUser;
            if (orgUser == null)
            {
                // this isn't a OrgUser (SuperUser or something else)
                var identity = await result.GenerateUserIdentityAsync(UserManager);
                identity.AddClaim(new Claim("email", result.Email));
                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim("role", "user"));

                context.Validated(identity);
            }
            else
            {
                // we have a OrgUser. validate access type.
                var accessTypeString = context.Request.Headers[ACCESS_TYPE_HEADER_NAME];
                AccessTypes accessType;

                var accessGranted = false;
                if (Enum.TryParse(accessTypeString, out accessType))
                {
                    switch (accessType)
                    {
                        case AccessTypes.WebApp:
                            accessGranted = orgUser.IsWebUser;
                            break;
                        case AccessTypes.MobileApp:
                            accessGranted = orgUser.IsMobileUser;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    context.SetError("invalid_grant", "The access type header is missing or invalid.");
                    return;
                }

                if (accessGranted)
                {
                    var identity = await result.GenerateUserIdentityAsync(UserManager);
                    identity.AddClaim(new Claim("email", result.Email));
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim("role", "user"));

                    context.Validated(identity);
                }
                else
                {
                    context.SetError("invalid_grant", "You do not have access to this software.");
                    return;
                }
            }
        }
    }
}