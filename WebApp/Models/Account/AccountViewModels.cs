using LightMethods.Survey.Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LightMethods.Survey.Models.Entities.User;

namespace WebApi.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public Guid UserId { get; set; }

        public Guid? OrganisationId { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }

        public string Language { set; get; }

        public string Calendar { set; get; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public IList<string> Roles { set; get; }

        public UserProfileDTO Profile { get; set; }

        public bool TwoFactorAuthenticationEnabled { get; set; }
    }

    public class UserProfileDTO
    {
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Surname { get; set; }

        public GenderType? Gender { get; set; }

        public DateTime? Birthdate { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsSubscribed { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public SubscriptionDTO LastSubscription { get; set; }

        public MonthlyQuotaDTO MonthlyQuota { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ConfirmPassword { get; set; }
    }
}
