using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.DAL
{
    public class NumericMetricsRepository : Repository<NumericMetric>
    {

        public NumericMetricsRepository(UnitOfWork uow)
            : base(uow)
        {

        }
    }
}
