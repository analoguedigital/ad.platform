using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.MetricFilters;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class SurveysController : BaseApiController
    {
        FormTemplatesRepository Templates { get { return UnitOfWork.FormTemplatesRepository; } }
        FilledFormsRepository FilledForms { get { return UnitOfWork.FilledFormsRepository; } }
        FormValuesRepository FormValues { get { return UnitOfWork.FormValuesRepository; } }

        [DeflateCompression]
        [Route("api/surveys")]
        [ResponseType(typeof(IEnumerable<FilledFormDTO>))]
        public IHttpActionResult Get(FormTemplateDiscriminators discriminator, Guid? projectId = null)
        {
            //TODO: refactore to api/projects/{projectId}/surveys
            var foundSurveys = new List<FilledFormDTO>();

            if (this.CurrentUser is SuperUser)
            {
                var surveys = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                    .Where(s => s.FormTemplate.Discriminator == discriminator);

                if (projectId.HasValue && projectId != Guid.Empty)
                    surveys = surveys.Where(s => s.ProjectId == projectId);

                foundSurveys = surveys
                    .ToList()
                    .OrderByDescending(x => x.Date)
                    .Select(s => Mapper.Map<FilledFormDTO>(s))
                    .ToList();
            }

            if (this.CurrentUser is OrgUser)
            {
                var surveys = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                    .Where(s => s.FormTemplate.Discriminator == discriminator);

                if (projectId.HasValue && projectId != Guid.Empty)
                {
                    if (this.CurrentOrgUser.Type != OrgUserTypesRepository.Administrator)
                    {
                        var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == projectId);
                        if (assignment == null || !assignment.CanView)
                            return Unauthorized();
                    }

                    surveys = surveys.Where(s => s.ProjectId == projectId);

                    foundSurveys = surveys
                        .ToList()
                        .OrderByDescending(x => x.Date)
                        .Select(s => Mapper.Map<FilledFormDTO>(s))
                        .ToList();
                }
                else
                {
                    // return all projects that this user has a case or thread assignment for.
                    var caseSurveys = surveys.Where(s => s.Project.Assignments.Any(a => a.OrgUserId == CurrentOrgUser.Id && a.CanView));
                    var threadSurveys = surveys.Where(s => s.FormTemplate.Assignments.Any(a => a.OrgUserId == CurrentOrgUser.Id && a.CanView));
                    
                    var joinedSurveys = new List<FilledForm>();
                    joinedSurveys.AddRange(caseSurveys.ToList());
                    joinedSurveys.AddRange(threadSurveys.ToList());

                    foundSurveys = joinedSurveys
                        .ToList()
                        .OrderByDescending(x => x.Date)
                        .Select(s => Mapper.Map<FilledFormDTO>(s))
                        .ToList()
                        .Distinct()
                        .ToList();
                }
            }

            return Ok(foundSurveys);
        }

        [DeflateCompression]
        [Route("api/surveys/user/{projectId}")]
        [ResponseType(typeof(IEnumerable<FilledFormDTO>))]
        public IHttpActionResult GetUserSurveys(Guid projectId)
        {
            if (projectId == null || projectId == Guid.Empty)
                return BadRequest("Project not found");

            var project = this.UnitOfWork.ProjectsRepository.Find(projectId);
            if (project == null)
                return BadRequest("Project not found");

            var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == projectId);
            if (assignment == null || !assignment.CanView)
                return Unauthorized();

            // we want records from advice threads too, so can't filter by FilledById.
            // this might need a refactoring but works for now.

            //var surveys = this.UnitOfWork.FilledFormsRepository.AllAsNoTracking
            //    .Where(s => s.ProjectId == projectId && s.FilledById == this.CurrentOrgUser.Id)
            //    .OrderByDescending(s => s.DateCreated);

            var surveys = this.UnitOfWork.FilledFormsRepository.AllAsNoTracking
                .Where(s => s.ProjectId == projectId)
                .OrderByDescending(s => s.DateCreated);

            var result = surveys.ToList()
                .Select(s => Mapper.Map<FilledFormDTO>(s));

            return Ok(result);
        }

        [HttpPost]
        [DeflateCompression]
        [Route("api/surveys/search")]
        [ResponseType(typeof(IEnumerable<FilledFormDTO>))]
        public IHttpActionResult Search(SearchDTO model)
        {
            var project = this.UnitOfWork.ProjectsRepository.Find(model.ProjectId);
            if (project == null)
                return NotFound();

            if (this.CurrentUser is OrgUser)
            {
                var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == model.ProjectId);
                if (assignment == null || !assignment.CanView)
                    return Unauthorized();
            }

            var result = this.UnitOfWork.FilledFormsRepository.Search(model).OrderByDescending(r => r.Date);
            var retVal = result.Select(s => Mapper.Map<FilledFormDTO>(s)).ToList();

            return Ok(retVal);
        }

        [DeflateCompression]
        [Route("api/projects/{projectId}/formTemplates/{formTemplateId}/data")]
        [ResponseType(typeof(IEnumerable<IEnumerable<string>>))]
        // NOTE: we're not using the data view anymore. obsolete and can be removed.
        public IHttpActionResult GetDataView(Guid projectId, Guid formTemplateId)
        {
            var result = new List<List<string>>();
            using (var provider = new FormDataProvider(formTemplateId, projectId, ignoreRepeaters: true))
            {
                var dataTable = provider.GetDataTable(null, null);

                // Headers
                result.Add(dataTable.Columns.Cast<DataColumn>().Select(c => c.Caption).ToList());

                foreach (DataRow row in dataTable.Rows)
                    result.Add(row.ItemArray.Select(v => v.ToString()).ToList());
            }

            return Ok(result);
        }

        [DeflateCompression]
        [Route("api/surveys/{id}")]
        [ResponseType(typeof(FilledFormDTO))]
        public IHttpActionResult GetSurvey(Guid id)
        {
            var survey = UnitOfWork.FilledFormsRepository.Find(id);
            if (survey == null)
                return NotFound();

            if (this.CurrentOrgUser != null && !this.HasAccessToViewRecords(this.CurrentOrgUser, survey.FormTemplateId, survey.ProjectId))
                return Unauthorized();

            var result = Mapper.Map<FilledFormDTO>(survey);

            return Ok(result);
        }

        [HttpGet]
        [Route("api/surveys/{surveyId}/attachment/{id}")]
        public IHttpActionResult GetAttachment(Guid surveyId, Guid id)
        {
            var attachment = UnitOfWork.AttachmentsRepository.Find(id);
            if (attachment == null || attachment.FormValue.FilledFormId != surveyId)
                return NotFound();

            if (this.CurrentOrgUser != null)
            {
                var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == attachment.FormValue.FilledForm.ProjectId);
                if (assignment == null || !assignment.CanView)
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
            }

            var fileInfo = new FileInfo(Path.Combine(AttachmentsRepository.RootFolderPath, attachment.RelativeFolder, attachment.NameOnDisk));

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(System.IO.File.ReadAllBytes(fileInfo.FullName))
            };

            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = attachment.FileName
            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return Ok(result);
        }

        [HttpPost]
        [Route("api/surveys")]
        [NeedsActiveMonthlyQuota]
        public IHttpActionResult Post(FilledFormDTO survey)
        {
            if (this.CurrentOrgUser != null)
            {
                if (!this.HasAccessToAddRecords(this.CurrentOrgUser, survey.FormTemplateId, survey.ProjectId))
                    return Unauthorized();   
            }

            var filledForm = Mapper.Map<FilledForm>(survey);
            
            //filledForm.FilledById = CurrentOrgUser.Id;
            if (this.CurrentOrgUser != null)
                filledForm.FilledById = CurrentOrgUser.Id;
            else
                filledForm.FilledById = CurrentUser.Id;

            try
            {
                UnitOfWork.FilledFormsRepository.InsertOrUpdate(filledForm);

                foreach (var val in filledForm.FormValues.Where(v => UnitOfWork.MetricsRepository.Find(v.MetricId.Value) is DateMetric))
                {
                    var dateMetric = val.Metric as DateMetric;
                    if (val.DateValue.HasValue && !dateMetric.HasTimeValue)
                    {
                        var localValue = TimeZone.CurrentTimeZone.ToLocalTime(val.DateValue.Value);
                        val.DateValue = new DateTime(localValue.Year, localValue.Month, localValue.Day, 0, 0, 0, DateTimeKind.Utc);
                    }
                }

                foreach (var val in filledForm.FormValues.Where(v => UnitOfWork.MetricsRepository.Find(v.MetricId.Value) is AttachmentMetric))
                {
                    if (val.TextValue == string.Empty) continue;

                    var fileInfos = val.TextValue.Split(',')
                              .Select(i => HttpContext.Current.Server.MapPath("~/Uploads/" + i)).Select(path => new DirectoryInfo(path).GetFiles().FirstOrDefault());

                    var attachments = fileInfos.Select(fileInfo => UnitOfWork.AttachmentsRepository.CreateAttachment(fileInfo, val));
                    UnitOfWork.AttachmentsRepository.InsertOrUpdate(attachments);

                    val.TextValue = string.Empty;
                }

                UnitOfWork.Save(true);
                filledForm.FormValues.SelectMany(v => v.Attachments).ToList()
                    .ForEach(attachment => UnitOfWork.AttachmentsRepository.StoreFile(attachment));
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

        [HttpPut]
        [Route("api/surveys/{id}")]
        public IHttpActionResult Put(Guid id, FilledFormDTO surveyDTO)
        {
            if (this.CurrentOrgUser != null)
            {
                if (!this.HasAccessToEditRecords(this.CurrentOrgUser, surveyDTO.FormTemplateId, surveyDTO.ProjectId))
                    return Unauthorized();
            }

            var survey = Mapper.Map<FilledForm>(surveyDTO);
            ModelState.Clear();
            Validate(survey);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dbForm = UnitOfWork.FilledFormsRepository.Find(id);
            dbForm.SurveyDate = survey.SurveyDate;
            UnitOfWork.FilledFormsRepository.InsertOrUpdate(dbForm);

            foreach (var val in surveyDTO.FormValues)
            {
                var dbrecord = FormValues.Find(val.Id);
                if (dbrecord == null)
                {
                    dbrecord = new FormValue()
                    {
                        FilledFormId = dbForm.Id,
                        MetricId = val.MetricId,
                        RowDataListItemId = val.RowDataListItemId,
                        RowNumber = val.RowNumber
                    };

                }

                dbrecord.NumericValue = val.NumericValue;
                dbrecord.BoolValue = val.BoolValue;
                dbrecord.DateValue = val.DateValue;
                dbrecord.TimeValue = val.TimeValue.HasValue ? new TimeSpan(val.TimeValue.Value.Hour, val.TimeValue.Value.Minute, 0) : (TimeSpan?)null;
                dbrecord.TextValue = val.TextValue;
                dbrecord.GuidValue = val.GuidValue;
                dbrecord.RowNumber = val.RowNumber;

                if (dbrecord.Metric is DateMetric)
                {
                    var dateMetric = dbrecord.Metric as DateMetric;
                    if (dbrecord.DateValue.HasValue && !dateMetric.HasTimeValue)
                    {
                        var localDate = TimeZone.CurrentTimeZone.ToLocalTime(dbrecord.DateValue.Value);
                        dbrecord.DateValue = new DateTime(localDate.Year, localDate.Month, localDate.Day, 0, 0, 0, DateTimeKind.Utc);
                    }
                }

                if (dbrecord.Metric is AttachmentMetric)
                {
                    var deletedAttachments = val.Attachments.Where(a => a.isDeleted).ToList();
                    foreach (var item in deletedAttachments)
                        UnitOfWork.AttachmentsRepository.Delete(item.Id);

                    var fileInfos = val.TextValue == string.Empty ? new List<FileInfo>() : val.TextValue.Split(',')
                                         .Select(i => HttpContext.Current.Server.MapPath("~/Uploads/" + i)).Select(path => new DirectoryInfo(path).GetFiles().FirstOrDefault());

                    var attachments = fileInfos.Select(fileInfo => UnitOfWork.AttachmentsRepository.CreateAttachment(fileInfo, dbrecord));
                    UnitOfWork.AttachmentsRepository.InsertOrUpdate(attachments);

                    dbrecord.TextValue = string.Empty;
                }

                FormValues.InsertOrUpdate(dbrecord);
            }

            UnitOfWork.Save();

            var deletedFormValues = dbForm.FormValues.Where(dv => !survey.FormValues.Any(v => v.Id == dv.Id)).ToList();
            if (deletedFormValues.Any())
            {
                foreach (var deletedFormValue in deletedFormValues)
                    FormValues.Delete(deletedFormValue);
                UnitOfWork.Save();
            }

            var tempAttachments = dbForm.FormValues.SelectMany(v => v.Attachments).Where(a => a.IsTemp).ToList();
            if (tempAttachments.Any())
            {
                foreach (var attachment in tempAttachments)
                    UnitOfWork.AttachmentsRepository.StoreFile(attachment);
                UnitOfWork.Save();
            }

            return Ok();
        }

        // DELETE api/<controller>/5
        [Route("api/surveys/{id}")]
        public IHttpActionResult Delete(Guid id)
        {
            var survey = UnitOfWork.FilledFormsRepository.Find(id);
            if (survey == null)
                return NotFound();

            if (this.CurrentOrgUser != null)
            {
                if (!this.HasAccessToDeleteRecords(this.CurrentOrgUser, survey.FormTemplateId, survey.ProjectId))
                    return Unauthorized();
            }

            UnitOfWork.FilledFormsRepository.Delete(survey);
            UnitOfWork.FilledFormsRepository.Save();

            return Ok();
        }

        #region helpers

        private bool HasAccessToViewRecords(OrgUser orgUser, Guid threadId, Guid caseId)
        {
            var threadAssignment = this.CurrentOrgUser.ThreadAssignments.SingleOrDefault(a => a.FormTemplateId == threadId);
            var projectAssignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == caseId);

            if (projectAssignment == null && threadAssignment == null)
                return false;

            var isAuthorized = false;
            if (projectAssignment != null)
                isAuthorized = projectAssignment.CanView;

            if (threadAssignment != null)
                isAuthorized = threadAssignment.CanView;

            return isAuthorized;
        }

        private bool HasAccessToAddRecords(OrgUser orgUser, Guid threadId, Guid caseId)
        {
            var threadAssignment = this.CurrentOrgUser.ThreadAssignments.SingleOrDefault(a => a.FormTemplateId == threadId);
            var projectAssignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == caseId);

            if (projectAssignment == null && threadAssignment == null)
                return false;

            var isAuthorized = false;
            if (projectAssignment != null)
                isAuthorized = projectAssignment.CanAdd;

            if (threadAssignment != null)
                isAuthorized = threadAssignment.CanAdd;

            return isAuthorized;
        }

        private bool HasAccessToEditRecords(OrgUser orgUser, Guid threadId, Guid caseId)
        {
            var threadAssignment = this.CurrentOrgUser.ThreadAssignments.SingleOrDefault(a => a.FormTemplateId == threadId);
            var projectAssignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == caseId);

            if (projectAssignment == null && threadAssignment == null)
                return false;

            var isAuthorized = false;
            if (projectAssignment != null)
                isAuthorized = projectAssignment.CanEdit;

            if (threadAssignment != null)
                isAuthorized = threadAssignment.CanEdit;

            return isAuthorized;
        }

        private bool HasAccessToDeleteRecords(OrgUser orgUser, Guid threadId, Guid caseId)
        {
            var threadAssignment = this.CurrentOrgUser.ThreadAssignments.SingleOrDefault(a => a.FormTemplateId == threadId);
            var projectAssignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == caseId);

            if (projectAssignment == null && threadAssignment == null)
                return false;

            var isAuthorized = false;
            if (projectAssignment != null)
                isAuthorized = projectAssignment.CanDelete;

            if (threadAssignment != null)
                isAuthorized = threadAssignment.CanDelete;

            return isAuthorized;
        }

        #endregion helpers

    }
}