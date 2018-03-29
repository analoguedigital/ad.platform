using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class FeedbackController : BaseApiController
    {
        FeedbacksRepository Feedbacks
        {
            get { return this.UnitOfWork.FeedbacksRepository; }
        }

        EmailsRepository Emails
        {
            get { return this.UnitOfWork.EmailsRepository; }
        }

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

                var rootAdmin = this.UnitOfWork.OrgUsersRepository.AllAsNoTracking.Where(x => x.IsRootUser).FirstOrDefault();
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
                    <p>New feedback from: <strong>" + this.CurrentOrgUser.UserName + @"</strong></p>
                    <br>

                    <p>" + feedback.Comment + @"</p>

                    <br><br>
                    <p style='color: gray; font-size: small;'>Copyright &copy; 2018. analogueDIGITAL platform</p>
                </div>

                </body></html>";

                var email = new Email
                {
                    To = rootAdmin.Email,
                    Subject = $"New feedback posted - {this.CurrentOrgUser.UserName}",
                    Content = messageBody
                };

                this.UnitOfWork.EmailsRepository.InsertOrUpdate(email);
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
    }
}
