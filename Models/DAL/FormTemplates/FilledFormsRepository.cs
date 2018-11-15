using AutoMapper;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.FilterValues;
using LightMethods.Survey.Models.MetricFilters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class FilledFormsRepository : Repository<FilledForm>
    {
        public FilledFormsRepository(UnitOfWork uow) : base(uow) { }

        public FilledForm GetFullFilledFormAndTemplate(Guid id)
        {
            return FindIncluding(id, f => f.FormValues, f => f.FormTemplate.MetricGroups.Select(g => g.Metrics));
        }

        //private static object _lock;
        //public int GetSerialNumber(Guid organisationId, Guid formTemplateId)
        //{
        //    lock (_lock)
        //    {
        //        return All
        //            .Where(f => f.FormTemplateId == formTemplateId && f.Project.OrganisationId == organisationId)
        //            .DefaultIfEmpty()
        //            .Max(f => f == null ? 0 : f.Serial) + 1;
        //    }
        //}

        public FilledForm InsertOrUpdate(FilledForm filledForm, Project project)
        {
            FilledForm result = null;
            using (var uow = new UnitOfWork(Context))
            {
                if (filledForm.Id == Guid.Empty)
                {
                    filledForm.ProjectId = project.Id;
                    uow.FormValuesRepository.InsertOrUpdate(filledForm.FormValues);
                    filledForm.FormValues.ToList().ForEach(a => a.Id = Guid.NewGuid());
                    InsertOrUpdate(filledForm);
                    result = filledForm;
                }
                else
                {
                    var dbEntity = Find(filledForm.Id);
                    UpdateCore(uow, dbEntity, filledForm, project);
                    result = dbEntity;
                }
            }

            return result;
        }

        protected FilledForm UpdateCore(UnitOfWork uow, FilledForm dest, FilledForm src, Project project)
        {
            // Context.Entry(dest).CurrentValues.SetValues(src);

            // dest.ProjectId = project.Id;
            //  InsertOrUpdate(dest);

            //   if (src.FormValues != null)
            //      src.FormValues.Where(v=>v.FilledFormId == Guid.Empty).ToList().ForEach(a => a.FilledForm = dest);
            uow.FormValuesRepository.Update(dest.FormValues, src.FormValues);

            return dest;
        }

        public IEnumerable<FilledForm> Search(SearchDTO model)
        {
            var templates = this.CurrentUOW.FormTemplatesRepository
                .AllAsNoTracking
                .Where(t => model.FormTemplateIds.Contains(t.Id))
                .ToList();

            IQueryable<FilledForm> query = Enumerable.Empty<FilledForm>().AsQueryable();
            List<FilledForm> foundSurveys = new List<FilledForm>();

            foreach (var template in templates)
            {
                var safeSurveys = this.CurrentUOW.FilledFormsRepository
                    .AllAsNoTracking
                    .Where(s => s.ProjectId == model.ProjectId && s.FormTemplateId == template.Id);

                var surveys = safeSurveys;

                var surveyCount = this.CurrentUOW.FilledFormsRepository
                    .AllAsNoTracking
                    .Where(s => s.ProjectId == model.ProjectId && s.FormTemplateId == template.Id)
                    .Count();

                // apply generic date range
                if (model.StartDate.HasValue || model.EndDate.HasValue)
                    surveys = this.ApplySurveysDateRange(surveys, model.StartDate, model.EndDate);

                // apply search term
                if (!string.IsNullOrEmpty(model.Term))
                {
                    var punctuation = model.Term.Where(Char.IsPunctuation).Distinct().ToArray();
                    var words = model.Term.Split().Select(x => x.Trim(punctuation));

                    var descMetrics = template.GetDescriptionMetrics();
                    if (descMetrics.Any())
                    {
                        List<Guid?> metricIds = new List<Guid?>();
                        foreach (var metric in descMetrics)
                            metricIds.Add(metric.Id);

                        surveys = surveys.Where(s => s.FormValues.Any(fv => metricIds.Contains(fv.MetricId) && words.Any(w => fv.TextValue.Contains(w))));
                    }
                    else
                        surveys = surveys.Where(s => s.FormValues.Any(fv => words.Any(w => fv.TextValue.Contains(w))));
                }

                // apply metric filters
                foreach (var filter in model.FilterValues)
                {
                    var filterValue = Mapper.Map<FilterValue>(filter);
                    var metric = this.FindMetricByShortTitle(filter.ShortTitle, template);
                    if (metric != null)
                        surveys = surveys.Where(metric.GetFilterExpression(filterValue));
                }

                var finalResult = new List<FilledForm>();
                var result = surveys.OrderByDescending(s => s.SurveyDate).ToList();

                finalResult.AddRange(result);

                // filter serial numbers. but 'OR' the result.
                if (!String.IsNullOrEmpty(model.Term))
                {
                    var punctuation = model.Term.Where(Char.IsPunctuation).Distinct().ToArray();
                    var words = model.Term.Split().Select(x => x.Trim(punctuation));

                    var foundSerials = safeSurveys.Where(s => words.Any(w => s.Serial.ToString() == w));
                    finalResult.AddRange(foundSerials.OrderByDescending(s => s.SurveyDate).ToList());
                }

                foundSurveys.AddRange(finalResult.Distinct());
            }

            return foundSurveys;
        }

        public override void Delete(FilledForm entity)
        {
            entity.FormValues.ToList().ForEach(v => CurrentUOW.FormValuesRepository.Delete(v));
            base.Delete(entity);
        }

        #region Helpers

        private IQueryable<FilledForm> ApplySurveysDateRange(IQueryable<FilledForm> surveys, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && !endDate.HasValue)
                surveys = surveys.Where(s => DbFunctions.TruncateTime(s.SurveyDate) >= DbFunctions.TruncateTime(startDate.Value));
            else if (!startDate.HasValue && endDate.HasValue)
                surveys = surveys.Where(s => DbFunctions.TruncateTime(s.SurveyDate) <= DbFunctions.TruncateTime(endDate.Value));
            else if (startDate.HasValue && endDate.HasValue)
                surveys = surveys.Where(s => DbFunctions.TruncateTime(s.SurveyDate) >= DbFunctions.TruncateTime(startDate.Value) && DbFunctions.TruncateTime(s.SurveyDate) <= DbFunctions.TruncateTime(endDate.Value));

            return surveys;
        }

        private Metric FindMetricByShortTitle(string shortTitle, FormTemplate template)
        {
            Metric result = null;

            foreach (var group in template.MetricGroups)
                foreach (var metric in group.Metrics.Where(m => !m.IsArchived()))
                    if (metric.ShortTitle == shortTitle) result = metric;

            return result;
        }

        #endregion Helpers
    }
}
