using LightMethods.Survey.Models.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public class ChartSerie:Entity
    {
        public virtual ReportChart Chart { get; set; }

        public Guid ChartId { get; set; }

        public virtual ChartSerieType Type { set; get; }

        public Guid TypeId { set; get; }

        public virtual Metric Metric { set; get; }

        public Guid MetricId { set; get; }

        public Guid? MetricRowId { set; get; }

        public Guid? MetricColumnId { set; get; }

        public virtual Metric Label { set; get; }

        public Guid? LabelId { set; get; }

        public Guid? LabelRowId { set; get; }

        public Guid? LabelColumnId { set; get; }

        public IEnumerable<ChartPoint> GetPoints(DateTime? sDate, DateTime? eDate)
        {
            var StartDate = sDate ?? DateTime.MinValue;
            var EndDate = eDate ?? DateTime.MaxValue;

            IEnumerable<ChartPoint> points;
            using (var uow = new UnitOfWork(new LightMethods.Survey.Models.DAL.SurveyContext()))
            {
                var pointsQuery = uow.Context.FormValues.AsNoTracking()
                    .Where(v => v.MetricId == MetricId );

                if (MetricRowId != null)
                    pointsQuery = pointsQuery.Where(v => v.RowDataListItemId == MetricRowId);


                 points=pointsQuery.Where(value => value.FilledForm.SurveyDate <= EndDate && value.FilledForm.SurveyDate >= StartDate)
                    .Select(value => new ChartPoint() { y = value.NumericValue ?? 0, x = value.FilledForm.SurveyDate, name = "" })
                    .OrderBy(p => p.x);
                          
            }

            return points.ToList();
        }
    }

    public class ChartPoint
    {
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime x { set; get; }

        public double y { get; set; }

        public string name { get; set; }
    }
}
