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

            var orgUser = user as OrgUser;
            var result = Mapper.Map<OrgUserDTO>(user);

            var assignments = orgUser.Assignments.Select(a => Mapper.Map<ProjectAssignmentDTO>(a)).ToList();
            result.Assignments = assignments;

            return Ok(result);
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

            if (value.Type.Name.ToLower() == "administrator")
            {
                var projects = UnitOfWork.ProjectsRepository.AllAsNoTracking
                    .Where(p => p.OrganisationId == this.CurrentOrgUser.OrganisationId);

                foreach (var project in projects)
                {
                    var assignment = new Assignment
                    {
                        ProjectId = project.Id,
                        OrgUserId = orguser.Id,
                        CanView = true,
                        CanAdd = true,
                        CanEdit = true,
                        CanDelete = true
                    };

                    UnitOfWork.AssignmentsRepository.InsertOrUpdate(assignment);
                }

                UnitOfWork.Save();
            }

            return Ok();
        }

        public IHttpActionResult Put(Guid id, [FromBody]OrgUserDTO value)
        {
            var orguser = Users.Find(id);
            if (orguser == null)
                return NotFound();

            Mapper.Map(value, orguser);
            orguser.OrganisationId = CurrentOrgUser.OrganisationId.Value;

            var result = UnitOfWork.UserManager.UpdateSync(orguser);

            if (result.Succeeded)
                return Ok();
            else
                return BadRequest(result.Errors.ToString(", "));
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