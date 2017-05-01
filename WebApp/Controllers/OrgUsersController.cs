using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Models;

namespace WebApi.Controllers
{

    public class OrgUsersController : BaseApiController
    {

        private OrgUsersRepository Users { get { return UnitOfWork.OrgUsersRepository; } }
        private OrganisationRepository Organisations { get { return UnitOfWork.OrganisationRepository; } }

        private OrgUserTypesRepository Types { get { return UnitOfWork.OrgUserTypesRepository; } }

        [ResponseType(typeof(IEnumerable<OrgUserDTO>))]
        public IHttpActionResult Get()
        {
            var users = Users.AllIncluding(u => u.Type)
                .Where(u => u.OrganisationId == CurrentOrganisationId)
                .OrderBy(u => u.Surname)
                .ThenBy(u => u.FirstName)
                .ToList()
                .Select(u => Mapper.Map<OrgUserDTO>(u)).ToList();

            return Ok(users);
        }

        [ResponseType(typeof(IEnumerable<OrgUserDTO>))]
        public IHttpActionResult Get(Guid id)
        {
            var user = Users.Find(id);
            return Ok(Mapper.Map<OrgUserDTO>(user));
        }

        public IHttpActionResult Post([FromBody]OrgUserDTO value)
        {

            if (value.Password.IsEmpty())
                ModelState.AddModelError("Password", "Please provide password.");

            if (value.Password != value.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "'Password' and 'Confirm password' must be the same.");

            var orguser = Mapper.Map<OrgUser>(value);
            orguser.UserName = orguser.Email;
            orguser.OrganisationId = CurrentOrgUser.OrganisationId.Value;

            var identityResult = ServiceContext.UserManager.CreateSync(orguser, value.Password);

            if (!identityResult.Succeeded)
                throw new Exception(identityResult.Errors.ToString(". "));

            orguser.Type = UnitOfWork.OrgUserTypesRepository.Find(orguser.TypeId);
            UnitOfWork.UserManager.AssignRolesByUserType(orguser);
            return Ok();
        }

        public IHttpActionResult Put(Guid id, [FromBody]OrgUserDTO value)
        {
            var orguser = Users.Find(id);
            if (orguser == null)
                return NotFound();

            Mapper.Map(value, orguser);
            orguser.OrganisationId = CurrentOrgUser.OrganisationId.Value;

            UnitOfWork.UserManager.UpdateSync(orguser);

            return Ok();
        }

        public IHttpActionResult Delete(Guid id)
        {
            if (id == CurrentUser.Id)
                throw new InvalidOperationException("Current user cannot be deleted!");

            var orguser = Users.Find(id);
            if (orguser == null)
                return NotFound();

            if (orguser.IsRootUser)
                return BadRequest();

            Users.Delete(id);
            UnitOfWork.Save();

            return Ok();
        }
    }
}