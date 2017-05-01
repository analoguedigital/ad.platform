using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class MetricGroupsRepository : Repository<MetricGroup>
    {
        public MetricGroupsRepository(UnitOfWork uow)
            : base(uow)
        {
            
        }

        public override void Delete(MetricGroup entity)
        {
            entity.prepareForDelete();

            base.Delete(entity);
       }
    }
}
