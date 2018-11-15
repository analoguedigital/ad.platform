using LightMethods.Survey.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class CalendarsRepository : Repository<Calendar>
    {
        public CalendarsRepository(UnitOfWork uow) : base(uow) { }

        public override IQueryable<Calendar> All
        {
            get
            {
                return base.All.OrderBy(t => t.Order);
            }
        }

        private static Calendar EnsureHasValue(ref Calendar prop)
        {
            if (prop != null)
                return prop;

            using (var uow = new UnitOfWork(new SurveyContext()))
            {
                var calendars = uow.CalendarsRepository.All.ToList();
                _Gregorian = calendars.Where(i => i.SystemName == "Gregorian").Single();
                _Persian = calendars.Where(i => i.SystemName == "Persian").Single();
            }

            return prop;
        }

        private static Calendar _Gregorian;
        public static Calendar Gregorian { get { return EnsureHasValue(ref _Gregorian); } }

        private static Calendar _Persian;
        public static Calendar Persian { get { return EnsureHasValue(ref _Persian); } }
    }
}
