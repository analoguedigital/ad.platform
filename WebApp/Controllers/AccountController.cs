using LightMethods.Survey.Models.DAL;
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
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using WebApi.Controllers;
using WebApi.Providers;
using WebApi.Results;

namespace WebApi.Models
{
    [RoutePrefix("api/Account")]
    public class AccountController : BaseApiController
    {

        #region Properties

        private const string LocalLoginProvider = "Local";

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set { _userManager = value; }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        #endregion

        public AccountController() { }

        public AccountController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

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
                TwoFactorAuthenticationEnabled = twoFactorAuth,
            };

            if (orgUser != null)
            {
                userInfo.OrganisationId = this.CurrentOrganisationId;

                var subscriptionService = new SubscriptionService(orgUser, this.UnitOfWork);
                var expiryDate = subscriptionService.GetLatest(orgUser.Id);
                var lastSubscription = subscriptionService.GetLastSubscription(orgUser.Id);
                var quota = subscriptionService.GetMonthlyQuota(orgUser.Id);

                userInfo.Profile = new UserProfileDTO
                {
                    FirstName = orgUser.FirstName,
                    Surname = orgUser.Surname,
                    Gender = orgUser.Gender,
                    Birthdate = orgUser.Birthdate,
                    Address = orgUser.Address,
                    Email = orgUser.Email,  // THIS COULD BE REMOVED AS WELL
                    PhoneNumber = orgUser.PhoneNumber,  // REMOVE THIS PROPERTY
                    IsSubscribed = orgUser.IsSubscribed,
                    ExpiryDate = expiryDate,
                    LastSubscription = lastSubscription,
                    MonthlyQuota = quota
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

            if (!orgUser.PhoneNumberConfirmed)
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
            // client apps need to call this to actually log the current user out.
            // otherwise the identity tokens are still open on our server.
            // PS. the sign out function is a bit different with 2FA enabled.

            // there's no logout in OWIN and bearer tokens. you just generate
            // a token and it is valid for a set period of time. we could use
            // a refresh token provider though.

            //Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            //Request.GetOwinContext()
            //    .Authentication
            //    .SignOut(Request.GetOwinContext()
            //               .Authentication.GetAuthenticationTypes()
            //               .Select(o => o.AuthenticationType).ToArray());

            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId().ToGuid());
            if (user == null) return null;

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
                return BadRequest(ModelState);

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId().ToGuid(), model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId().ToGuid(), model.NewPassword);
            if (!result.Succeeded)
                return GetErrorResult(result);

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
            // if user isn't found or the email isn't confirmed, don't reveal anything just return Ok.
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                return Ok();

            // generate the reset token and send the email.
            string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var encodedToken = HttpUtility.UrlEncode(token);

            var rootIndex = WebHelpers.GetRootIndexPath();
            var baseUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}";
            var redirectLink = $"{baseUrl}#!/set-password?email={model.Email}&token={encodedToken}";

            //var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            //var redirectLink = baseUrl + "/wwwroot/dist/index.html#!/set-password?email=" + model.Email + "&token=" + encodedToken;

            var content = @"<p>Click on <a href='" + redirectLink + @"'>this link</a> to set a new password.</p><br>
                    <p>If the link didn't work, copy/paste the token below and reset your password manually.</p>
                    <p style='color: gray; font-weight: italic'>" + encodedToken + @"</p>";

            var emailBody = WebHelpers.GenerateEmailTemplate(content, "Reset your password");

            await UserManager.SendEmailAsync(user.Id, "Password reset request", emailBody);

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
            if (user == null)   // Don't reveal the user doesn't exist
                return Ok();

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
                return BadRequest(ModelState);

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
                return BadRequest("The external login is already associated with an account.");

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId().ToGuid(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId().ToGuid());
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId().ToGuid(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
                return GetErrorResult(result);

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
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));

            if (!User.Identity.IsAuthenticated)
                return new ChallengeResult(provider, this);

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
                return InternalServerError();

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            User user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;
            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager, OAuthDefaults.AuthenticationType);
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
                state = null;

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
                IsWebUser = true,   // this should be 'false' in live production.
                IsSubscribed = false,
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

            // create a project for this user
            var project = new Project()
            {
                Name = $"{model.FirstName} {model.Surname}",
                StartDate = DateTimeService.UtcNow,
                OrganisationId = organisation.Id,
                CreatedById = user.Id
            };

            UnitOfWork.ProjectsRepository.InsertOrUpdate(project);
            UnitOfWork.Save();

            // assign this user to their project.
            var assignment = new Assignment()
            {
                ProjectId = project.Id,
                OrgUserId = user.Id,
                CanView = true,
                CanAdd = true,
                CanEdit = false,
                CanDelete = false,
                CanExportPdf = true,    // temporary. turn off in production.
                CanExportZip = true     // temporary. turn off in production.
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

            // subscribe this user under OnRecord with full access.
            if (_orgUser.AccountType == AccountType.MobileAccount && !_orgUser.Subscriptions.Any())
            {
                var onrecord = UnitOfWork.OrganisationRepository.AllAsNoTracking
                    .Where(x => x.Name == "OnRecord")
                    .SingleOrDefault();

                var subscription = new Subscription
                {
                    IsActive = true,
                    Type = UserSubscriptionType.Organisation,
                    StartDate = DateTimeService.UtcNow,
                    EndDate = null,
                    Note = $"Joined organisation - OnRecord",
                    OrgUserId = user.Id,
                    OrganisationId = onrecord.Id
                };

                UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
                _orgUser.IsSubscribed = true;

                UnitOfWork.Save();
            }

            // send account confirmation email
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var encodedCode = HttpUtility.UrlEncode(code);

            var rootIndex = WebHelpers.GetRootIndexPath();
            var baseUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}";
            var callbackUrl = $"{baseUrl}#!/verify-email?userId={user.Id}&code={encodedCode}";

            var content = @"<p>Your new account has been created. To complete your registration please confirm your email address by clicking the link below.</p>
                            <p><a href='" + callbackUrl + @"'>Verify Email Address</a></p>";

            var messageBody = WebHelpers.GenerateEmailTemplate(content, "Confirm your registration");
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
                return BadRequest(ModelState);

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
                return InternalServerError();

            var user = new OrgUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
                return GetErrorResult(result);

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        // POST api/account/sendEmailConfirmation
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

            var rootIndex = WebHelpers.GetRootIndexPath();
            var baseUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}";
            var callbackUrl = $"{baseUrl}#!/verify-email?userId={user.Id}&code={encodedCode}";

            var content = @"<p>Your new account has been created. To complete your registration please confirm your email address by clicking the link below.</p>
                            <p><a href='" + callbackUrl + @"'>Verify Email Address</a></p>";

            var messageBody = WebHelpers.GenerateEmailTemplate(content, "Confirm your registration");
            await UserManager.SendEmailAsync(user.Id, "Confirm your account", messageBody);

            return Ok();
        }

        // POST api/account/confirmEmail
        [HttpPost]
        [AllowAnonymous]
        [Route("ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(ConfirmEmailModel model)
        {
            if (model.UserId == Guid.Empty || string.IsNullOrEmpty(model.Code))
                return BadRequest();

            var result = await UserManager.ConfirmEmailAsync(model.UserId, model.Code);
            if (result.Succeeded)
            {
                // subscribe this user under OnRecord with full access.
                var orgUser = this.UnitOfWork.OrgUsersRepository.Find(model.UserId);
                if (orgUser.AccountType == AccountType.MobileAccount && !orgUser.Subscriptions.Any())
                {
                    var onrecord = UnitOfWork.OrganisationRepository.AllAsNoTracking
                        .Where(x => x.Name == "OnRecord")
                        .SingleOrDefault();

                    var subscription = new Subscription
                    {
                        IsActive = true,
                        Type = UserSubscriptionType.Organisation,
                        StartDate = DateTimeService.UtcNow,
                        EndDate = null,
                        Note = $"Joined organisation - OnRecord",
                        OrgUserId = model.UserId,
                        OrganisationId = onrecord.Id
                    };

                    UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);

                    var orgUserAssignment = orgUser.Assignments.Where(x => x.ProjectId == orgUser.CurrentProject.Id).SingleOrDefault();
                    orgUserAssignment.CanExportPdf = true;
                    orgUserAssignment.CanExportZip = true;

                    UnitOfWork.AssignmentsRepository.InsertOrUpdate(orgUserAssignment);
                    orgUser.IsSubscribed = true;

                    UnitOfWork.Save();
                }

                return Ok();
            }

            var errorString = new StringBuilder();
            foreach (var err in result.Errors)
                errorString.AppendLine(err);

            return BadRequest(errorString.ToString());
        }

        // POST api/account/addPhoneNumber
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

            // if the Sms service is null, return an error or something.

            return Ok();
        }

        // POST api/account/verifyPhoneNumber
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
                // sign in after phone number confirmation.
                // this could be turned on after enabling 2FA.
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

        // POST api/account/removePhoneNumber
        [HttpPost]
        [Route("RemovePhoneNumber")]
        public IHttpActionResult RemovePhoneNumber()
        {
            var user = UnitOfWork.UsersRepository.Find(this.CurrentUser.Id);
            user.PhoneNumber = string.Empty;
            user.PhoneNumberConfirmed = false;

            try
            {
                UnitOfWork.UsersRepository.InsertOrUpdate(user);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST api/account/changePhoneNumber
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

        // POST api/account/verifyChangedNumber
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region ExternalLogin helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
                return InternalServerError();

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                        ModelState.AddModelError("", error);
                }

                if (ModelState.IsValid)
                    return BadRequest();    // No ModelState errors are available to send, just return an empty BadRequest.

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
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));

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

        #endregion

    }

}
