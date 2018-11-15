using AppHelper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LightMethods.Survey.Models.Services
{
    public class FormDataProvider : IDisposable
    {
        private SurveyContext DbContext { set; get; }
        public FormTemplate FormTemplate { private set; get; }
        public Project Project { private set; get; }
        public IDictionary<Guid, Metric> Metrics { private set; get; }
        private bool IgnoreRepeaters { set; get; }

        public FormDataProvider(Guid templateId, Guid projectId, bool ignoreRepeaters = false)
        {
            DbContext = new SurveyContext(false);
            Project = DbContext.Projects.Find(projectId);
            IgnoreRepeaters = ignoreRepeaters;
            FormTemplate = DbContext.FormTemplates.Find(templateId);

            Metrics = FormTemplate.MetricGroups
                .Where(g => !ignoreRepeaters || (g.DataListId == null && g.NumberOfRows == null))
                .SelectMany(g => g.Metrics)
                .Where(m => m.DateArchived == null)
                .ToList()
                .Where(m => !(m is AttachmentMetric))
                .ToDictionary<Metric, Guid, Metric>(m => m.Id, m => m);
        }

        private DataTable GetTable()
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn("Id") { DataType = typeof(string), Namespace = "System" });
            dataTable.Columns.Add(new DataColumn("Serial No") { DataType = typeof(int), Namespace = "System" });
            dataTable.Columns.Add(new DataColumn("Date") { DataType = typeof(string), Namespace = "System" });
            dataTable.Columns.Add(new DataColumn("User") { DataType = typeof(string), Namespace = "System" });

            foreach (var group in FormTemplate.MetricGroups.OrderBy(g => g.PageOrder))
            {
                if (group.IsRepeater)
                {

                    if (group.Type == MetricGroupType.DataListRepeater)
                    {
                        foreach(var relationship in group.DataList.Relationships)
                            dataTable.Columns.Add(new DataColumn(group.Id.ToString() + relationship.Id.ToString()) { DataType = typeof(string), Caption = relationship.Name });
                    }
                }

                foreach (var metric in group.Metrics.OrderBy(m => m.Order))
                {
                    dataTable.Columns.Add(new DataColumn()
                    {
                        DataType = typeof(string),
                        ColumnName = metric.Id.ToString(),
                        Caption = metric.ShortTitle,
                    });
                }
            }

            return dataTable;
        }

        public DataTable GetDataTable(DateTime? sDate, DateTime? eDate)
        {
            var dataTable = GetTable();

            var data = GetData(sDate, eDate);

            foreach (var surveyData in data)
            {
                var rows = new List<DataRow>();
                rows.Add(dataTable.NewRow());
                rows[0]["Id"] = surveyData.FilledFormId.ToString();
                rows[0]["Serial No"] = surveyData.SerialNo;
                rows[0]["Date"] = surveyData.SurveyDate.ToString("dd/MM/yyyy",this.Project.Organisation.DefaultCalendar.Name);
                rows[0]["User"] = surveyData.User;

                foreach (var groupdata in surveyData.GroupData)
                {
                    var groupRowIndex = 0;
                    foreach (var row in groupdata.DataRows)
                    {
                        if (groupRowIndex >= rows.Count)
                            rows.Add(dataTable.NewRow());

                        foreach (var value in row.Values)
                        {
                            rows[groupRowIndex][value.Key] = value.Value;
                            if (groupdata.Group.Type == MetricGroupType.DataListRepeater)
                            {
                                foreach(var relationship in groupdata.Group.DataList.Relationships)
                                    rows[groupRowIndex][groupdata.Group.Id.ToString() + relationship.Id.ToString()] = row.DataListItem.GetAttrValue( relationship.Id)?.Text;
                            }
                        }

                        groupRowIndex++;
                    }
                }
                rows.ForEach(r => dataTable.Rows.Add(r));
            }

            return dataTable;
        }

        public IEnumerable<FormData> GetData(DateTime? sDate, DateTime? eDate)
        {
            var StartDate = sDate ?? new DateTime(1900, 1, 1);
            var EndDate = eDate ?? new DateTime(2100, 1, 1);

            var filledForms = DbContext.FilledForms
               .Where(f => f.FormTemplateId == FormTemplate.Id)
               .Where(f => f.ProjectId == Project.Id)
               .Where(v => v.SurveyDate >= StartDate)
               .Where(v => v.SurveyDate <= EndDate)
               .OrderBy(f => f.Serial);


            foreach (var survey in filledForms)
            {
                var values = DbContext.FormValues
                    .Where(v => v.FilledFormId == survey.Id && Metrics.Keys.Contains(v.MetricId.Value))
                    .Where(v => v.BoolValue.HasValue || v.DateValue.HasValue || v.GuidValue.HasValue || v.NumericValue.HasValue || v.TextValue != null)
                    .ToList();

                var mainRow = new FormData
                {
                    FilledFormId = survey.Id,
                    SurveyDate = survey.SurveyDate,
                    SerialNo = survey.Serial,
                    User = survey.FilledBy.ToString()
                };

                foreach (var group in FormTemplate.MetricGroups)
                {
                    mainRow.GroupData.Add(GetGroupData(group, values));
                }

                yield return mainRow;
            }

        }

        private GroupData GetGroupData(MetricGroup group, List<FormValue> values)
        {
            var groupData = new GroupData()
            {
                Group = group,
            };

            var metrics = group.Metrics
                .Where(m => m.DateArchived == null)// && Columns.Any(c => c.MetricId == m.Id))
                .AsEnumerable()
                .ToDictionary<Metric, Guid, Metric>(m => m.Id, m => m); ;

            if (group.Type == MetricGroupType.Single)
            {
                groupData.DataRows.Add(new GroupDataRow()
                {
                    Group = group,
                    Values = values
                        .Where(v => metrics.Keys.Contains(v.MetricId.Value))
                        .GroupBy(v => v.MetricId)
                        .Select(g => new KeyValuePair<string, string>(g.Key.ToString(), GetStringValue(g.ToList(), metrics[g.Key.Value])))
                        .ToDictionary(kv => kv.Key, kv => kv.Value)
                });
            }
            else if (group.Type == MetricGroupType.IterativeRepeater)
            {
                groupData.DataRows.AddRange(

                values.Where(v => metrics.Keys.Contains(v.MetricId.Value))
                .GroupBy(v => v.RowNumber)
                .OrderBy(g => g.Key)
                .Select(r => new GroupDataRow()
                {
                    Group = group,
                    RowNumber = r.Key.Value,
                    Values = r.ToList()
                        .GroupBy(v => v.MetricId)
                        .Select(g => new KeyValuePair<string, string>(g.Key.ToString(), GetStringValue(g.ToList(), metrics[g.Key.Value])))
                        .ToDictionary(kv => kv.Key, kv => kv.Value)
                }));
            }
            else if (group.Type == MetricGroupType.DataListRepeater)
            {
                groupData.DataRows.AddRange(

                values.Where(v => metrics.Keys.Contains(v.MetricId.Value))
                .GroupBy(v => v.RowDataListItem)
                .OrderBy(g => g.Key.Order)
                .Select(r => new GroupDataRow()
                {
                    Group = group,
                    DataListItem = r.Key,
                    Values = r.ToList()
                        .GroupBy(v => v.MetricId)
                        .Select(g => new KeyValuePair<string, string>(g.Key.ToString(), GetStringValue(g.ToList(), metrics[g.Key.Value])))
                        .ToDictionary(kv => kv.Key, kv => kv.Value)
                }));
            }

            return groupData;
        }

        private string GetStringValue(IEnumerable<FormValue> values, Metric metric)
        {
            if (metric is MultipleChoiceMetric)
            {
                var mpcMetric = metric as MultipleChoiceMetric;
                if (mpcMetric.ViewType == MultipleChoiceViewType.CheckBoxList)
                {
                    var result = string.Empty;
                    foreach (var item in mpcMetric.DataList.Items)
                    {
                        var value = values.Where(v => v.GuidValue == item.Id).SingleOrDefault();

                        if (value?.BoolValue ?? false)
                        {
                            result += mpcMetric.DataList.Items.Where(c => c.Id == value.GuidValue).Single().Text + ", ";
                        }
                    }
                    return result;
                }
            }

            return values.First().ToString();
        }

        public void Dispose()
        {
            if (DbContext != null)
                DbContext.Dispose();
        }
    }

    public class FormData
    {
        public Guid FilledFormId { set; get; }
        public DateTime SurveyDate { set; get; }
        public int SerialNo { set; get; }
        public string User { set; get; }
        public List<GroupData> GroupData { set; get; } = new List<GroupData>();
    }

    public class GroupData
    {
        public Guid FilledFormId { set; get; }
        public MetricGroup Group { set; get; }
        public List<GroupDataRow> DataRows { set; get; } = new List<GroupDataRow>();

    }

    public class GroupDataRow
    {
        public Guid FilledFormId { set; get; }
        public MetricGroup Group { set; get; }
        public DataListItem DataListItem { set; get; }
        public int RowNumber { set; get; }
        public Dictionary<string, string> Values { set; get; }
    }
}

