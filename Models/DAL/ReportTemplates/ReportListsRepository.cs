using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class ReportListsRepository : Repository<ReportList>
    {
        public ReportListsRepository(UnitOfWork uow) : base(uow) { }
    }
}
