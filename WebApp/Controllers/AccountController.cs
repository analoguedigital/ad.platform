﻿using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using LightMethods.Survey.Models.Services.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApi.Controllers;
using WebApi.Providers;
using WebApi.Results;

namespace WebApi.Models
{
    [RoutePrefix("api/Account")]
    public class AccountController : BaseApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public async Task<IHttpActionResult> GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            var userId = Guid.Parse(User.Identity.GetUserId());
            var user = UnitOfWork.UsersRepository.Find(userId);
            var orgUser = user as OrgUser;

            var phoneNumber = await UserManager.GetPhoneNumberAsync(userId);
            var twoFactorAuth = await UserManager.GetTwoFactorEnabledAsync(userId);

            var userInfo = new UserInfoViewModel
            {
                UserId = this.CurrentUser.Id,
                Email = User.Identity.GetUserName(),
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null,
                Language = this.CurrentOrganisation?.DefaultLanguage?.Calture,
                Calendar = this.CurrentOrganisation?.DefaultCalendar?.SystemName,
                Roles = ServiceContext.UserManager.GetRoles(this.CurrentUser.Id),
                TwoFactorAuthenticationEnabled = twoFactorAuth
            };

            if (orgUser != null)
            {
                userInfo.OrganisationId = this.CurrentOrganisationId;

                userInfo.Profile = new UserProfileDTO
                {
                    FirstName = orgUser.FirstName,
                    Surname = orgUser.Surname,
                    Gender = orgUser.Gender,
                    Birthdate = orgUser.Birthdate,
                    Address = orgUser.Address,
                    PhoneNumber = orgUser.PhoneNumber
                };
            }

            return Ok(userInfo);
        }

        // POST api/Account/UpdateProfile
        [HttpPost]
        [Route("UpdateProfile")]
        public IHttpActionResult UpdateProfileInfo(UserProfileDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orgUser = this.UnitOfWork.OrgUsersRepository.Find(this.CurrentUser.Id);
            if (orgUser == null)
                return BadRequest("User not found!");

            orgUser.FirstName = model.FirstName;
            orgUser.Surname = model.Surname;
            orgUser.Gender = model.Gender;
            orgUser.Birthdate = model.Birthdate;
            orgUser.Address = model.Address;
            orgUser.PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? null : model.PhoneNumber;

            try
            {
                this.UnitOfWork.OrgUsersRepository.InsertOrUpdate(orgUser);
                this.UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            //Authentication.SignOut( CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId().ToGuid());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (var linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId().ToGuid(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId().ToGuid(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await UserManager.FindByNameAsync(model.Email);
            // don't reveal that user does not exist! return Ok();
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                return Ok();

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771

            string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var encodedToken = HttpUtility.UrlEncode(token);

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var redirectLink = baseUrl + "/wwwroot/dist/index.html#!/set-password?email=" + model.Email + "&token=" + encodedToken;

            var messageBody = @"<html>
                <head>
                    <style>
                        .message-container {
                            border: 1px solid #e8e8e8;
                            border-radius: 2px;
                            padding: 10px 15px;
                        }
                    </style>
                </head>
                <body>
                <div class='message-container'>
                    <p>
                        Click on <a href='" + redirectLink + @"'>this link</a> to set a new password.
                    </p>
                    <br>
                    <p>If the link didn't work, copy/paste the token below and reset your password manually.</p>
                    <p style='color: gray; font-weight: italic'>" + token + @"</p>

                    <br><br>
                    <small style='color: gray;'>Copyright &copy; 2018. analogueDIGITAL platform</small>
                </div>

                </body></html>";

            await UserManager.SendEmailAsync(user.Id, "Password reset request", messageBody);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
                return Ok();

            ModelState.Clear();
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return BadRequest(ModelState);
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId().ToGuid(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId().ToGuid());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId().ToGuid(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            User user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organisation = UnitOfWork.OrganisationRepository.FindByName(model.OrganisationName);
            if (organisation == null)
            {
                ModelState.AddModelError("Organisation", "Organisation was not found!");
                return BadRequest(ModelState);
            }

            var user = new OrgUser()
            {
                FirstName = model.FirstName,
                Surname = model.Surname,
                UserName = model.Email,
                Email = model.Email,
                OrganisationId = organisation.Id,
                IsRootUser = false,
                IsActive = true,
                TypeId = OrgUserTypesRepository.TeamUser.Id,
                IsMobileUser = true,
                IsWebUser = true,   // should be turned off by default
                AccountType = AccountType.MobileAccount
            };

            if (!string.IsNullOrEmpty(model.Address))
                user.Address = model.Address;

            if (!string.IsNullOrEmpty(model.Gender))
                user.Gender = (User.GenderType)Enum.Parse(typeof(User.GenderType), model.Gender);

            if (model.Birthdate.HasValue)
            {
                var localValue = TimeZone.CurrentTimeZone.ToLocalTime(model.Birthdate.Value);
                user.Birthdate = new DateTime(localValue.Year, localValue.Month, localValue.Day, 0, 0, 0, DateTimeKind.Utc);
            }

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return GetErrorResult(result);

            user.Type = UnitOfWork.OrgUserTypesRepository.Find(user.TypeId);
            UnitOfWork.UserManager.AssignRolesByUserType(user);

            var project = new Project()
            {
                Name = $"{model.FirstName} {model.Surname}",
                StartDate = DateTimeService.UtcNow,
                OrganisationId = organisation.Id,
                CreatedById = user.Id
            };

            UnitOfWork.ProjectsRepository.InsertOrUpdate(project);
            UnitOfWork.Save();

            var assignment = new Assignment()
            {
                ProjectId = project.Id,
                OrgUserId = user.Id,
                CanView = true,
                CanAdd = true,
                CanExportPdf = true,
                CanExportZip = true
            };

            UnitOfWork.AssignmentsRepository.InsertOrUpdate(assignment);

            // assign organisation admin to this project
            if (organisation.RootUser != null)
            {
                UnitOfWork.AssignmentsRepository.InsertOrUpdate(new Assignment
                {
                    ProjectId = project.Id,
                    OrgUserId = organisation.RootUserId.Value,
                    CanView = true,
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true,
                    CanExportPdf = true,
                    CanExportZip = true
                });
            }

            UnitOfWork.Save();

            // set user's current project
            var _orgUser = UnitOfWork.OrgUsersRepository.Find(user.Id);
            _orgUser.CurrentProjectId = project.Id;

            UnitOfWork.OrgUsersRepository.InsertOrUpdate(_orgUser);
            UnitOfWork.Save();

            // send account confirmation email
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var encodedCode = HttpUtility.UrlEncode(code);

            var rootIndex = GetRootIndexPath();
            var baseUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}";
            var callbackUrl = $"{baseUrl}#!/verify-email?userId={user.Id}&code={encodedCode}";

            var messageBody = $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>";

            await UserManager.SendEmailAsync(user.Id, "Confirm your account", messageBody);

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new OrgUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("SendEmailConfirmation")]
        public async Task<IHttpActionResult> SendEmailConfirmation(SendEmailConfirmationModel model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound();

            if (user.EmailConfirmed)
                return Ok();

            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var encodedCode = HttpUtility.UrlEncode(code);

            var rootIndex = GetRootIndexPath();
            var baseUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}";
            var callbackUrl = $"{baseUrl}#!/verify-email?userId={user.Id}&code={encodedCode}";

            var messageBody = $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>";
            
            await UserManager.SendEmailAsync(user.Id, "Confirm your account", messageBody);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(ConfirmEmailModel model)
        {
            if (model.UserId == Guid.Empty || string.IsNullOrEmpty(model.Code))
                return BadRequest();

            var result = await UserManager.ConfirmEmailAsync(model.UserId, model.Code);
            if (result.Succeeded)
                return Ok();

            var errorString = new StringBuilder();
            foreach (var err in result.Errors)
                errorString.AppendLine(err);

            return BadRequest(errorString.ToString());
        }

        [HttpPost]
        [Route("AddPhoneNumber")]
        public async Task<IHttpActionResult> AddPhoneNumber(AddPhoneNumberModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
                return BadRequest();

            var userId = Guid.Parse(User.Identity.GetUserId());
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, model.PhoneNumber);

            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.PhoneNumber,
                    Body = "Your security code is: " + code
                };

                await UserManager.SmsService.SendAsync(message);
            }

            return Ok();
        }

        [HttpPost]
        [Route("VerifyPhoneNumber")]
        public async Task<IHttpActionResult> VerifyPhoneNumber(VerifyPhoneNumberModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.Code))
                return BadRequest();

            var userId = Guid.Parse(User.Identity.GetUserId());
            var result = await UserManager.ChangePhoneNumberAsync(userId, model.PhoneNumber, model.Code);

            if (result.Succeeded)
            {
                // sign in automatically after verification
                //var user = await UserManager.FindByIdAsync(userId);
                //if (user != null)
                //{
                //    await SignInAsync(user, isPersistent: false);
                //}

                return Ok();
            }

            var errorString = new StringBuilder();
            foreach (var err in result.Errors)
                errorString.AppendLine(err);

            return BadRequest(errorString.ToString());
        }

        [HttpPost]
        [Route("RemovePhoneNumber")]
        public IHttpActionResult RemovePhoneNumber()
        {
            var user = UnitOfWork.UsersRepository.Find(this.CurrentUser.Id);
            user.PhoneNumber = string.Empty;
            user.PhoneNumberConfirmed = false;

            UnitOfWork.UsersRepository.InsertOrUpdate(user);
            UnitOfWork.Save();

            return Ok();
        }

        [HttpPost]
        [Route("ChangePhoneNumber")]
        public async Task<IHttpActionResult> ChangePhoneNumber(ChangePhoneNumberModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
                return BadRequest();

            var userId = Guid.Parse(User.Identity.GetUserId());
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (user.PhoneNumber.Equals(model.PhoneNumber))
                return BadRequest("You need to input a different phone number");

            var changeToken = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, model.PhoneNumber);

            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.PhoneNumber,
                    Body = "Your security code is: " + changeToken
                };

                await UserManager.SmsService.SendAsync(message);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> VerifyChangedNumber(VerifyChangedNumberModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.Code))
                return BadRequest();

            var userId = Guid.Parse(User.Identity.GetUserId());
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var result = await UserManager.VerifyChangePhoneNumberTokenAsync(userId, model.Code, model.PhoneNumber);
            if (result)
            {
                var changePhoneResult = await UserManager.ChangePhoneNumberAsync(userId, model.PhoneNumber, model.Code);
                if (changePhoneResult.Succeeded)
                    return Ok();

                var errorString = new StringBuilder();
                foreach (var err in changePhoneResult.Errors)
                    errorString.AppendLine(err);

                return BadRequest(errorString.ToString());
            }

            return BadRequest("Invalid security code");
        }

        //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        //{
        //    // Clear the temporary cookies used for external and two factor sign ins
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie,
        //       DefaultAuthenticationTypes.TwoFactorCookie);
        //    AuthenticationManager.SignIn(new AuthenticationProperties
        //    {
        //        IsPersistent = isPersistent
        //    },
        //       await user.GenerateUserIdentityAsync(UserManager));
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        private string GetRootIndexPath()
        {
            var rootIndexPath = ConfigurationManager.AppSettings["RootIndexPath"];
            if (!string.IsNullOrEmpty(rootIndexPath))
                return rootIndexPath;

            return "wwwroot/index.html";
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }

    #region DTOs

    public class VerifyPhoneNumberModel
    {
        public string PhoneNumber { get; set; }

        public string Code { get; set; }
    }

    public class AddPhoneNumberModel
    {
        public string PhoneNumber { get; set; }
    }

    public class SendEmailConfirmationModel
    {
        public string Email { get; set; }
    }

    public class ConfirmEmailModel
    {
        public Guid UserId { get; set; }

        public string Code { get; set; }
    }

    #endregion

    public class ChangePhoneNumberModel
    {
        public string PhoneNumber { get; set; }
    }

    public class VerifyChangedNumberModel
    {
        public string PhoneNumber { get; set; }

        public string Code { get; set; }
    }

}
