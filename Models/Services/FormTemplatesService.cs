using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LightMethods.Survey.Models.Services
{
    public class FormTemplatesService
    {

        #region Properties

        private UnitOfWork UnitOfWork { get; set; }

        public OrgUser OrgUser { get; set; }

        public User CurrentUser { get; set; }

        #endregion Properties

        #region C-tor

        public FormTemplatesService(UnitOfWork uow, OrgUser user, User currentUser)
        {
            UnitOfWork = uow;
            OrgUser = user as OrgUser;
            CurrentUser = currentUser;
        }

        #endregion C-tor

        public IEnumerable<FormTemplateDTO> Get(Guid? projectId, FormTemplateDiscriminators discriminator)
        {
            var surveyProvider = new SurveyProvider(OrgUser, UnitOfWork, false);
            var templates = surveyProvider.GetAllProjectTemplates(projectId, discriminator);

            var result = new List<FormTemplateDTO>();

            foreach (var template in templates)
            {
                var dto = Mapper.Map<FormTemplateDTO>(template);

                if (OrgUser != null)
                {
                    var assignment = UnitOfWork.FormTemplatesRepository.GetUserAssignment(template, OrgUser.Id);
                    Mapper.Map(assignment, dto);
                }
                else
                {
                    dto.CanView = true;
                    dto.CanEdit = true;
                    dto.CanAdd = true;
                    dto.CanDelete = true;
                }

                result.Add(dto);
            }

            return result;
        }

        public FormTemplateDTO Get(Guid id)
        {
            if (id == Guid.Empty)
                return Mapper.Map<FormTemplateDTO>(new FormTemplate() { });

            var surveyProvider = new SurveyProvider(OrgUser, UnitOfWork, onlyPublished: false);
            var form = surveyProvider.GetAllFormTemplates().Where(f => f.Id == id).SingleOrDefault();
            if (form == null) return null;

            var result = Mapper.Map<FormTemplateDTO>(form);

            if (OrgUser != null)
            {
                var assignment = UnitOfWork.FormTemplatesRepository.GetUserAssignment(form, OrgUser.Id);
                Mapper.Map(assignment, result);
            }
            else
            {
                result.CanView = true;
                result.CanAdd = true;
                result.CanEdit = true;
                result.CanDelete = true;
            }

            return result;
        }

        public OperationResult Create(FormTemplate entity)
        {
            var result = new OperationResult();

            // TODO: Prevent reinserting the existing formTemplateCategory

            // TODO: Set current project
            // formTemplate.ProjectId = UnitOfWork.ProjectsRepository.All.Where(p => p.OrganisationId == CurrentOrgUser.OrganisationId).FirstOrDefault().Id;

            try
            {
                entity.FormTemplateCategory = null;

                if (OrgUser != null)
                {
                    entity.CreatedById = OrgUser.Id;
                    entity.OrganisationId = OrgUser.Organisation.Id;
                }
                else
                {
                    entity.CreatedById = CurrentUser.Id;

                    // organisation and project are bound to dropdowns,
                    // and we have the IDs. so remove the DTO mappings to avoid EF validation errors.
                    entity.Organisation = null;
                    entity.Project = null;
                }

                UnitOfWork.FormTemplatesRepository.InsertOrUpdate(entity);
                UnitOfWork.Save();

                result.Success = true;
                result.Message = "Form Template created";
                result.NewRecordId = entity.Id;
                result.ReturnValue = Mapper.Map<FormTemplateDTO>(entity);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                        result.ValidationErrors.Add(
                            new ValidationResult(validationError.ErrorMessage, new string[] { (entityError.Entry.Entity as Entity)?.Id.ToString() ?? string.Empty }));
                }

                result.Success = false;
                result.Message = "Validation failed";
            }
            catch (DbUpdateException ex)
            {
                result.ValidationErrors.Add(new ValidationResult(ex.Message, new string[] { entity.Id.ToString() }));
                result.Success = false;
                result.Message = ex.Message;
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Error occured updating form template";
            }

            return result;
        }

        public OperationResult Update(Guid templateId, FormTemplateDTO model)
        {
            var result = new OperationResult();

            var surveyProvider = new SurveyProvider(OrgUser, UnitOfWork, onlyPublished: false);
            var form = surveyProvider.GetAllFormTemplates().Where(f => f.Id == templateId).SingleOrDefault();
            if (form == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            Mapper.Map(model, form);

            if (model.Organisation != null)
                form.OrganisationId = Guid.Parse(model.Organisation.Id);

            if (model.Project != null)
                form.ProjectId = model.Project.Id;

            // validate organisation. Project/Category organisations must match the selected organisation.

            UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);

            var groupOrder = 1;
            foreach (var valueGroup in model.MetricGroups)
            {
                var group = form.MetricGroups.SingleOrDefault(g => g.Id == valueGroup.Id);
                if (valueGroup.isDeleted && group != null)
                {
                    if (valueGroup.Metrics.Any(m => !m.isDeleted))
                        result.ValidationErrors.Add(new ValidationResult($"Group {group.Title} is not empty!", new string[] { group.Id.ToString() }));

                    //result.Errors.Add(group.Id.ToString(), $"Group {group.Title} is not empty!");
                }

                if (CurrentUser is OrgUser)
                    group = valueGroup.Map(group, UnitOfWork, OrgUser.Organisation);
                else if (CurrentUser is SuperUser)
                    group = valueGroup.Map(group, UnitOfWork, form.Organisation);

                group.FormTemplateId = form.Id;
                group.Order = groupOrder++;

                var metricOrder = 1;
                foreach (var valueMetric in valueGroup.Metrics)
                {
                    var metric = group.Metrics.Where(m => m.Id == valueMetric.Id)
                        .SingleOrDefault();

                    if (metric == null && valueMetric.isDeleted)
                        continue;

                    if (CurrentUser is OrgUser)
                        metric = valueMetric.Map(metric, UnitOfWork, OrgUser.Organisation);
                    else if (CurrentUser is SuperUser)
                        metric = valueMetric.Map(metric, UnitOfWork, form.Organisation);

                    if (valueMetric.isDeleted) // Delete
                        UnitOfWork.MetricsRepository.Delete(metric);
                    else
                    {   // Insert or update
                        metric.FormTemplateId = form.Id;
                        metric.MetricGroup = group;
                        metric.Order = metricOrder++;

                        // Validate metric
                        var validationResult = metric.Validate();
                        if (validationResult.Any())
                        {
                            validationResult.ToList().ForEach(res =>
                                result.ValidationErrors.Add(new ValidationResult(res.ErrorMessage, new string[] { metric.Id.ToString() })));

                            //validationResult.ToList().ForEach(res => result.Errors.Add(metric.Id.ToString(), res.ErrorMessage));
                        }

                        UnitOfWork.MetricsRepository.InsertOrUpdate(metric);
                    }
                }

                if (valueGroup.isDeleted && group != null)
                    UnitOfWork.MetricGroupsRepository.Delete(group);
                else
                    UnitOfWork.MetricGroupsRepository.InsertOrUpdate(group);
            }

            if (result.ValidationErrors.Any())
            {
                result.Success = false;
                result.Message = "Validation failed";

                return result;
            }

            //result.Errors = new Dictionary<string, string>();
            var formErrors = form.Validate(new ValidationContext(form));
            if (formErrors.Any())
            {
                formErrors.ToList().ForEach(res => result.ValidationErrors.Add(new ValidationResult(res.ErrorMessage, new string[] { templateId.ToString() })));
                result.Success = false;
                result.Message = "Validation failed";

                return result;
            }

            try
            {
                UnitOfWork.Save();

                result.Success = true;
                result.Message = "Form Templated updated";
                result.ReturnValue = form;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                        result.ValidationErrors.Add(
                            new ValidationResult(validationError.ErrorMessage, new string[] { (entityError.Entry.Entity as Entity)?.Id.ToString() ?? string.Empty }));
                }

                result.Success = false;
                result.Message = "Validation failed";
            }
            catch (DbUpdateException ex)
            {
                result.ValidationErrors.Add(new ValidationResult(ex.Message, new string[] { form.Id.ToString() }));
                result.Success = false;
                result.Message = ex.Message;
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Error occured updating form template";
            }

            return result;
        }

        public OperationResult UpdateBasicDetails(Guid templateId, EditBasicDetailsReqDTO model)
        {
            var result = new OperationResult();

            var surveyProvider = new SurveyProvider(OrgUser, UnitOfWork, false); ;
            var form = surveyProvider.GetAllFormTemplates().Where(f => f.Id == templateId).SingleOrDefault();
            if (form == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            try
            {
                Mapper.Map(model, form);
                UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);
                UnitOfWork.Save();

                result.Success = true;
                result.Message = "Basic Details updated";
                result.ReturnValue = Mapper.Map<FormTemplateDTO>(form);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                        result.ValidationErrors.Add(new ValidationResult(validationError.ErrorMessage, new string[] { (entityError.Entry.Entity as Entity)?.Id.ToString() ?? string.Empty }));
                }

                result.Success = false;
                result.Message = "Validation failed";
            }
            catch (DbUpdateException ex)
            {
                result.ValidationErrors.Add(new ValidationResult(ex.Message, new string[] { form.Id.ToString() }));
                result.Success = false;
                result.Message = "Validation failed";
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Error occured updating form template";
            }

            return result;
        }

        public OperationResult Delete(Guid id)
        {
            var result = new OperationResult();

            var surveyProvider = new SurveyProvider(OrgUser, UnitOfWork, false);
            var form = surveyProvider.GetAllFormTemplates().Where(f => f.Id == id).SingleOrDefault();
            if (form == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            try
            {
                UnitOfWork.FormTemplatesRepository.Delete(form);
                UnitOfWork.Save();

                result.Success = true;
                result.Message = "Form Template deleted";
            }
            catch (DbUpdateException)
            {
                result.Success = false;
                result.Message = "This Form cannot be deleted!";
            }

            return result;
        }

        public OperationResult ForceDelete(Guid id)
        {
            var result = new OperationResult();

            var surveyProvider = new SurveyProvider(OrgUser, UnitOfWork, false);
            var form = surveyProvider.GetAllFormTemplates().Where(f => f.Id == id).SingleOrDefault();
            if (form == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            try
            {
                // delete filled forms
                var surveys = UnitOfWork.FilledFormsRepository
                    .AllAsNoTracking
                    .Where(x => x.FormTemplateId == id)
                    .ToList();

                var attachments = new List<Attachment>();

                foreach (var survey in surveys)
                {
                    foreach (var fv in survey.FormValues)
                    {
                        if (fv.Attachments.Any())
                        {
                            attachments.AddRange(fv.Attachments.ToList());
                        }
                    }
                }

                UnitOfWork.AttachmentsRepository.Delete(attachments);
                UnitOfWork.FilledFormsRepository.Delete(surveys);
                UnitOfWork.Save();

                // delete metrics & metric groups
                var metrics = UnitOfWork.MetricsRepository
                    .All
                    .Where(x => x.FormTemplateId == id)
                    .ToList();

                var metricGroups = UnitOfWork.MetricGroupsRepository
                    .All
                    .Where(x => x.FormTemplateId == id)
                    .ToList();

                UnitOfWork.MetricsRepository.Delete(metrics);
                UnitOfWork.MetricGroupsRepository.Delete(metricGroups);
                UnitOfWork.Save();

                // delete the form template itself
                UnitOfWork.FormTemplatesRepository.Delete(id);
                UnitOfWork.Save();

                result.Success = true;
                result.Message = "Form Template deleted";
            }
            catch (DbUpdateException)
            {
                result.Success = false;
                result.Message = "This Form cannot be deleted!";
            }

            return result;
        }

        public OperationResult Clone(Guid id, CloneReqDTO request)
        {
            var result = new OperationResult();

            var template = UnitOfWork.FormTemplatesRepository.Find(id);
            //var clone = UnitOfWork.FormTemplatesRepository.Clone(template, OrgUser as OrgUser, request.Title, request.Colour, request.ProjectId);
            var clone = UnitOfWork.FormTemplatesRepository.Clone(template, CurrentUser.Id, request.Title, request.Colour, request.ProjectId);

            result.Success = true;
            result.Message = "Form Template cloned";
            result.ReturnValue = Mapper.Map<FormTemplateDTO>(clone);

            return result;
        }

        public OperationResult Publish(Guid id)
        {
            var result = new OperationResult();

            var template = UnitOfWork.FormTemplatesRepository.Find(id);
            if (template == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            var publishResult = template.Publish();
            if (publishResult.Any())
            {
                publishResult.ToList().ForEach(res => result.ValidationErrors.Add(new ValidationResult(res.ErrorMessage, new string[] { id.ToString() })));
                result.Success = false;
                result.Message = "Validation failed";

                return result;
            }

            try
            {
                result.ValidationErrors.Clear();
                UnitOfWork.FormTemplatesRepository.InsertOrUpdate(template);
                UnitOfWork.Save();

                result.Success = true;
                result.Message = "Form Template published";
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                        result.ValidationErrors.Add(new ValidationResult(validationError.ErrorMessage, new string[] { (entityError.Entry.Entity as Entity)?.Id.ToString() ?? string.Empty }));
                }

                result.Success = false;
                result.Message = "Validation failed";
            }
            catch (DbUpdateException ex)
            {
                result.ValidationErrors.Add(new ValidationResult(ex.Message, new string[] { template.Id.ToString() }));
                result.Success = false;
                result.Message = "Validation failed";
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Error occured updating form template";
            }

            return result;
        }

        public OperationResult Unpublish(Guid id)
        {
            var result = new OperationResult();

            var template = UnitOfWork.FormTemplatesRepository.Find(id);
            if (template == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            try
            {
                template.IsPublished = false;
                UnitOfWork.FormTemplatesRepository.InsertOrUpdate(template);
                UnitOfWork.Save();

                result.Success = true;
                result.Message = "Form Template archived";
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                        result.ValidationErrors.Add(new ValidationResult(validationError.ErrorMessage, new string[] { (entityError.Entry.Entity as Entity)?.Id.ToString() ?? string.Empty }));
                }

                result.Success = false;
                result.Message = "Validation failed";
            }
            catch (DbUpdateException ex)
            {
                result.ValidationErrors.Add(new ValidationResult(ex.Message, new string[] { template.Id.ToString() }));
                result.Success = false;
                result.Message = "Validation failed";
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Error occured updating form template";
            }

            return result;
        }

    }
}
