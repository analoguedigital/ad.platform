using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class AdultAddressesRepository : Repository<AdultAddress>
    {
        public AdultAddressesRepository(UnitOfWork uow) : base(uow) { }
    }
}
