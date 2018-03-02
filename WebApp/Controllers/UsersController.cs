using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    public class UsersController : BaseApiController
    {

        [HttpPost]
        [DeflateCompression]
        [ResponseType(typeof(UserDTO))]
        public async Task<IHttpActionResult> CreateSuperUser(CreateSuperUserDTO model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("Email address is required");

            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("Password is required");

            var superUser = new SuperUser
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email
            };

            UnitOfWork.UserManager.AddOrUpdateUser(superUser, model.Password);
            await UnitOfWork.UserManager.AddToRoleAsync(superUser.Id, Role.PLATFORM_ADMINISTRATOR);

            return Ok();
        }

        public class CreateSuperUserDTO
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }

    }
}
