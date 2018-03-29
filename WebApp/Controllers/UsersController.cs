using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class UsersController : BaseApiController
    {
        [Authorize(Roles = "System administrator,Platform administrator")]
        public IHttpActionResult Get()
        {
            var users = UnitOfWork.SuperUsersRepository
                .AllAsNoTracking.ToList()
                .Select(x => Mapper.Map<UserDTO>(x))
                .ToList();

            return Ok(users);
        }

        [Authorize(Roles = "System administrator,Platform administrator")]
        public IHttpActionResult Get(Guid id)
        {
            var user = UnitOfWork.SuperUsersRepository.Find(id);
            if (user == null)
                return NotFound();

            return Ok(Mapper.Map<UserDTO>(user));
        }

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

        [HttpPut]
        public IHttpActionResult Put(Guid id, [FromBody]UserDTO value)
        {
            var user = UnitOfWork.SuperUsersRepository.Find(id);
            if (user == null)
                return NotFound();

            user.Email = value.Email;
            user.Gender = value.Gender;
            user.Birthdate = value.Birthdate;
            user.Address = value.Address;

            if (!user.PhoneNumberConfirmed)
                user.PhoneNumber = value.PhoneNumber;

            var result = UnitOfWork.UserManager.UpdateSync(user);
            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors.ToString(", "));
        }

    }
}
