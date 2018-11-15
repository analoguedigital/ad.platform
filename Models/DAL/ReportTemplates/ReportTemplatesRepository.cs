using LightMethods.Survey.Models.Entities;
using System;

namespace LightMethods.Survey.Models.DAL
{
    public class ReportTemplatesRepository : Repository<ReportTemplate>
    {
        public ReportTemplatesRepository(UnitOfWork uow) : base(uow) { }

        public object GetFullTemplate(Guid id)
        {
            return FindIncluding(id, t => t.Category, t => t.Items);
        }
    }
}
