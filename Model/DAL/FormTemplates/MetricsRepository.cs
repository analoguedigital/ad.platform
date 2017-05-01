using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class MetricsRepository : Repository<Metric>
    {
        public MetricsRepository(UnitOfWork uow)
            : base(uow)
        {
            
        }


    }
}
