using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebApi.Controllers
{
    public class EmailRecipientsController : BaseApiController
    {
        [ResponseType(typeof(IEnumerable<EmailRecipientDTO>))]
        [Route("api/email-recipients")]
        public IHttpActionResult Get()
        {
            var recipients = UnitOfWork.EmailRecipientsRepository.AllAsNoTracking.ToList();
            var result = recipients
                .Select(x => Mapper.Map<EmailRecipientDTO>(x))
                .ToList();

            return Ok(result);
        }

        [ResponseType(typeof(EmailRecipientDTO))]
        [Route("api/email-recipients/{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var recipient = UnitOfWork.EmailRecipientsRepository.Find(id);
            if (recipient == null)
                return NotFound();

            return Ok(Mapper.Map<EmailRecipientDTO>(recipient));
        }

        // POST api/email-recipients/assign/{userId}/{flag}
        [HttpPost]
        [Route("api/email-recipients/assign/{userId:guid}/{flag}")]
        [ResponseType(typeof(IEnumerable<EmailRecipientDTO>))]
        public IHttpActionResult AddAssignments(Guid userId, EmailNotificationType flag)
        {
            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            var result = UnitOfWork.EmailRecipientsRepository.AssignEmailNotification(userId, flag, enabled: true);

            return Ok(Mapper.Map<EmailRecipientDTO>(result));
        }

        // DEL api/email-recipients/assign/{userId}/{flag}
        [HttpDelete]
        [Route("api/email-recipients/assign/{userId:guid}/{flag}")]
        [ResponseType(typeof(IEnumerable<EmailRecipientDTO>))]
        public IHttpActionResult DeleteAssignments(Guid userId, EmailNotificationType flag)
        {
            var recipient = UnitOfWork.EmailRecipientsRepository.AllAsNoTracking.SingleOrDefault(x => x.OrgUserId == userId);
            if (recipient == null)
                return NotFound();

            var result = UnitOfWork.EmailRecipientsRepository.AssignEmailNotification(userId, flag, enabled: false);
            if (result != null)
                return Ok(Mapper.Map<EmailRecipientDTO>(result));

            return Ok(new EmailRecipientDTO());
        }

    }
}
