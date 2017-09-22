using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class ChangePasswordController : BaseApiController
    {
        private OrgUsersRepository Users { get { return UnitOfWork.OrgUsersRepository; } }

        [HttpPost]
        [Route("api/changePassword")]
        public async Task<IHttpActionResult> Post([FromBody]ChangePasswordDTO value)
        {

            var currentUser = ServiceContext.CurrentUser;
            var userManager = ServiceContext.UserManager;

            var selfReset = (value.OldPassword?.Length ?? 0) > 0;
            var isAdmin = !(currentUser is OrgUser) || (currentUser as OrgUser).Type.GetRoles().Contains(Role.ORG_ADMINSTRATOR);
            var user = selfReset ? currentUser : Users.Find(value.UserId);

            if (selfReset && userManager.PasswordHasher.VerifyHashedPassword(currentUser.PasswordHash, value.OldPassword) != PasswordVerificationResult.Success)
                return BadRequest("Provided old password is incorrect!");

            if (!selfReset && !isAdmin)
                return Unauthorized();

            var token = await user.GeneratePasswordResetTokenAsync(userManager);

            var result = userManager.ResetPassword(user.Id, token, value.NewPassword);

            if(result.Succeeded)
                return Ok();

            return BadRequest(string.Join(" ,", result.Errors));
        }
    }
}