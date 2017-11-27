using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class ReportListDataTypesRepository : Repository<ReportListDataType>
    {
        public ReportListDataTypesRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        public override IQueryable<ReportListDataType> All
        {
            get
            {
                return base.All.OrderBy(t => t.Order);
            }
        }
    }
}
