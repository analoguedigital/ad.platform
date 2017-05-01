using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml;
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
        public IHttpActionResult Get(Guid projectId, string filter = "")
        {

            //TODO: refactore to api/projects/{projectId}/surveys
            MatchCollection filters = null;

            if (filter.HasValue())
            {
                string strRegex = @"(?<Filter>" +
                "\n" + @"     (?<Resource>.+?)\s+" +
                "\n" + @"     (?<Operator>eq|ne|gt|ge|lt|le|add|sub|mul|div|mod)\s+" +
                "\n" + @"     '?(?<Value>.+?)'?" +
                "\n" + @")" +
                "\n" + @"(?:" +
                "\n" + @"    \s*$" +
                "\n" + @"   |\s+(?:or|and|not)\s+" +
                "\n" + @")" +
                "\n";
                Regex myRegex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                if (!myRegex.IsMatch(filter))
                    BadRequest();

                filters = myRegex.Matches(filter);
            }

            var surveys = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                .Where(s => s.ProjectId == projectId)
                .ToList();
            //.Select(f => Mapper.Map<FilledFormDTO>(f));

            if (filters != null)
            {
                foreach (Match criteria in filters)
                {
                    var metricName = criteria.Groups["Resource"].Value;
                    var op = criteria.Groups["Operator"].Value;
                    var value = criteria.Groups["Value"].Value;


                    surveys = surveys
                        .Where(s => (s.FormValues.Where(v => v.Metric.ShortTitle == metricName).SingleOrDefault()) == value)
                        .ToList();
                }
            }

            return Ok(surveys.Select(f => Mapper.Map<FilledFormDTO>(f)));
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

            return Ok(survey);
        }

        [HttpGet]
        [Route("api/surveys/{surveyId}/attachment/{id}")]
        public IHttpActionResult GetAttachment(Guid surveyId, Guid id)
        {
            var attachment = UnitOfWork.AttachmentsRepository.Find(id);
            if (attachment.FormValue.FilledFormId != surveyId)
                return NotFound();

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
            var filledForm = Mapper.Map<FilledForm>(survey);
            filledForm.FilledById = CurrentOrgUser.Id;
            try
            {
                UnitOfWork.FilledFormsRepository.InsertOrUpdate(filledForm);

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
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("api/surveys/{id}")]
        public IHttpActionResult Put(Guid id, FilledFormDTO surveyDTO)
        {
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
                dbForm.FormValues.SelectMany(v => v.Attachments).Where(a => a.IsTemp).ToList()
                    .ForEach(attachment => UnitOfWork.AttachmentsRepository.StoreFile(attachment));
                UnitOfWork.Save();
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
                NotFound();

            UnitOfWork.FilledFormsRepository.Delete(survey);
            UnitOfWork.FilledFormsRepository.Save();

            return Ok();
        }
    }
}