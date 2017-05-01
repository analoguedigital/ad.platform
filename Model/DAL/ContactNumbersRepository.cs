using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{

    public class AdultContactNumbersRepository : Repository<AdultContactNumber>
    {
        public AdultContactNumbersRepository(UnitOfWork uow)
            : base(uow)
        {

        }

    }


    public class ExternalOrgContactNumbersRepository : Repository<ExternalOrgContactNumber>
    {
        public ExternalOrgContactNumbersRepository(UnitOfWork uow)
            : base(uow)
        {

        }

    }
}
