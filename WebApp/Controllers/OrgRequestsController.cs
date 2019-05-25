using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [RoutePrefix("api/orgRequests")]
    [Authorize(Roles = "System administrator,Platform administrator")]
    public class OrgRequestsController : BaseApiController
    {

        private const string CACHE_KEY = "ORG_REQUESTS";

        // GET api/orgRequests
        public IHttpActionResult Get()
        {
            var cacheEntry = MemoryCacher.GetValue(CACHE_KEY);
            if (cacheEntry == null)
            {
                var orgRequests = UnitOfWork.OrgRequestsRepository
                    .AllAsNoTracking
                    .OrderByDescending(x => x.DateCreated);

                var result = orgRequests
                    .ToList()
                    .Select(x => Mapper.Map<OrgRequestDTO>(x))
                    .ToList();

                MemoryCacher.Add(CACHE_KEY, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (List<OrgRequestDTO>)cacheEntry;
                return new CachedResult<List<OrgRequestDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/orgRequests/{id}
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var orgRequest = UnitOfWork.OrgRequestsRepository.Find(id);
                if (orgRequest == null)
                    return NotFound();

                var result = Mapper.Map<OrgRequestDTO>(orgRequest);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (OrgRequestDTO)cacheEntry;
                return new CachedResult<OrgRequestDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/orgRequests
        [HttpPost]
        [OverrideAuthorization]
        [Authorize(Roles = "Organisation user")]
        public IHttpActionResult Post([FromBody]OrgRequestDTO model)
        {
            if (CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("organisation requests can only be made by mobile users");

            if (string.IsNullOrEmpty(model.Name))
                return BadRequest("organisation name is required");

            var existingOrg = UnitOfWork.OrganisationRepository
                .AllAsNoTracking
                .Where(x => x.Name == model.Name)
                .FirstOrDefault();

            if (existingOrg != null)
                return BadRequest("an organisation with this name already exists");

            var requestCount = UnitOfWork.OrgRequestsRepository
                .AllAsNoTracking
                .Count(x => x.OrgUserId == CurrentOrgUser.Id && x.Name == model.Name);

            if (requestCount > 0)
                return BadRequest("you have already requested this organisation");

            try
            {
                var orgRequest = new OrgRequest
                {
                    Name = model.Name,
                    Address = model.Address,
                    ContactName = model.ContactName,
                    Email = model.Email,
                    TelNumber = model.TelNumber,
                    Postcode = model.Postcode,
                    OrgUserId = CurrentOrgUser.Id
                };

                UnitOfWork.OrgRequestsRepository.InsertOrUpdate(orgRequest);

                var onRecord = UnitOfWork.OrganisationRepository
                    .AllAsNoTracking
                    .Where(x => x.Name == "OnRecord")
                    .SingleOrDefault();

                var rootIndex = WebHelpers.GetRootIndexPath();
                var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/organisations/requests/";

                var content = @"<p>User name: <b>" + CurrentOrgUser.UserName + @"</b></p>
                            <p>Organisation: <b>" + model.Name + @"</b></p>
                            <p><br></p>
                            <p>Go to <a href='" + url + @"'>organization requests</a> on the platform menu.</p>";

                if (onRecord.RootUserId.HasValue)
                {
                    var onRecordAdmin = UnitOfWork.OrgUsersRepository.AllAsNoTracking
                        .Where(x => x.Id == onRecord.RootUserId.Value)
                        .SingleOrDefault();

                    var email = new Email
                    {
                        To = onRecordAdmin.Email,
                        Subject = $"We have received an application to set up a new organisation",
                        Content = WebHelpers.GenerateEmailTemplate(content, "Setting up a new organisation")
                    };

                    UnitOfWork.EmailsRepository.InsertOrUpdate(email);
                }

                // find email recipients
                var recipients = UnitOfWork.EmailRecipientsRepository
                    .AllAsNoTracking
                    .Where(x => x.OrgRequests == true)
                    .ToList();

                foreach (var recipient in recipients)
                {
                    var recipientEmail = new Email
                    {
                        To = recipient.OrgUser.Email,
                        Subject = $"We have had a request to set up a new organisation",
                        Content = WebHelpers.GenerateEmailTemplate(content, "Setting up a new organisation")
                    };

                    UnitOfWork.EmailsRepository.InsertOrUpdate(recipientEmail);
                }

                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/orgRequests/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            try
            {
                UnitOfWork.OrgRequestsRepository.Delete(id);
                UnitOfWork.OrgRequestsRepository.Save();

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
