using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class SubscriptionPlansRepository : Repository<SubscriptionPlan>
    {
        public SubscriptionPlansRepository(UnitOfWork uow) : base(uow) { }
    }
}
