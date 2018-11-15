using LightMethods.Survey.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class ContactNumberTypesRepository : Repository<ContactNumberType>
    {
        public ContactNumberTypesRepository(UnitOfWork uow) : base(uow) { }

        public override IQueryable<ContactNumberType> All
        {
            get
            {
                return base.All.OrderBy(t => t.Order);
            }
        }
    }
}
