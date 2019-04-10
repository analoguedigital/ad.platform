using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator")]
    public class PlatformUsersController : BaseApiController
    {

        private const string CACHE_KEY = "PLATFORM_USERS";

        // GET api/platform-users
        [Route("api/platform-users")]
        public IHttpActionResult Get()
        {
            var cacheEntry = MemoryCacher.GetValue(CACHE_KEY);
            if (cacheEntry == null)
            {
                var users = UnitOfWork.PlatformUsersRepository
                    .AllAsNoTracking
                    .ToList()
                    .Select(x => Mapper.Map<UserDTO>(x))
                    .ToList();

                MemoryCacher.Add(CACHE_KEY, users, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(users);
            }
            else
            {
                var result = (List<UserDTO>)cacheEntry;
                return new CachedResult<List<UserDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/platform-users/{id}
        [Route("api/platform-users/{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var user = UnitOfWork.PlatformUsersRepository.Find(id);
                if (user == null)
                    return NotFound();

                var result = Mapper.Map<UserDTO>(user);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (UserDTO)cacheEntry;
                return new CachedResult<UserDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/platform-users
        [HttpPost]
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
                Surname = model.Surname,
                RegistrationDate = DateTime.UtcNow
            };

            try
            {
                UnitOfWork.UserManager.AddOrUpdateUser(platformUser, model.Password);
                await UnitOfWork.UserManager.AddToRoleAsync(platformUser.Id, Role.PLATFORM_ADMINISTRATOR);

                MemoryCacher.Delete(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/platform-users/{id}
        [HttpPut]
        [Route("api/platform-users/{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]UserDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var user = UnitOfWork.PlatformUsersRepository.Find(id);
            if (user == null)
                return NotFound();

            user.Email = value.Email;
            user.Address = value.Address;
            user.FirstName = value.FirstName;
            user.Surname = value.Surname;

            if (!user.PhoneNumberConfirmed)
                user.PhoneNumber = value.PhoneNumber;

            try
            {
                var result = UnitOfWork.UserManager.UpdateSync(user);
                if (result.Succeeded)
                {
                    MemoryCacher.DeleteListAndItem(CACHE_KEY, id);
                    return Ok();
                }

                return BadRequest(result.Errors.ToString(", "));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/platform-users/{id}
        [HttpDelete]
        [Route("api/platform-users/{id:guid}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var user = UnitOfWork.PlatformUsersRepository.Find(id);
            if (user == null)
                return NotFound();

            try
            {
                var result = await UnitOfWork.UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    MemoryCacher.DeleteListAndItem(CACHE_KEY, id);
                    return Ok();
                }

                return BadRequest(result.Errors.ToString(", "));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
