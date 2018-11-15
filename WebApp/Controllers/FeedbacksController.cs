using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Organisation user")]
    public class FeedbackController : BaseApiController
    {

        // POST api/feedbacks
        [HttpPost]
        [Route("api/feedbacks")]
        public IHttpActionResult Post(FeedbackDTO model)
        {
            var feedback = Mapper.Map<Feedback>(model);
            feedback.AddedAt = DateTimeService.UtcNow;
            feedback.AddedById = CurrentOrgUser.Id;
            feedback.OrganisationId = CurrentOrganisation.Id;

            ModelState.Clear();
            Validate(feedback);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                UnitOfWork.FeedbacksRepository.InsertOrUpdate(feedback);

                var onrecord = UnitOfWork.OrganisationRepository
                    .AllAsNoTracking
                    .Where(x => x.Name == "OnRecord")
                    .SingleOrDefault();

                if (onrecord.RootUserId.HasValue)
                {
                    var onRecordAdmin = UnitOfWork.OrgUsersRepository.Find(onrecord.RootUserId.Value);
                    var content = $"<p>{feedback.Comment}</p>";

                    var adminEmail = new Email
                    {
                        To = onrecord.RootUser.Email,
                        Subject = $"New feedback arrived - {CurrentOrgUser.UserName}",
                        Content = WebHelpers.GenerateEmailTemplate(content, "User Feedback")
                    };

                    UnitOfWork.EmailsRepository.InsertOrUpdate(adminEmail);
                }

                // find feedback recipients
                var recipients = UnitOfWork.EmailRecipientsRepository.AllAsNoTracking
                    .Where(x => x.Feedbacks == true)
                    .ToList();

                // queue emails for all recipients
                foreach (var recipient in recipients)
                {
                    var recipientEmail = new Email
                    {
                        To = recipient.OrgUser.Email,
                        Subject = $"New feedback arrived - {CurrentOrgUser.UserName}",
                        Content = WebHelpers.GenerateEmailTemplate($"<p>{feedback.Comment}</p>", "User Feedback")
                    };

                    UnitOfWork.EmailsRepository.InsertOrUpdate(recipientEmail);
                }

                UnitOfWork.Save();

                return Ok();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                throw dbEx;
            }
        }

        // POST api/feedbacks/sendEmail
        [HttpPost]
        [Route("api/feedbacks/sendEmail")]
        public IHttpActionResult SendEmail(SendEmailDTO value)
        {
            if (string.IsNullOrEmpty(value.EmailAddress))
                return BadRequest("email address is required");

            if (string.IsNullOrEmpty(value.Body))
                return BadRequest("email body is required");

            var email = new Email
            {
                To = value.EmailAddress,
                Subject = string.Format("Message from OnRecord - {0}", CurrentUser.UserName),
                Content = WebHelpers.GenerateEmailTemplate(value.Body, "Message from OnRecord")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);

            try
            {
                UnitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
