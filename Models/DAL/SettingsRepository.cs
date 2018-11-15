using LightMethods.Survey.Models.Entities;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class SettingsRepository : Repository<Settings>
    {
        public SettingsRepository(UnitOfWork uow) : base(uow) { }

        public Settings Current
        {
            get { return All.FirstOrDefault(); }
        }
    }
}
