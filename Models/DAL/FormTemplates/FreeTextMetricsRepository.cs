using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class FreeTextMetricsRepository : Repository<FreeTextMetric>
    {
        public FreeTextMetricsRepository(UnitOfWork uow) : base(uow) { }
    }
}
