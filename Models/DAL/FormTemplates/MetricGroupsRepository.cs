using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class MetricGroupsRepository : Repository<MetricGroup>
    {
        public MetricGroupsRepository(UnitOfWork uow) : base(uow) { }

        public override void Delete(MetricGroup entity)
        {
            entity.PrepareForDelete();
            base.Delete(entity);
        }
    }
}
