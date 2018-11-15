using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class FeedbacksRepository : Repository<Feedback>
    {
        public FeedbacksRepository(UnitOfWork uow) : base(uow) { }
    }
}
