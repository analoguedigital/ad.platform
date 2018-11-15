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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator,Organisation user")]
    public class SurveysController : BaseApiController
    {

        #region Properties

        private const string CACHE_KEY = "SURVEYS";
        private const string ADMIN_KEY = "ADMIN";
        private const string ORG_ADMIN_KEY = "ORG_ADMIN";

        #endregion

        #region CRUD

        // GET api/surveys
        [DeflateCompression]
        [Route("api/surveys")]
        [ResponseType(typeof(IEnumerable<FilledFormDTO>))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user,Restricted user")]
        public async Task<IHttpActionResult> Get(FormTemplateDiscriminators discriminator, Guid? projectId = null)
        {
            //TODO: refactor to api/projects/{projectId}/surveys
            var surveyService = new SurveyService(UnitOfWork);

            if (CurrentUser is OrgUser)
            {
                var isOrgAdmin = await ServiceContext.UserManager.IsInRoleAsync(CurrentOrgUser.Id, Role.ORG_ADMINSTRATOR);
                var cacheKey = string.Empty;

                if (isOrgAdmin)
                {
                    cacheKey = projectId.HasValue ?
                        $"{CACHE_KEY}_{ORG_ADMIN_KEY}_{discriminator}_{projectId}_{CurrentOrgUser.Id}" :
                        $"{CACHE_KEY}_{ORG_ADMIN_KEY}_{discriminator}_{CurrentOrgUser.Id}";
                }
                else
                {
                    cacheKey = projectId.HasValue ?
                        $"{CACHE_KEY}_{discriminator}_{projectId}_{CurrentOrgUser.Id}" :
                        $"{CACHE_KEY}_{discriminator}_{CurrentOrgUser.Id}";
                }

                var cacheEntry = MemoryCacher.GetValue(cacheKey);
                if (cacheEntry == null)
                {
                    var userSurveys = surveyService.GetUserSurveys(CurrentOrgUser, discriminator, projectId);
                    if (userSurveys == null)
                        return Unauthorized();

                    MemoryCacher.Add(cacheKey, userSurveys, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(userSurveys);
                }
                else
                {
                    var result = (List<FilledFormDTO>)cacheEntry;
                    return new CachedResult<List<FilledFormDTO>>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser
            var _cacheKey = projectId.HasValue ?
                   $"{CACHE_KEY}_{ADMIN_KEY}_{discriminator}_{projectId}" :
                   $"{CACHE_KEY}_{ADMIN_KEY}_{discriminator}";

            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);
            if (_cacheEntry == null)
            {
                var surveys = surveyService.GetAllSurveys(discriminator, projectId);
                MemoryCacher.Add(_cacheKey, surveys, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(surveys);
            }
            else
            {
                var result = (List<FilledFormDTO>)_cacheEntry;
                return new CachedResult<List<FilledFormDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/surveys/{id}
        [DeflateCompression]
        [Route("api/surveys/{id}")]
        [ResponseType(typeof(FilledFormDTO))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user,Restricted user")]
        public IHttpActionResult GetSurvey(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{id}_{CurrentOrgUser.Id}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var survey = UnitOfWork.FilledFormsRepository.Find(id);
                    if (survey == null)
                        return NotFound();

                    if (!HasAccessToViewRecords(survey.FormTemplateId, survey.ProjectId))
                        return Unauthorized();

                    var result = Mapper.Map<FilledFormDTO>(survey);
                    MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(result);
                }
                else
                {
                    var result = (FilledFormDTO)cacheEntry;
                    return new CachedResult<FilledFormDTO>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is SuperUser
            var _cacheKey = $"{CACHE_KEY}_{ADMIN_KEY}_{id}";
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var survey = UnitOfWork.FilledFormsRepository.Find(id);
                if (survey == null)
                    return NotFound();

                var result = Mapper.Map<FilledFormDTO>(survey);
                MemoryCacher.Add(_cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var response = (FilledFormDTO)_cacheEntry;
                return new CachedResult<FilledFormDTO>(response, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/surveys/user/{projectId}
        [DeflateCompression]
        [Route("api/surveys/user/{projectId}")]
        [ResponseType(typeof(IEnumerable<FilledFormDTO>))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user,Restricted user")]
        public IHttpActionResult GetUserSurveys(Guid projectId)
        {
            if (projectId == Guid.Empty)
                return BadRequest("project id is empty");

            var project = UnitOfWork.ProjectsRepository.Find(projectId);
            if (project == null)
                return NotFound();

            var assignment = CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == projectId);
            if (assignment == null || !assignment.CanView)
                return Unauthorized();

            // we want records from advice threads too, so can't filter by FilledById.
            // this might need a refactoring but works for now.

            //var surveys = UnitOfWork.FilledFormsRepository.AllAsNoTracking
            //    .Where(s => s.ProjectId == projectId && s.FilledById == CurrentOrgUser.Id)
            //    .OrderByDescending(s => s.DateCreated);

            var cacheKey = $"{CACHE_KEY}_{projectId}_{CurrentOrgUser.Id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var surveys = UnitOfWork.FilledFormsRepository
                    .AllAsNoTracking
                    .Where(s => s.ProjectId == projectId)
                    .OrderByDescending(s => s.DateCreated)
                    .ToList()
                    .Select(s => Mapper.Map<FilledFormDTO>(s))
                    .ToList();

                MemoryCacher.Add(cacheKey, surveys, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(surveys);
            }
            else
            {
                var result = (List<FilledFormDTO>)cacheEntry;
                return new CachedResult<List<FilledFormDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/surveys/search
        [HttpPost]
        [DeflateCompression]
        [Route("api/surveys/search")]
        [ResponseType(typeof(IEnumerable<FilledFormDTO>))]
        public IHttpActionResult Search(SearchDTO model)
        {
            if (model.ProjectId == Guid.Empty)
                return BadRequest("project id is empty");

            if (model.FormTemplateIds == null || !model.FormTemplateIds.Any())
                return BadRequest("form templates are empty");

            var project = UnitOfWork.ProjectsRepository.Find(model.ProjectId);
            if (project == null)
                return NotFound();

            if (CurrentUser is OrgUser)
            {
                var threadAssignments = UnitOfWork.ThreadAssignmentsRepository
                    .AllAsNoTracking
                    .Where(x => x.OrgUserId == CurrentOrgUser.Id)
                    .ToList();

                var projectFound = threadAssignments.Any(x => x.FormTemplate.Project.Id == model.ProjectId);
                var assignment = CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == model.ProjectId);

                if (!projectFound)
                {
                    if (assignment == null || !assignment.CanView)
                        return Unauthorized();
                }
            }

            var records = UnitOfWork.FilledFormsRepository.Search(model)
                .OrderByDescending(r => r.Date)
                .ToList();

            var result = records.Select(s => Mapper.Map<FilledFormDTO>(s)).ToList();

            return Ok(result);
        }

        // POST api/surveys
        [HttpPost]
        [Route("api/surveys")]
        [NeedsActiveMonthlyQuota]
        public IHttpActionResult Post(FilledFormDTO survey)
        {
            if (CurrentUser is OrgUser)
            {
                if (!HasAccessToAddRecords(survey.FormTemplateId, survey.ProjectId))
                    return Unauthorized();
            }

            var filledForm = Mapper.Map<FilledForm>(survey);
            filledForm.FilledById = CurrentUser.Id;

            foreach (var val in filledForm.FormValues.Where(v => UnitOfWork.MetricsRepository.Find(v.MetricId.Value) is DateMetric))
            {
                var dateMetric = val.Metric as DateMetric;  // this results in null, it shouldn't!
                if (dateMetric != null)
                {
                    if (val.DateValue.HasValue && !dateMetric.HasTimeValue)
                    {
                        var localValue = TimeZone.CurrentTimeZone.ToLocalTime(val.DateValue.Value);
                        val.DateValue = new DateTime(localValue.Year, localValue.Month, localValue.Day, 0, 0, 0, DateTimeKind.Utc);
                    }
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

            try
            {
                UnitOfWork.FilledFormsRepository.InsertOrUpdate(filledForm);
                UnitOfWork.Save(disableValidation: true);

                filledForm.FormValues
                    .SelectMany(v => v.Attachments)
                    .ToList()
                    .ForEach(attachment => UnitOfWork.AttachmentsRepository.StoreFile(attachment));

                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

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

        // PUT api/surveys/{id}
        [HttpPut]
        [Route("api/surveys/{id}")]
        public IHttpActionResult Put(Guid id, FilledFormDTO surveyDTO)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            if (CurrentUser is OrgUser)
            {
                if (!HasAccessToEditRecords(surveyDTO.FormTemplateId, surveyDTO.ProjectId))
                    return Unauthorized();
            }

            var survey = Mapper.Map<FilledForm>(surveyDTO);
            ModelState.Clear();
            Validate(survey);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var dbForm = UnitOfWork.FilledFormsRepository.Find(id);
                dbForm.SurveyDate = survey.SurveyDate;
                UnitOfWork.FilledFormsRepository.InsertOrUpdate(dbForm);

                foreach (var val in surveyDTO.FormValues)
                {
                    var dbrecord = UnitOfWork.FormValuesRepository.Find(val.Id);
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

                    UnitOfWork.FormValuesRepository.InsertOrUpdate(dbrecord);
                }

                UnitOfWork.Save();

                var deletedFormValues = dbForm.FormValues
                    .Where(dv => !survey.FormValues.Any(v => v.Id == dv.Id))
                    .ToList();

                if (deletedFormValues.Any())
                {
                    foreach (var deletedFormValue in deletedFormValues)
                        UnitOfWork.FormValuesRepository.Delete(deletedFormValue);

                    UnitOfWork.Save();
                }

                var tempAttachments = dbForm.FormValues
                    .SelectMany(v => v.Attachments)
                    .Where(a => a.IsTemp)
                    .ToList();

                if (tempAttachments.Any())
                {
                    foreach (var attachment in tempAttachments)
                        UnitOfWork.AttachmentsRepository.StoreFile(attachment);

                    UnitOfWork.Save();
                }

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/surveys/{id}
        [Route("api/surveys/{id}")]
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var survey = UnitOfWork.FilledFormsRepository.Find(id);
            if (survey == null)
                return NotFound();

            if (CurrentUser is OrgUser)
            {
                if (!HasAccessToDeleteRecords(survey.FormTemplateId, survey.ProjectId))
                    return Unauthorized();
            }

            try
            {
                UnitOfWork.FilledFormsRepository.Delete(survey);
                UnitOfWork.FilledFormsRepository.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion CRUD

        #region Obsolete Methods

        // GET api/surveys/{surveyId}/attachment/{id}
        [HttpGet]
        [Route("api/surveys/{surveyId}/attachment/{id}")]
        // TODO: this can be removed since we don't return attachment files directly.
        // the Downloads Controller is responsible for this, via OneTimeAccess Tokens.
        public IHttpActionResult GetAttachment(Guid surveyId, Guid id)
        {
            var attachment = UnitOfWork.AttachmentsRepository.Find(id);
            if (attachment == null || attachment.FormValue.FilledFormId != surveyId)
                return NotFound();

            if (CurrentOrgUser != null)
            {
                var assignment = CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == attachment.FormValue.FilledForm.ProjectId);
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

        // GET api/projects/{projectId}/formTemplates/{formTemplateId}/data
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

        #endregion Obsolete Methods

        #region Assignments Helpers

        private bool HasAccessToViewRecords(Guid threadId, Guid caseId)
        {
            var threadAssignment = CurrentOrgUser.ThreadAssignments.SingleOrDefault(a => a.FormTemplateId == threadId);
            var projectAssignment = CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == caseId);

            if (projectAssignment == null && threadAssignment == null)
                return false;

            var isAuthorized = false;
            if (threadAssignment != null)
                isAuthorized = threadAssignment.CanView;

            if (projectAssignment != null)
                isAuthorized = projectAssignment.CanView;

            return isAuthorized;
        }

        private bool HasAccessToAddRecords(Guid threadId, Guid caseId)
        {
            var threadAssignment = CurrentOrgUser.ThreadAssignments.SingleOrDefault(a => a.FormTemplateId == threadId);
            var projectAssignment = CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == caseId);

            if (projectAssignment == null && threadAssignment == null)
                return false;

            var isAuthorized = false;
            if (threadAssignment != null)
                isAuthorized = threadAssignment.CanAdd;

            if (projectAssignment != null)
                isAuthorized = projectAssignment.CanAdd;

            return isAuthorized;
        }

        private bool HasAccessToEditRecords(Guid threadId, Guid caseId)
        {
            var threadAssignment = CurrentOrgUser.ThreadAssignments.SingleOrDefault(a => a.FormTemplateId == threadId);
            var projectAssignment = CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == caseId);

            if (projectAssignment == null && threadAssignment == null)
                return false;

            var isAuthorized = false;
            if (threadAssignment != null)
                isAuthorized = threadAssignment.CanEdit;

            if (projectAssignment != null)
                isAuthorized = projectAssignment.CanEdit;

            return isAuthorized;
        }

        private bool HasAccessToDeleteRecords(Guid threadId, Guid caseId)
        {
            var threadAssignment = CurrentOrgUser.ThreadAssignments.SingleOrDefault(a => a.FormTemplateId == threadId);
            var projectAssignment = CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == caseId);

            if (projectAssignment == null && threadAssignment == null)
                return false;

            var isAuthorized = false;
            if (threadAssignment != null)
                isAuthorized = threadAssignment.CanDelete;

            if (projectAssignment != null)
                isAuthorized = projectAssignment.CanDelete;

            return isAuthorized;
        }

        #endregion Assignments Helpers

    }
}