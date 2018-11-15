using LightMethods.Survey.Models.Entities;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class RolesRepository : Repository<Role>
    {
        public RolesRepository(UnitOfWork uow) : base(uow) { }

        private static Role EnsureHasValue(ref Role prop)
        {
            if (prop != null)
                return prop;

            using (var uow = new UnitOfWork(new SurveyContext()))
            {
                var roles = uow.RolesRepository.AllAsNoTracking.ToList();
                _SystemAdministrator = roles.Where(i => i.Name == Role.SYSTEM_ADMINSTRATOR).Single();
                _PlatformAdministrator = roles.Where(i => i.Name == Role.PLATFORM_ADMINISTRATOR).Single();
                _OrgUser = roles.Where(i => i.Name == Role.ORG_USER).Single();
                _OrgAdministrator = roles.Where(i => i.Name == Role.ORG_ADMINSTRATOR).Single();
                _OrgTemplateManagement = roles.Where(i => i.Name == Role.ORG_TEMPLATES_MANAGMENT).Single();
                _OrgUserManagement = roles.Where(i => i.Name == Role.ORG_USER_MANAGMENT).Single();
                _OrgProjectManagement = roles.Where(i => i.Name == Role.ORG_PROJECT_MANAGMENT).Single();
            }

            return prop;
        }

        private static Role _SystemAdministrator;
        public static Role SystemAdministrator { get { return EnsureHasValue(ref _SystemAdministrator); } }

        private static Role _PlatformAdministrator;
        public static Role PlatformAdministrator { get { return EnsureHasValue(ref _PlatformAdministrator); } }

        private static Role _OrgAdministrator;
        public static Role OrgAdministrator { get { return EnsureHasValue(ref _OrgAdministrator); } }

        private static Role _OrgUser;
        public static Role OrgUser { get { return EnsureHasValue(ref _OrgUser); } }

        private static Role _OrgTemplateManagement;
        public static Role OrgTemplateManagement { get { return EnsureHasValue(ref _OrgTemplateManagement); } }

        private static Role _OrgUserManagement;
        public static Role OrgUserManagement { get { return EnsureHasValue(ref _OrgUserManagement); } }

        private static Role _OrgProjectManagement;
        public static Role OrgProjectManagement { get { return EnsureHasValue(ref _OrgProjectManagement); } }
    }
}
