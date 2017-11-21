﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class FormTemplatesRepository : Repository<FormTemplate>
    {
        public FormTemplatesRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        //Getting a form template with preloaded metrics
        public FormTemplate GetNotTrackedFullTemplate(Guid id)
        {
            return Context.FormTemplates
                .Where(t => t.Id == id)
                .Include(t => t.MetricGroups.Select(g => g.Metrics))
                .AsNoTracking()
                .First();
        }

        //Getting a form template with preloaded metrics
        public FormTemplate GetFullTemplate(Guid id)
        {
            return Context.FormTemplates
                .Where(t => t.Id == id)
                .Include(t => t.MetricGroups.Select(g => g.Metrics))
                .First();
        }

        public FormTemplate Clone(FormTemplate template, OrgUser user, string newTitle, string newColour, Guid? newProjectId)
        {
            var clone = template.Clone();
            clone.Title = newTitle;
            clone.Colour = newColour;
            clone.ProjectId = newProjectId;
            clone.CreatedById = user.Id;
            clone.IsPublished = true;

            using (var tran = Context.Database.BeginTransaction())
            {
                InsertOrUpdate(clone);
                Save();

                if (template.CalendarDateMetricId != null)
                {
                    clone.CalendarDateMetricId = clone.MetricGroups.SelectMany(g => g.Metrics)
                        .Single(m => m.ShortTitle == template.CalendarDateMetric.ShortTitle).Id;
                }

                if (template.TimelineBarMetricId != null)
                {
                    clone.TimelineBarMetricId = clone.MetricGroups.SelectMany(g => g.Metrics)
                        .Single(m => m.ShortTitle == template.TimelineBarMetric.ShortTitle).Id;
                }

                InsertOrUpdate(clone);
                Save();
                tran.Commit();
            }

            return clone;
        }
    }
}