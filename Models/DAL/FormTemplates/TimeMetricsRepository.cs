using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class TimeMetricsRepository : Repository<TimeMetric>
    {
        public TimeMetricsRepository(UnitOfWork uow) : base(uow) { }
    }
}
