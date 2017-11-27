using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.DAL;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using LightMethods.Survey.Models.Services;

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
