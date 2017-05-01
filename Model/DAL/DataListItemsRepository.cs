using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DAL
{
    public class DataListItemsRepository : Repository<DataListItem>
    {
        public DataListItemsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        public override void InsertOrUpdate(DataListItem entity)
        {
            base.InsertOrUpdate(entity);

            //var repo = new Repository<DataListItemAttr>(CurrentUOW);
           // repo.InsertOrUpdate(entity.Attributes);
        }
    }
}
 