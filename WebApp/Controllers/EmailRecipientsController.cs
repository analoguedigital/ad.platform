using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator,Platform administrator")]
    public class EmailRecipientsController : BaseApiController
    {

        private const string CACHE_KEY = "EMAIL_RECIPIENTS";

        [ResponseType(typeof(IEnumerable<EmailRecipientDTO>))]
        [Route("api/email-recipients")]
        public IHttpActionResult Get()
        {
            var cacheEntry = MemoryCacher.GetValue(CACHE_KEY);
            if (cacheEntry == null)
            {
                var recipients = UnitOfWork.EmailRecipientsRepository
                .AllAsNoTracking
                .ToList();

                var result = recipients
                    .Select(x => Mapper.Map<EmailRecipientDTO>(x))
                    .ToList();

                MemoryCacher.Add(CACHE_KEY, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (List<EmailRecipientDTO>)cacheEntry;
                return new CachedResult<List<EmailRecipientDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        [ResponseType(typeof(EmailRecipientDTO))]
        [Route("api/email-recipients/{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var recipient = UnitOfWork.EmailRecipientsRepository.Find(id);
                if (recipient == null)
                    return NotFound();

                var result = Mapper.Map<EmailRecipientDTO>(recipient);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (EmailRecipientDTO)cacheEntry;
                return new CachedResult<EmailRecipientDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/email-recipients/assign/{userId}/{flag}
        [HttpPost]
        [Route("api/email-recipients/assign/{userId:guid}/{flag}")]
        [ResponseType(typeof(IEnumerable<EmailRecipientDTO>))]
        public IHttpActionResult AddAssignments(Guid userId, EmailNotificationType flag)
        {
            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            var recipient = UnitOfWork.EmailRecipientsRepository.AssignEmailNotification(userId, flag, enabled: true);
            var result = Mapper.Map<EmailRecipientDTO>(recipient);

            // invalidate cache
            MemoryCacher.DeleteStartingWith(CACHE_KEY);

            return Ok(result);
        }

        // DEL api/email-recipients/assign/{userId}/{flag}
        [HttpDelete]
        [Route("api/email-recipients/assign/{userId:guid}/{flag}")]
        [ResponseType(typeof(IEnumerable<EmailRecipientDTO>))]
        public IHttpActionResult DeleteAssignments(Guid userId, EmailNotificationType flag)
        {
            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var recipient = UnitOfWork.EmailRecipientsRepository
                .AllAsNoTracking
                .SingleOrDefault(x => x.OrgUserId == userId);

            if (recipient == null)
                return NotFound();

            var entry = UnitOfWork.EmailRecipientsRepository.AssignEmailNotification(userId, flag, enabled: false);
            if (entry != null)
            {
                var result = Mapper.Map<EmailRecipientDTO>(entry);
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok(result);
            }

            return Ok(new EmailRecipientDTO());
        }

    }
}
