using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class NumericMetricsRepository : Repository<NumericMetric>
    {
        public NumericMetricsRepository(UnitOfWork uow) : base(uow) { }
    }
}
