using LightMethods.Survey.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class AddressTypesRepository : Repository<AddressType>
    {
        public AddressTypesRepository(UnitOfWork uow) : base(uow) { }

        public override IQueryable<AddressType> All
        {
            get
            {
                return base.All.OrderBy(t => t.Order);
            }
        }
    }
}
