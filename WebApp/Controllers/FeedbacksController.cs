using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class FeedbackController : BaseApiController
    {
        FeedbacksRepository Feedbacks { get { return this.UnitOfWork.FeedbacksRepository; } }
        EmailsRepository Emails { get { return this.UnitOfWork.EmailsRepository; } }

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
                var email = new LightMethods.Survey.Models.Entities.Email
                {
                    To = this.CurrentOrgUser.Email,
                    Subject = "New feedback posted",
                    Content = "Hello, a new feedback has been posted."
                };

                this.UnitOfWork.FeedbacksRepository.InsertOrUpdate(feedback);
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
