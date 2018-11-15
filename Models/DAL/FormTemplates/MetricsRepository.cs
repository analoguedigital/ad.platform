using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class MetricsRepository : Repository<Metric>
    {
        public MetricsRepository(UnitOfWork uow) : base(uow) { }
    }
}
