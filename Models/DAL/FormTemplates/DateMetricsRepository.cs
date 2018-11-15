using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class DateMetricsRepository : Repository<DateMetric>
    {
        public DateMetricsRepository(UnitOfWork uow)
            : base(uow) { }
    }
}
