using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DAL
{
    public class DataListsRepository : Repository<DataList>
    {
        public DataListsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        public override void Delete(DataList entity)
        {
            var items = entity.AllItems.ToArray();
            base.Delete(entity);
            if (Context.Entry(entity).State == EntityState.Deleted)
                foreach (var item in items)
                    Context.Entry(item).State = EntityState.Deleted;
        }
    }
}
