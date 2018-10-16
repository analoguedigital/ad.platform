using AutoMapper;
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
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator")]
    public class PlatformUsersController : BaseApiController
    {
        // GET api/platform-users
        [Route("api/platform-users")]
        public IHttpActionResult Get()
        {
            var users = UnitOfWork.PlatformUsersRepository
                .AllAsNoTracking.ToList()
                .Select(x => Mapper.Map<UserDTO>(x))
                .ToList();

            return Ok(users);
        }

        // GET api/platform-users/{id}
        [Route("api/platform-users/{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var user = UnitOfWork.PlatformUsersRepository.Find(id);
            if (user == null)
                return NotFound();

            return Ok(Mapper.Map<UserDTO>(user));
        }

        [HttpPost]
        //[DeflateCompression]
        //[ResponseType(typeof(UserDTO))]
        [Route("api/platform-users")]
        public async Task<IHttpActionResult> Post(CreateSuperUserDTO model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("Email address is required");

            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("Password is required");

            var platformUser = new PlatformUser
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname
            };

            UnitOfWork.UserManager.AddOrUpdateUser(platformUser, model.Password);
            await UnitOfWork.UserManager.AddToRoleAsync(platformUser.Id, Role.PLATFORM_ADMINISTRATOR);

            return Ok();
        }

        // PUT api/platform-users/{id}
        [HttpPut]
        [Route("api/platform-users/{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]UserDTO value)
        {
            var user = UnitOfWork.PlatformUsersRepository.Find(id);
            if (user == null)
                return NotFound();

            user.Email = value.Email;
            user.Address = value.Address;
            user.FirstName = value.FirstName;
            user.Surname = value.Surname;

            if (!user.PhoneNumberConfirmed)
                user.PhoneNumber = value.PhoneNumber;

            var result = UnitOfWork.UserManager.UpdateSync(user);
            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors.ToString(", "));
        }

        // DEL api/platform-users/{id}
        [HttpDelete]
        [Route("api/platform-users/{id:guid}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            if (id == null || id == Guid.Empty)
                return BadRequest();

            var user = UnitOfWork.PlatformUsersRepository.Find(id);
            if (user == null)
                return NotFound();

            var result = await UnitOfWork.UserManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors.ToString(", "));
        }

    }
}
