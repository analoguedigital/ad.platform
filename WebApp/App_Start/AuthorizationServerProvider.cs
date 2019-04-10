using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
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

            var signInResult = await ServiceContext.SignInManager.PasswordSignInAsync(context.UserName, context.Password, isPersistent: true, shouldLockout: false);
            if (signInResult == SignInStatus.Failure)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (signInResult == SignInStatus.LockedOut)
            {
                context.SetError("locked_out", "Your account has been locked out.");
                return;
            }

            if (signInResult == SignInStatus.RequiresVerification)
            {
                var user = await UserManager.FindAsync(context.UserName, context.Password);
                string tokenKey = "X-2FA-Token";
                string token = context.Request.Headers.Get(tokenKey);

                if (string.IsNullOrEmpty(token))
                {
                    var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, "Email Code");
                    IdentityResult result = await UserManager.NotifyTwoFactorTokenAsync(user.Id, "Email Code", code);

                    if (!result.Succeeded)
                    {
                        context.SetError("two_factor_error", "failed to send 2FA code");
                        return;
                    }
                    else
                    {
                        // user needs to verify security code
                        context.SetError("requires_verification");
                        return;
                    }
                }
                else
                {
                    var isValid = await UserManager.VerifyTwoFactorTokenAsync(user.Id, "Email Code", token);
                    if (!isValid)
                    {
                        context.SetError("invalid_grant", "Two-factor security code is not valid");
                        return;
                    }

                    await GenerateUserIdentity(context, user);
                }
            }

            if (signInResult == SignInStatus.Success)
            {
                var user = await UserManager.FindAsync(context.UserName, context.Password);
                if (user is SuperUser || user is PlatformUser)
                {
                    await GenerateUserIdentity(context, user);
                }
                else if (user is OrgUser)
                {
                    var orgUser = user as OrgUser;
                    if (!OrgUserHasAccess(orgUser))
                    {
                        context.SetError("invalid_grant", "You do not have access to this software.");
                        return;
                    }
                    else
                    {
                        if (!orgUser.EmailConfirmed)
                        {
                            context.SetError("email_not_verified", "You haven't confirmed your email address yet.");
                            return;
                        }

                        await GenerateUserIdentity(context, orgUser);
                    }
                }
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(User user)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userId", user.Id.ToString() },
                { "userName", user.UserName },
                { "twoFactorAuthEnabled", user.TwoFactorEnabled.ToString() }
            };

            return new AuthenticationProperties(data);
        }

        private async Task GenerateUserIdentity(OAuthGrantResourceOwnerCredentialsContext context, User user)
        {
            var identity = await user.GenerateUserIdentityAsync(UserManager);
            identity.AddClaim(new Claim("email", user.Email));
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);

            var dbUser = ServiceContext.UnitOfWork.UsersRepository.Find(user.Id);
            dbUser.LastLogin = DateTime.UtcNow;
            ServiceContext.UnitOfWork.UsersRepository.Save();
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