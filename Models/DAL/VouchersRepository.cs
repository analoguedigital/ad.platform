using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class VouchersRepository : Repository<Voucher>
    {
        public VouchersRepository(UnitOfWork uow) : base(uow) { }
    }
}
