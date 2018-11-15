using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class PaymentsRepository : Repository<PaymentRecord>
    {
        public PaymentsRepository(UnitOfWork uow) : base(uow) { }
    }
}
