using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class RateMetricsRepository : Repository<RateMetric>
    {
        public RateMetricsRepository(UnitOfWork uow) : base(uow) { }
    }
}
