using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class ReportTablesRepository : Repository<ReportTable>
    {
        public ReportTablesRepository(UnitOfWork uow) : base(uow) { }
    }
}
