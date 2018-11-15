using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class SubscriptionsRepository : Repository<Subscription>
    {
        public SubscriptionsRepository(UnitOfWork uow) : base(uow) { }
    }
}
