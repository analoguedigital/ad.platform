using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;

namespace LightMethods.Survey.Models.Services
{
    //public class DataTableProvider
    //{
    //    public FormTemplate FormTemplate { private set; get; }
    //    public IEnumerable<TableColumn> Columns { private set; get; }

    //    public DataTableProvider(FormTemplate template)
    //    {
    //        FormTemplate = template;
    //        Columns = GetAllColumns(FormTemplate);
    //    }

    //    public DataTableProvider(IEnumerable<TableColumn> columns)
    //    {
    //        Columns = columns;
    //        FormTemplate = columns.First().Metric.FormTemplate;
    //    }

    //    private IEnumerable<TableColumn> GetAllColumns(FormTemplate template)
    //    {
    //        foreach (var group in template.MetricGroups.OrderBy(g => g.Order))
    //        {
    //            foreach (var metric in group.Metrics.OrderBy(m => m.Order))
    //            {
    //                yield return new TableColumn()
    //                {
    //                    Metric = metric,
    //                    MetricId = metric.Id,
    //                    HeaderText = metric.ShortTitle,
    //                };
    //            }
    //        }
    //    }

    //    private DataTable GetTable()
    //    {
    //        var dataTable = new DataTable();

    //        var filledFormIdColumn = new DataColumn("FilledFormId") { DataType = typeof(Guid), Namespace = "System" };
    //        dataTable.Columns.Add(filledFormIdColumn);
    //        dataTable.Columns.Add(new DataColumn("SurveyDate") { DataType = typeof(DateTime), Namespace = "System" });
   
    //        foreach (var col in Columns)
    //        {
    //            if (col.Metric.MetricGroup.IsRepeater && !dataTable.Columns.Contains(col.Metric.MetricGroup.Id.ToString()))
    //            {
    //                dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.Title, Namespace = "System" });
    //                if (col.Metric.MetricGroup.Type == MetricGroupType.DataListRepeater)
    //                {
    //                    dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Name });

    //                    if (col.Metric.MetricGroup.DataList.Relationship1Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship1Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship1.Name });
    //                    if (col.Metric.MetricGroup.DataList.Relationship2Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship2Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship2.Name });
    //                    if (col.Metric.MetricGroup.DataList.Relationship3Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship3Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship3.Name });
    //                    if (col.Metric.MetricGroup.DataList.Relationship4Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship4Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship4.Name });
    //                    if (col.Metric.MetricGroup.DataList.Relationship5Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship5Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship5.Name });
    //                    if (col.Metric.MetricGroup.DataList.Relationship6Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship6Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship6.Name });
    //                    if (col.Metric.MetricGroup.DataList.Relationship7Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship7Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship7.Name });
    //                    if (col.Metric.MetricGroup.DataList.Relationship8Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship8Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship8.Name });
    //                    if (col.Metric.MetricGroup.DataList.Relationship9Id.HasValue) dataTable.Columns.Add(new DataColumn(col.Metric.MetricGroup.Id.ToString() + col.Metric.MetricGroup.DataListId.ToString() + col.Metric.MetricGroup.DataList.Relationship9Id.Value.ToString()) { DataType = typeof(string), Caption = col.Metric.MetricGroup.DataList.Relationship9.Name });
    //                }
    //            }

    //            dataTable.Columns.Add(new DataColumn()
    //            {
    //                DataType = typeof(string),
    //                ColumnName = col.MetricId.ToString(),
    //                Caption = col.HeaderText,
    //            });
    //        }

    //        return dataTable;
    //    }


    //    public DataTable GetDataTable(DateTime? sDate, DateTime? eDate)
    //    {
    //        var StartDate = sDate ?? new DateTime(1900, 1, 1);
    //        var EndDate = eDate ?? new DateTime(2100, 1, 1);

    //        using (var context = new SurveyContext())
    //        {

    //            var table = GetTable();
    //            foreach (var column in Columns)
    //            {
    //                var values = context.FormValues.Include(v => v.FilledForm).Include(v => v.Metric.MetricGroup)
    //                    .Where(v => v.MetricId == column.MetricId)
    //                    .Where(v => v.FilledForm.SurveyDate >= StartDate)
    //                    .Where(v => v.FilledForm.SurveyDate <= EndDate)
    //                    .AsNoTracking();

    //                foreach (FormValue value in values)
    //                {

    //                    var rowQuery = table.AsEnumerable().Where(r => r.Field<Nullable<Guid>>("FilledFormId").Value == value.FilledFormId);
    //                    //var rowCriteria = string.Format("FilledFormId='{0}'", value.FilledFormId.ToString());
    //                    var group = value.Metric.MetricGroup;
    //                    if (group.IsRepeater)
    //                    {
    //                        //   rowCriteria += string.Format(" and ( [{0}]='{1}' or [{0}] is null) ", group.Id.ToString(), value.RowNumber);
    //                        rowQuery = rowQuery.Where(r => r.Field<string>(group.Id.ToString()) == value.RowNumber.ToString());
    //                    }
    //                    //var row = table.Select(rowCriteria).FirstOrDefault() ?? table.NewRow();
    //                    var row = rowQuery.SingleOrDefault() ?? table.NewRow();
                        
    //                    row["FilledFormId"] = value.FilledFormId;
    //                    row["SurveyDate"] = value.FilledForm.SurveyDate;
    //                    if (group.IsRepeater)
    //                    {
    //                        row[group.Id.ToString()] = value.RowNumber.ToString();
    //                        if (group.Type == MetricGroupType.DataListRepeater)
    //                        {
    //                            var datalistItem = group.DataList.AllItems.SingleOrDefault(i => i.Id == value.RowDataListItemId);
    //                            if (datalistItem != null)
    //                            {
    //                                row[group.Id.ToString() + group.DataListId.ToString()] = datalistItem.Text;

    //                                if (group.DataList.Relationship1Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship1Id.Value.ToString()] = group.DataList.Relationship1.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr1Id)?.Text;
    //                                if (group.DataList.Relationship2Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship2Id.Value.ToString()] = group.DataList.Relationship2.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr2Id)?.Text;
    //                                if (group.DataList.Relationship3Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship3Id.Value.ToString()] = group.DataList.Relationship3.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr3Id)?.Text;
    //                                if (group.DataList.Relationship4Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship4Id.Value.ToString()] = group.DataList.Relationship4.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr4Id)?.Text;
    //                                if (group.DataList.Relationship5Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship5Id.Value.ToString()] = group.DataList.Relationship5.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr5Id)?.Text;
    //                                if (group.DataList.Relationship6Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship6Id.Value.ToString()] = group.DataList.Relationship6.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr6Id)?.Text;
    //                                if (group.DataList.Relationship7Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship7Id.Value.ToString()] = group.DataList.Relationship7.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr7Id)?.Text;
    //                                if (group.DataList.Relationship8Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship8Id.Value.ToString()] = group.DataList.Relationship8.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr8Id)?.Text;
    //                                if (group.DataList.Relationship9Id.HasValue) row[group.Id.ToString() + group.DataListId.ToString() + group.DataList.Relationship9Id.Value.ToString()] = group.DataList.Relationship9.DataList.AllItems.SingleOrDefault(i => i.Id == datalistItem?.Attr9Id)?.Text;
    //                            }
    //                        }
    //                    }

    //                    row[value.MetricId.ToString()] = value.ToString();
    //                    if (row.RowState == DataRowState.Detached) table.Rows.Add(row);
    //                }
    //            }
    //            var dataview = new DataView(table);
    //            dataview.Sort = "SurveyDate desc, FilledFormId";

    //            var result = dataview.ToTable();
    //            DeleteSystemColumns(result);

    //            return result;
    //        }
    //    }

    //    void DeleteSystemColumns(DataTable table)
    //    {
    //        var invisibleColumns = new List<DataColumn>();
    //        foreach (DataColumn col in table.Columns)
    //            if (col.Namespace == "System")
    //                invisibleColumns.Add(col);

    //        foreach (DataColumn col in invisibleColumns)
    //            table.Columns.Remove(col);
    //    }

    //}
}
