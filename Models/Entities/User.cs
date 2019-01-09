using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    //http://typecastexception.com/post/2014/07/13/ASPNET-Identity-20-Extending-Identity-Models-and-Using-Integer-Keys-Instead-of-Strings.aspx
    public class UserClaim : IdentityUserClaim<Guid> { }

    public class UserLogin : IdentityUserLogin<Guid> { }

    public class UserRole : IdentityUserRole<Guid> { }

    public class User : IdentityUser<Guid, UserLogin, UserRole, UserClaim>, IEntity
    {
        public enum GenderType { Male = 0, Female = 1, Other = 2 }

        public User()
        {
            IsActive = true;
        }

        [ReadOnly(true)]
        [Display(Name = "Last Login")]
        public DateTime? LastLogin { set; get; }

        [Display(Name = "First name")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Display(Name = "Surname")]
        [MaxLength(30)]
        public string Surname { get; set; }

        public GenderType? Gender { set; get; }

        public DateTime? Birthdate { set; get; }

        public string Address { set; get; }

        [ReadOnly(true)]
        public bool IsActive { set; get; }

        public override string ToString()
        {
            return UserName;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, Guid> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ExternalBearer);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(UserManager<User, Guid> manager)
        {
            return await manager.GeneratePasswordResetTokenAsync(this.Id);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, Guid> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

    }
}
