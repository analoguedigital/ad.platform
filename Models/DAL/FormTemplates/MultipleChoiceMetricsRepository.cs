using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class MultipleChoiceMetricsRepository : Repository<MultipleChoiceMetric>
    {
        public MultipleChoiceMetricsRepository(UnitOfWork uow) : base(uow) { }

        public override void Delete(MultipleChoiceMetric entity)
        {
            base.Delete(entity);
            if (entity.DataList != null && entity.DataList.IsAdHoc)
            {
                using (var uow = new UnitOfWork(Context))
                    uow.DataListsRepository.Delete(entity.DataList);
            }
        }
    }
}
