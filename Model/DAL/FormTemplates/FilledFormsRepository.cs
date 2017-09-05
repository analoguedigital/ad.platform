﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.MetricFilters;
using AutoMapper;
using LightMethods.Survey.Models.FilterValues;
using System.Data.Entity;

namespace LightMethods.Survey.Models.DAL
{
    public class FilledFormsRepository : Repository<FilledForm>
    {
        public FilledFormsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        public FilledForm GetFullFilledFormAndTemplate(Guid id)
        {
            return FindIncluding(id
                , f => f.FormValues
                , f => f.FormTemplate.MetricGroups.Select(g => g.Metrics));

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

        public IEnumerable<FilledForm> Search(Search model)
        {
            var template = this.Context.FormTemplates.Find(model.FormTemplateId);

            var surveys = this.Context.FilledForms
                .Where(s => s.ProjectId == model.ProjectId && s.FormTemplateId == model.FormTemplateId);

            // apply generic date range
            if (model.StartDate.HasValue || model.EndDate.HasValue)
                surveys = this.ApplySurveysDateRange(surveys, model.StartDate, model.EndDate);

            // apply metric filters
            foreach (var filter in model.FilterValues)
            {
                var filterValue = Mapper.Map<FilterValue>(filter);

                var metric = this.FindMetricByShortTitle(filter.ShortTitle, template);
                if (metric != null)
                    surveys = surveys.Where(metric.GetFilterExpression(filterValue));
            }

            var result = surveys.ToList();

            // apply generic search term
            if (!string.IsNullOrEmpty(model.Term))
                result = result.Where(s => s.Description.Contains(model.Term, caseSensitive: false)).ToList();

            return result;
        }

        public IEnumerable<FilledForm> SummarySearch(SummarySearchDTO model)
        {
            var templates = this.Context.FormTemplates.Where(t => model.FormTemplateIds.Contains(t.Id)).ToList();

            var surveys = this.Context.FilledForms
                .Where(s => s.ProjectId == model.ProjectId && model.FormTemplateIds.Contains(s.FormTemplateId));

            // apply generic date range
            if (model.StartDate.HasValue || model.EndDate.HasValue)
                surveys = this.ApplySurveysDateRange(surveys, model.StartDate, model.EndDate);

            foreach (var template in templates)
            {
                // apply metric filters
                foreach (var filter in model.FilterValues)
                {
                    var filterValue = Mapper.Map<FilterValue>(filter);

                    var metric = this.FindMetricByShortTitle(filter.ShortTitle, template);
                    if (metric != null)
                        surveys = surveys.Where(metric.GetFilterExpression(filterValue));
                }
            }

            var result = surveys.ToList();

            // apply generic search term
            if (!string.IsNullOrEmpty(model.Term))
                result = result.Where(s => s.Description.Contains(model.Term, caseSensitive: false)).ToList();

            return result;
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
