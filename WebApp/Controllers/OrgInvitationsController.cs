using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [RoutePrefix("api/orginvitations")]
    [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator")]
    public class OrgInvitationsController : BaseApiController
    {

        private const string CACHE_KEY = "ORG_INVITATIONS";

        // GET api/orgInvitations/{organisationId?}
        public IHttpActionResult Get(Guid? organisationId = null)
        {
            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{CurrentOrgUser.Organisation.Id}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var invitations = UnitOfWork.OrgInvitationsRepository
                        .AllAsNoTracking
                        .Where(x => x.OrganisationId == CurrentOrgUser.Organisation.Id)
                        .ToList();

                    var result = invitations
                        .Select(i => Mapper.Map<OrgInvitationDTO>(i))
                        .ToList();

                    MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(result);
                }
                else
                {
                    var result = (List<OrgInvitationDTO>)cacheEntry;
                    return new CachedResult<List<OrgInvitationDTO>>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser or PlatformUser
            var _cacheKey = organisationId.HasValue ? $"{CACHE_KEY}_{organisationId.Value}" : CACHE_KEY;
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var _invitations = UnitOfWork.OrgInvitationsRepository.AllAsNoTracking;

                if (organisationId.HasValue)
                    _invitations = _invitations.Where(x => x.OrganisationId == organisationId.Value);

                var retVal = _invitations
                    .ToList()
                    .Select(i => Mapper.Map<OrgInvitationDTO>(i))
                    .ToList();

                MemoryCacher.Add(_cacheKey, retVal, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(retVal);
            }
            else
            {
                var retVal = (List<OrgInvitationDTO>)_cacheEntry;
                return new CachedResult<List<OrgInvitationDTO>>(retVal, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/orgInvitations/{id}
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var invitation = UnitOfWork.OrgInvitationsRepository.Find(id);
                if (invitation == null)
                    return NotFound();

                var result = Mapper.Map<OrgInvitationDTO>(invitation);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (OrgInvitationDTO)cacheEntry;
                return new CachedResult<OrgInvitationDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/orgInvitations
        [HttpPost]
        public IHttpActionResult Post([FromBody]OrgInvitationDTO value)
        {
            var invitation = Mapper.Map<OrganisationInvitation>(value);

            if (CurrentUser is OrgUser)
                invitation.Organisation = CurrentOrgUser.Organisation;
            else
            {
                invitation.OrganisationId = Guid.Parse(value.Organisation.Id);
                invitation.Organisation = null;
            }

            UnitOfWork.OrgInvitationsRepository.InsertOrUpdate(invitation);

            try
            {
                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest("cannot create duplicate invitation tokens");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/orgInvitations/{id}
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Put(Guid id, [FromBody]OrgInvitationDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var invitation = UnitOfWork.OrgInvitationsRepository.Find(id);
            if (invitation == null)
                return NotFound();

            invitation.Name = value.Name;
            invitation.Token = value.Token;
            invitation.Limit = value.Limit;
            invitation.IsActive = value.IsActive;
            invitation.OrganisationId = Guid.Parse(value.Organisation.Id);

            try
            {
                UnitOfWork.OrgInvitationsRepository.InsertOrUpdate(invitation);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/orgInvitations/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            try
            {
                UnitOfWork.OrgInvitationsRepository.Delete(id);
                UnitOfWork.OrgInvitationsRepository.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
