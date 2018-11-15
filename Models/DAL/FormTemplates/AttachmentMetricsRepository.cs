using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class AttachmentMetricsRepository : Repository<AttachmentMetric>
    {
        public AttachmentMetricsRepository(UnitOfWork uow) : base(uow) { }
    }
}
