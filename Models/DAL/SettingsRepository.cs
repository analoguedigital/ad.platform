using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class SettingsRepository : Repository<Settings>
    {
        public SettingsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        public Settings Current {
            get
            {
                return All.FirstOrDefault();
            }
        }
    }
}
