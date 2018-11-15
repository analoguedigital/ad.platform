using System;
using System.Collections.Generic;
using System.Data;

namespace LightMethods.Survey.Models.Entities
{
    public class ReportTable : ReportItem
    {
        public virtual ICollection<TableColumn> Columns { get; set; }

        public DataTable GetDataTable(DateTime? sDate, DateTime? eDate)
        {
            //var tableProvider = new DataTableProvider(Columns);
            //return tableProvider.GetDataTable(sDate, eDate);
            return new DataTable();
        }

    }
}
