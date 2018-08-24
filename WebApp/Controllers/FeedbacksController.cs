﻿using AutoMapper;
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
    public class FeedbackController : BaseApiController
    {

        #region Properties

        FeedbacksRepository Feedbacks
        {
            get { return this.UnitOfWork.FeedbacksRepository; }
        }

        EmailsRepository Emails
        {
            get { return this.UnitOfWork.EmailsRepository; }
        }

        #endregion

        // POST api/feedbacks
        [HttpPost]
        [Route("api/feedbacks")]
        public IHttpActionResult Post(FeedbackDTO model)
        {
            var feedback = Mapper.Map<Feedback>(model);
            feedback.AddedAt = DateTimeService.UtcNow;
            feedback.AddedById = this.CurrentOrgUser.Id;
            feedback.OrganisationId = this.CurrentOrganisation.Id;

            ModelState.Clear();
            Validate(feedback);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                this.UnitOfWork.FeedbacksRepository.InsertOrUpdate(feedback);

                var onrecord = this.UnitOfWork.OrganisationRepository.AllAsNoTracking
                    .Where(x => x.Name == "OnRecord")
                    .SingleOrDefault();

                if (onrecord.RootUserId.HasValue)
                {
                    var onRecordAdmin = this.UnitOfWork.OrgUsersRepository.Find(onrecord.RootUserId.Value);
                    var content = $"<p>{feedback.Comment}</p>";

                    var adminEmail = new Email
                    {
                        To = onrecord.RootUser.Email,
                        Subject = $"New feedback arrived - {this.CurrentOrgUser.UserName}",
                        Content = WebHelpers.GenerateEmailTemplate(content, "User Feedback")
                    };
                    this.UnitOfWork.EmailsRepository.InsertOrUpdate(adminEmail);
                }

                this.UnitOfWork.Save();

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
                return BadRequest("Email address is required");

            if (string.IsNullOrEmpty(value.Body))
                return BadRequest("Body content is required");

            var email = new Email
            {
                To = value.EmailAddress,
                Subject = string.Format("Message from OnRecord - {0}", this.CurrentUser.UserName),
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
