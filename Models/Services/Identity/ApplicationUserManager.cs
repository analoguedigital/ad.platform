﻿using AppHelper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services.Identity
{

    public class ApplicationUserManager : UserManager<User, Guid>
    {
        public ApplicationUserManager(IUserStore<User, Guid> store) : base(store) { }

        public void AddOrUpdateUser(User user, string password)
        {
            if (this.FindByEmail(user.Email) == null)
            {
                var result = this.Create(user, password);
            }
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<SurveyContext>()));

            manager.UserValidator = new UserValidator<User, Guid>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User, Guid>
            {
                MessageFormat = "Your security code is {0}"
            });

            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User, Guid>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }

        public IdentityResult CreateSync(User user)
        {
            var result = this.Create(user);
            AssignRolesByUserType(user);

            return result;
        }

        public IdentityResult UpdateSync(User user)
        {
            var result = this.Update(user);
            AssignRolesByUserType(user);

            return result;
        }

        public bool RolesContainsAny(Guid userId, params string[] roles)
        {
            return this.GetRoles(userId).ContainsAny(roles);
        }

        public void AssignRolesByUserType(User user)
        {
            // this method gets executed only in update requests.
            // removing and readding roles is necessary for OrgUsers
            // since their type might change. but not required for 
            // super users or platform users. we don't need to touch them.

            if (user is SuperUser)
            {
                this.AddToRole(user.Id, Role.SYSTEM_ADMINSTRATOR);
            }
            else if (user is PlatformUser)
            {
                this.AddToRole(user.Id, Role.PLATFORM_ADMINISTRATOR);
            }
            else if (user is OrgUser)
            {
                var roles = (user as OrgUser).Type.GetRoles();
                foreach (var role in this.GetRoles(user.Id).Where(r => !roles.Contains(r)))
                    this.RemoveFromRole(user.Id, role);

                foreach (var role in roles)
                    this.AddToRole(user.Id, role);
            }
        }

        public User FindByEmailSync(string email)
        {
            return this.FindByEmail(email);
        }

        public IdentityResult CreateSync(User user, string password)
        {
            return this.Create(user, password);
        }

        public IdentityResult AddToRoleSync(Guid userId, string role)
        {
            return this.AddToRole(userId, role);
        }

        public IdentityResult AddToRolesSync(Guid userId, params string[] roles)
        {
            return this.AddToRoles(userId, roles);
        }

        public override Task<IList<string>> GetRolesAsync(Guid userId)
        {
            var orgUser = this.FindById(userId) as OrgUser;
            if (orgUser != null && orgUser.AccountType == AccountType.MobileAccount)
            {
                var subscriptionService = new SubscriptionService(new UnitOfWork(new SurveyContext()));
                if (!subscriptionService.HasValidSubscription(userId))
                    return Task.FromResult<IList<string>>(new List<string>(new string[] { Role.RESTRICTED_USER }));
            }

            return base.GetRolesAsync(userId);
        }
    }
}
