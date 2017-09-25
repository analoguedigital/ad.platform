using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.FilterValues;
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
using WebApi.Models;

namespace WebApi.Controllers
{
    public class SurveysController : BaseApiController
    {
        FormTemplatesRepository Templates { get { return UnitOfWork.FormTemplatesRepository; } }
        FilledFormsRepository FilledForms { get { return UnitOfWork.FilledFormsRepository; } }
        FormValuesRepository FormValues { get { return UnitOfWork.FormValuesRepository; } }

        [Route("api/surveys")]
        [ResponseType(typeof(IEnumerable<FilledFormDTO>))]
        public IHttpActionResult Get(Guid projectId)
        {
            var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == projectId);
            if (assignment == null || !assignment.CanView)
                return Content(HttpStatusCode.Forbidden, "Access Denied");
            //TODO: refactore to api/projects/{projectId}/surveys

            var surveys = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                .Where(s => s.ProjectId == projectId)
                .ToList()
                .OrderByDescending(x => x.Date);


            var result = surveys.Select(s => Mapper.Map<FilledFormDTO>(s));

            return Ok(result);
        }

        [HttpPost]
        [Route("api/surveys/search")]
        [ResponseType(typeof(IEnumerable<FilledFormDTO>))]
        public IHttpActionResult Search(SearchDTO model)
        {
            var project = this.UnitOfWork.ProjectsRepository.Find(model.ProjectId);
            if (project == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(this.CurrentOrgUser.Id);
            if (orgUser == null)
                return NotFound();

            if (this.CurrentOrganisationId != project.OrganisationId || orgUser.OrganisationId != project.OrganisationId)
                return NotFound();

            var result = this.UnitOfWork.FilledFormsRepository.Search(model).OrderByDescending(r => r.Date);
            var retVal = result.Select(s => Mapper.Map<FilledFormDTO>(s)).ToList();

            return Ok(retVal);
        }

        [Route("api/projects/{projectId}/formTemplates/{formTemplateId}/data")]
        [ResponseType(typeof(IEnumerable<IEnumerable<string>>))]
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

        [Route("api/surveys/{id}")]
        [ResponseType(typeof(FilledFormDTO))]
        public IHttpActionResult GetSurvey(Guid id)
        {
            var survey = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                .Where(s => s.Id == id)
                .ToList()
                .Select(f => Mapper.Map<FilledFormDTO>(f))
                .SingleOrDefault();

            if (survey == null)
                return NotFound();

            var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == survey.ProjectId);
            if (assignment == null || !assignment.CanView)
                return Content(HttpStatusCode.Forbidden, "Access Denied");

            return Ok(survey);
        }

        [HttpGet]
        [Route("api/surveys/{surveyId}/attachment/{id}")]
        public IHttpActionResult GetAttachment(Guid surveyId, Guid id)
        {
            var attachment = UnitOfWork.AttachmentsRepository.Find(id);
            if (attachment.FormValue.FilledFormId != surveyId)
                return NotFound();

            var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == attachment.FormValue.FilledForm.ProjectId);
            if (assignment == null || !assignment.CanView)
                return Content(HttpStatusCode.Forbidden, "Access Denied");

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

        [Route("api/surveys")]
        public IHttpActionResult Post(FilledFormDTO survey)
        {
            var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == survey.ProjectId);
            if (assignment == null || !assignment.CanAdd)
                return Content(HttpStatusCode.Forbidden, "Access Denied");

            var filledForm = Mapper.Map<FilledForm>(survey);
            filledForm.FilledById = CurrentOrgUser.Id;
            try
            {
                UnitOfWork.FilledFormsRepository.InsertOrUpdate(filledForm);

                foreach (var val in filledForm.FormValues.Where(v => UnitOfWork.MetricsRepository.Find(v.MetricId.Value) is DateMetric))
                {
                    var dateMetric = val.Metric as DateMetric;
                    var _test = val.DateValue.Value.ToString();

                    if (val.DateValue.HasValue && !dateMetric.HasTimeValue)
                    {
                        var dateValue = val.DateValue.Value;
                        val.DateValue = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, 0, 0, 0).AddDays(1);
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
            var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == surveyDTO.ProjectId);
            if (assignment == null || !assignment.CanEdit)
                return Content(HttpStatusCode.Forbidden, "Access Denied");

            var survey = Mapper.Map<FilledForm>(surveyDTO);
            ModelState.Clear();
            Validate(survey);

            if (ModelState.IsValid)
            {
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
                            var dateValue = dbrecord.DateValue.Value;
                            dbrecord.DateValue = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, 0, 0, 0).AddDays(1);
                        }
                    }

                    if (dbrecord.Metric is AttachmentMetric)
                    {
                        //Delete removed attachments 
                        val.Attachments.Where(a => a.isDeleted).ToList().ForEach(a => UnitOfWork.AttachmentsRepository.Delete(a.Id));

                        var fileInfos = val.TextValue == string.Empty ? new List<FileInfo>() : val.TextValue.Split(',')
                                             .Select(i => HttpContext.Current.Server.MapPath("~/Uploads/" + i)).Select(path => new DirectoryInfo(path).GetFiles().FirstOrDefault());

                        var attachments = fileInfos.Select(fileInfo => UnitOfWork.AttachmentsRepository.CreateAttachment(fileInfo, dbrecord));
                        UnitOfWork.AttachmentsRepository.InsertOrUpdate(attachments);

                        dbrecord.TextValue = string.Empty;
                    }

                    FormValues.InsertOrUpdate(dbrecord);
                }

                var deletedFormValues = dbForm.FormValues.Where(dv => !survey.FormValues.Any(v => v.Id == dv.Id)).ToList();
                foreach (var deletedFormValue in deletedFormValues)
                    FormValues.Delete(deletedFormValue);

                UnitOfWork.Save();

                var tempAttachments = dbForm.FormValues.SelectMany(v => v.Attachments).Where(a => a.IsTemp).ToList();
                if (tempAttachments.Any())
                {
                    foreach (var attachment in tempAttachments)
                        UnitOfWork.AttachmentsRepository.StoreFile(attachment);
                    UnitOfWork.Save();
                }

                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE api/<controller>/5
        [Route("api/surveys/{id}")]
        public IHttpActionResult Delete(Guid id)
        {
            var survey = UnitOfWork.FilledFormsRepository.Find(id);
            if (survey == null)
                return NotFound();

            var assignment = this.CurrentOrgUser.Assignments.SingleOrDefault(a => a.ProjectId == survey.ProjectId);
            if (assignment == null || !assignment.CanDelete)
                return Content(HttpStatusCode.Forbidden, "Access Denied");

            UnitOfWork.FilledFormsRepository.Delete(survey);
            UnitOfWork.FilledFormsRepository.Save();

            return Ok();
        }
    }
}