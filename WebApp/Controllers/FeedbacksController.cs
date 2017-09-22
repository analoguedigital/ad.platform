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
                this.UnitOfWork.FeedbacksRepository.InsertOrUpdate(feedback);

                var orgAdmins = CurrentOrganisation.OrgUsers.Where(u => u.TypeId == OrgUserTypesRepository.Administrator.Id);
                foreach (var admin in orgAdmins)
                {
                    var email = new Email
                    {
                        To = admin.Email,
                        Subject = "New feedback posted",
                        Content = feedback.Comment
                    };

                    this.UnitOfWork.EmailsRepository.InsertOrUpdate(email);
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
    }
}
