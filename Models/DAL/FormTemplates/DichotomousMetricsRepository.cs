using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class DichotomousMetricsRepository : Repository<DichotomousMetric>
    {
        public DichotomousMetricsRepository(UnitOfWork uow) : base(uow) { }
    }
}
