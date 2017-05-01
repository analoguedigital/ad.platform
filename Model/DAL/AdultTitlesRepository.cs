using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class AdultTitlesRepository : Repository<AdultTitle>
    {
        public AdultTitlesRepository(UnitOfWork uow)
            : base(uow)
        {

        }


        public override IQueryable<AdultTitle> All
        {
            get
            {
                return base.All.OrderBy(t =>t.Order);
            }
        }


    }
}
