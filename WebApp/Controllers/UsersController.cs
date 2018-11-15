using AutoMapper;
using LightMethods.Survey.Models.DAL;
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
    public class UsersController : BaseApiController
    {
        private const string CACHE_KEY = "SUPERUSERS";

        // GET api/users
        public IHttpActionResult Get()
        {
            var values = MemoryCacher.GetValue(CACHE_KEY);
            if (values == null)
            {
                var users = UnitOfWork.SuperUsersRepository
                    .AllAsNoTracking
                    .ToList()
                    .Select(x => Mapper.Map<UserDTO>(x))
                    .ToList();

                MemoryCacher.Add(CACHE_KEY, users, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(users);
            }
            else
            {
                var result = (List<UserDTO>)values;
                return new CachedResult<List<UserDTO>>(result, TimeSpan.FromMinutes(1), this);
            }

        }

        // GET api/users/{id}
        public IHttpActionResult Get(Guid id)
        {
            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var user = UnitOfWork.SuperUsersRepository.Find(id);
                if (user == null)
                    return NotFound();

                var result = Mapper.Map<UserDTO>(user);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = cacheEntry as UserDTO;
                return new CachedResult<UserDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/users
        [HttpPost]
        public async Task<IHttpActionResult> CreateSuperUser(CreateSuperUserDTO model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("email address is required");

            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("password is required");

            var superUser = new SuperUser
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname
            };

            try
            {
                UnitOfWork.UserManager.AddOrUpdateUser(superUser, model.Password);
                await UnitOfWork.UserManager.AddToRoleAsync(superUser.Id, Role.SYSTEM_ADMINSTRATOR);

                MemoryCacher.Delete(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/users/{id}
        [HttpPut]
        public IHttpActionResult Put(Guid id, [FromBody]UserDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var user = UnitOfWork.SuperUsersRepository.Find(id);
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

        // DEL api/users/{id}
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var user = UnitOfWork.SuperUsersRepository.Find(id);
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
