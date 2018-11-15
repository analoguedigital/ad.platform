using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class ReportChartsRepository : Repository<ReportChart>
    {
        public ReportChartsRepository(UnitOfWork uow) : base(uow) { }
    }
}
