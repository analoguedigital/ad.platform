﻿using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    public class OrganisationsController : BaseApiController
    {
        private OrganisationRepository Organisations { get { return UnitOfWork.OrganisationRepository; } }
        private OrgUsersRepository OrgUsers { get { return UnitOfWork.OrgUsersRepository; } }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrganisationDTO>))]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator")]
        public IHttpActionResult Get()
        {
            var orgs = Organisations.AllIncluding(u => u.RootUser)
                .OrderBy(u => u.Name)
                .ToList()
                .Select(u => Mapper.Map<OrganisationDTO>(u)).ToList();

            return Ok(orgs);
        }

        [DeflateCompression]
        [ResponseType(typeof(OrganisationDTO))]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<OrganisationDTO>(new Organisation()));

            var org = Mapper.Map<OrganisationDTO>(Organisations.Find(id));

            if (org == null)
                return NotFound();

            return Ok(org);
        }

        [Authorize(Roles = "System administrator,Platform administrator")]
        public IHttpActionResult Post([FromBody]OrganisationDTO value)
        {
            var createOrganisation = new CreateOrganisation();
            createOrganisation.Name = value.Name;
            createOrganisation.RootUserEmail = value.RootUser.Email;
            createOrganisation.RootUserFirstName = value.RootUser.FirstName;
            createOrganisation.RootUserSurname = value.RootUser.Surname;
            createOrganisation.RootPassword = value.RootUser.Password;
            createOrganisation.RootConfirmPassword = value.RootUser.ConfirmPassword;
            createOrganisation.AddressLine1 = value.AddressLine1;
            createOrganisation.AddressLine2 = value.AddressLine2;
            createOrganisation.County = value.County;
            createOrganisation.Town = value.Town;
            createOrganisation.Postcode = value.Postcode;
            createOrganisation.TelNumber = value.TelNumber;
            createOrganisation.DefaultCalendarId = CalendarsRepository.Gregorian.Id;
            createOrganisation.DefaultLanguageId = LanguagesRepository.English.Id;

            Organisations.CreateOrganisation(createOrganisation);
            UnitOfWork.Save();

            return Ok();
        }

        [Authorize(Roles = "System administrator,Platform administrator")]
        public void Put(Guid id, [FromBody]OrganisationDTO value)
        {

            var org = Organisations.Find(id);
            if (org == null) return;

            Mapper.Map(value, org);

            Organisations.InsertOrUpdate(org);
            UnitOfWork.Save();
        }

        [Authorize(Roles = "System administrator,Platform administrator")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                Organisations.Delete(id);
                UnitOfWork.Save();

                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest("This organisation cannot be deleted!");
            }
        }

        [HttpPost]
        [Route("api/organisations/{id:guid}/assign")]
        [Authorize(Roles = "System administrator,Platform administrator")]
        public IHttpActionResult AssignUsers(Guid id, OrganisationAssignmentDTO model)
        {
            if (id == Guid.Empty)
                return NotFound();

            var org = this.Organisations.Find(id);
            if (org == null)
                return NotFound();

            foreach (var userId in model.OrgUsers)
            {
                var orgUser = this.OrgUsers.Find(userId);

                // remove this user from any teams in current organisation.
                var records = UnitOfWork.OrgTeamUsersRepository.AllAsNoTracking
                    .Where(x => x.OrgUserId == orgUser.Id && x.OrganisationTeam.OrganisationId == orgUser.OrganisationId)
                    .ToList();

                foreach (var item in records)
                    UnitOfWork.OrgTeamUsersRepository.Delete(item);

                orgUser.OrganisationId = id;    // update user's organisation

                if (orgUser.CurrentProject != null) // update user's current project, if exists
                {
                    if (orgUser.CurrentProject.CreatedById == orgUser.Id)
                    {
                        var project = UnitOfWork.ProjectsRepository.Find(orgUser.CurrentProject.Id);
                        project.OrganisationId = org.Id;    // update project's organisation

                        // update threads under this project
                        var threads = UnitOfWork.FormTemplatesRepository.AllAsNoTracking
                            .Where(t => t.ProjectId == project.Id)
                            .ToList();

                        // update form templates' organisation
                        foreach (var form in threads)
                        {
                            form.OrganisationId = org.Id;
                            UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);
                        }

                        // assign Org root user to the project
                        UnitOfWork.AssignmentsRepository.InsertOrUpdate(new Assignment
                        {
                            ProjectId = orgUser.CurrentProjectId.Value,
                            OrgUserId = org.RootUserId.Value,
                            CanAdd = true,
                            CanEdit = true,
                            CanDelete = true,
                            CanView = true,
                            CanExportPdf = true,
                            CanExportZip = true
                        });
                    }
                }
            }

            UnitOfWork.Save();

            return Ok();
        }

        [HttpDelete]
        [Route("api/organisations/{id:guid}/revoke/{userId:guid}")]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator")]
        public IHttpActionResult RevokeUser(Guid id, Guid userId)
        {
            // IMPORTANT NOTE:
            // when removing a user from an organisation,
            // we need to deactivate the subscription record and,
            // create a case assignment for the OnRecord root admin again.

            var org = this.Organisations.Find(id);
            if (org == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            // we need a better way of getting the default seed organization.
            // OnRecord ID: cfa81eb0-9fc7-4932-a3e8-1c822370d034
            var onrecord = UnitOfWork.OrganisationRepository.AllAsNoTracking
                .Where(x => x.Name == "OnRecord").FirstOrDefault();

            // remove this user from any teams in current organisation.
            var records = UnitOfWork.OrgTeamUsersRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == userId && x.OrganisationTeam.OrganisationId == orgUser.OrganisationId)
                .ToList();

            foreach (var item in records)
                UnitOfWork.OrgTeamUsersRepository.Delete(item);

            orgUser.OrganisationId = onrecord.Id;   // update user's organisation

            if (orgUser.CurrentProject != null)
            {
                if (orgUser.CurrentProject.CreatedById == orgUser.Id)
                {
                    var project = UnitOfWork.ProjectsRepository.Find(orgUser.CurrentProject.Id);
                    project.OrganisationId = onrecord.Id;    // update project's organisation

                    var threads = UnitOfWork.FormTemplatesRepository.AllAsNoTracking
                           .Where(t => t.ProjectId == project.Id)
                           .ToList();

                    foreach (var form in threads)   // update form templates' organisation
                    {
                        form.OrganisationId = onrecord.Id;
                        UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);
                    }

                    // remove the assignment for current organisation's root user
                    var rootUserAssignment = UnitOfWork.AssignmentsRepository.AllAsNoTracking
                        .Where(x => x.OrgUserId == org.RootUserId && x.ProjectId == project.Id).FirstOrDefault();

                    UnitOfWork.AssignmentsRepository.Delete(rootUserAssignment);

                    // assign OnRecord root admin to this project again.
                    if (onrecord.RootUser != null)
                    {
                        var onrecordRootAssignment = UnitOfWork.AssignmentsRepository.AllAsNoTracking
                            .Where(x => x.OrgUserId == onrecord.RootUserId && x.ProjectId == project.Id).FirstOrDefault();
                        if (onrecordRootAssignment == null)
                        {
                            var onRecordAdminAssignment = new Assignment
                            {
                                ProjectId = project.Id,
                                OrgUserId = onrecord.RootUser.Id,
                                CanView = true,
                                CanAdd = true,
                                CanEdit = true,
                                CanDelete = true,
                                CanExportPdf = true,
                                CanExportZip = true
                            };

                            UnitOfWork.AssignmentsRepository.InsertOrUpdate(onRecordAdminAssignment);
                        }
                    }
                }
            }

            try
            {
                var messageBody = @"<html>
                        <head>
                            <style>
                                .message-container {
                                    border: 1px solid #e8e8e8;
                                    border-radius: 2px;
                                    padding: 10px 15px;
                                }
                            </style>
                        </head>
                        <body>
                        <div class='message-container'>
                            <p>You have left the <strong>" + org.Name + @"</strong> organization.</p>
                            <p>Your personal case has been moved and is now filed under OnRecord.</p>

                            <br><br>
                            <p style='color: gray; font-size: small;'>Copyright &copy; 2018. analogueDIGITAL platform</p>
                        </div>

                        </body></html>";

                var email = new Email
                {
                    To = orgUser.Email,
                    Subject = $"Left organization - {org.Name}",
                    Content = messageBody
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(email);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class OrganisationAssignmentDTO
        {
            public List<Guid> OrgUsers { get; set; }
        }
    }
}