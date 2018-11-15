using LightMethods.Survey.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class LanguagesRepository : Repository<Language>
    {
        public LanguagesRepository(UnitOfWork uow) : base(uow) { }

        public override IQueryable<Language> All
        {
            get
            {
                return base.All.OrderBy(t => t.Order);
            }
        }

        private static Language EnsureHasValue(ref Language prop)
        {
            if (prop != null)
                return prop;

            using (var uow = new UnitOfWork(new SurveyContext()))
            {
                var languages = uow.LanguagesRepository.All.ToList();
                _English = languages.Where(i => i.SystemName == "English").Single();
                _Persian = languages.Where(i => i.SystemName == "Persian").Single();
            }

            return prop;
        }

        private static Language _English;
        public static Language English { get { return EnsureHasValue(ref _English); } }

        private static Language _Persian;
        public static Language Persian { get { return EnsureHasValue(ref _Persian); } }
    }
}
