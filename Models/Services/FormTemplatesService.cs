﻿using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services
{
    public class FormTemplatesService
    {
        private UnitOfWork unitOfWork { get; set; }
        public OrgUser OrgUser { get; set; }

        public FormTemplatesService(UnitOfWork uow, OrgUser user)
        {
            this.unitOfWork = uow;
            this.OrgUser = user;
        }

        public IEnumerable<FormTemplateDTO> Get(Guid? projectId)
        {
            var surveyProvider = new SurveyProvider(this.OrgUser, this.unitOfWork, false);
            var templates = surveyProvider.GetAllProjectTemplates(projectId);
            var result = templates.Select(t => Mapper.Map<FormTemplateDTO>(t));

            return result;
        }

        public FormTemplateDTO Get(Guid id)
        {
            if (id == Guid.Empty)
                return Mapper.Map<FormTemplateDTO>(new FormTemplate() { });

            var surveyProvider = new SurveyProvider(this.OrgUser, this.unitOfWork, false);
            var form = surveyProvider.GetAllFormTemplates().Where(f => f.Id == id).SingleOrDefault();
            if (form == null) return null;

            return Mapper.Map<FormTemplateDTO>(form);
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
                entity.CreatedById = this.OrgUser.Id;
                entity.OrganisationId = this.OrgUser.OrganisationId.Value;

                this.unitOfWork.FormTemplatesRepository.InsertOrUpdate(entity);
                this.unitOfWork.Save();

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

            var surveyProvider = new SurveyProvider(this.OrgUser, this.unitOfWork, false);
            var form = surveyProvider.GetAllFormTemplates().Where(f => f.Id == templateId).SingleOrDefault();
            if (form == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            Mapper.Map(model, form);
            this.unitOfWork.FormTemplatesRepository.InsertOrUpdate(form);

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

                group = valueGroup.Map(group, this.unitOfWork, this.OrgUser.Organisation);
                group.FormTemplateId = form.Id;
                group.Order = groupOrder++;

                var metricOrder = 1;
                foreach (var valueMetric in valueGroup.Metrics)
                {
                    var metric = group.Metrics.Where(m => m.Id == valueMetric.Id)
                        .SingleOrDefault();

                    if (metric == null && valueMetric.isDeleted)
                        continue;

                    metric = valueMetric.Map(metric, this.unitOfWork, this.OrgUser.Organisation);

                    if (valueMetric.isDeleted) // Delete
                        this.unitOfWork.MetricsRepository.Delete(metric);
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

                        this.unitOfWork.MetricsRepository.InsertOrUpdate(metric);
                    }
                }

                if (valueGroup.isDeleted && group != null)
                    this.unitOfWork.MetricGroupsRepository.Delete(group);
                else
                    this.unitOfWork.MetricGroupsRepository.InsertOrUpdate(group);
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
                this.unitOfWork.Save();

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

            var surveyProvider = new SurveyProvider(this.OrgUser, this.unitOfWork, false); ;
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
                this.unitOfWork.FormTemplatesRepository.InsertOrUpdate(form);
                this.unitOfWork.Save();

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

            var surveyProvider = new SurveyProvider(this.OrgUser, this.unitOfWork, false);
            var form = surveyProvider.GetAllFormTemplates().Where(f => f.Id == id).SingleOrDefault();
            if (form == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            try
            {
                this.unitOfWork.FormTemplatesRepository.Delete(form);
                this.unitOfWork.Save();

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

            var template = this.unitOfWork.FormTemplatesRepository.Find(id);
            var clone = this.unitOfWork.FormTemplatesRepository.Clone(template, this.OrgUser as OrgUser, request.Title, request.Colour, request.ProjectId);

            result.Success = true;
            result.Message = "Form Template cloned";
            result.ReturnValue = Mapper.Map<FormTemplateDTO>(clone);

            return result;
        }

        public OperationResult Publish(Guid id)
        {
            var result = new OperationResult();

            var template = this.unitOfWork.FormTemplatesRepository.Find(id);
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
                this.unitOfWork.FormTemplatesRepository.InsertOrUpdate(template);
                this.unitOfWork.Save();

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

            var template = this.unitOfWork.FormTemplatesRepository.Find(id);
            if (template == null)
            {
                result.Success = false;
                result.Message = "Not found";

                return result;
            }

            try
            {
                template.IsPublished = false;
                this.unitOfWork.FormTemplatesRepository.InsertOrUpdate(template);
                this.unitOfWork.Save();

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
