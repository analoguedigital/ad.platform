using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("api/orginvitations")]
    public class OrgInvitationsController : BaseApiController
    {

        // GET api/orgInvitations/{organisationId?}
        public IHttpActionResult Get(Guid? organisationId = null)
        {
            var result = new List<OrgInvitationDTO>();

            if (CurrentUser is SuperUser)
            {
                var invitations = UnitOfWork.OrgInvitationsRepository.AllAsNoTracking;

                if (organisationId.HasValue)
                    invitations = invitations.Where(x => x.OrganisationId == organisationId.Value);

                result = invitations.ToList()
                    .Select(i => Mapper.Map<OrgInvitationDTO>(i)).ToList();
            }
            else if (CurrentUser is OrgUser)
            {
                var orgInvitations = UnitOfWork.OrgInvitationsRepository.AllAsNoTracking
                    .Where(x => x.OrganisationId == this.CurrentOrgUser.Organisation.Id);

                result = orgInvitations.ToList()
                    .Select(i => Mapper.Map<OrgInvitationDTO>(i)).ToList();
            }

            return Ok(result);
        }

        // GET api/orgInvitations/{id}
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var invitation = UnitOfWork.OrgInvitationsRepository.Find(id);
            if (invitation == null)
                return NotFound();

            return Ok(Mapper.Map<OrgInvitationDTO>(invitation));
        }

        // POST api/orgInvitations
        [HttpPost]
        public IHttpActionResult Post([FromBody]OrgInvitationDTO value)
        {
            var invitation = Mapper.Map<OrganisationInvitation>(value);

            if (this.CurrentUser is SuperUser)
            {
                invitation.OrganisationId = Guid.Parse(value.Organisation.Id);
                invitation.Organisation = null;
            }
            else if (this.CurrentUser is OrgUser)
            {
                invitation.Organisation = this.CurrentOrgUser.Organisation;
            }

            try
            {
                UnitOfWork.OrgInvitationsRepository.InsertOrUpdate(invitation);
                UnitOfWork.Save();

                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Cannot create duplicate tokens.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/orgInvitations/{id}
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]OrgInvitationDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var invitation = UnitOfWork.OrgInvitationsRepository.Find(id);
            if (invitation == null)
                return BadRequest();

            invitation.Name = value.Name;
            invitation.Token = value.Token;
            invitation.Limit = value.Limit;
            invitation.IsActive = value.IsActive;
            invitation.OrganisationId = Guid.Parse(value.Organisation.Id);

            try
            {
                UnitOfWork.OrgInvitationsRepository.InsertOrUpdate(invitation);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DEL api/orgInvitations/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                UnitOfWork.OrgInvitationsRepository.Delete(id);
                UnitOfWork.OrgInvitationsRepository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
