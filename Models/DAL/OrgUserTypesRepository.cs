using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class OrgUserTypesRepository : Repository<OrgUserType>
    {
        public OrgUserTypesRepository(UnitOfWork uow)
            : base(uow)
        {

        }


        private static OrgUserType EnsureHasValue(ref OrgUserType prop)
        {
            if (prop != null)
                return prop;

            using (var uow = new UnitOfWork(new SurveyContext()))
            {
                var types = uow.OrgUserTypesRepository.All.ToList();
                _Administrator = types.Where(i => i.SystemName == "Administrator").Single();
                _Manager = types.Where(i => i.SystemName == "Manager").Single();
                _TeamUser = types.Where(i => i.SystemName == "TeamUser").Single();
                _ExternalUser = types.Where(i => i.SystemName == "ExternalUser").Single();
            }

            return prop;
        }

        private static OrgUserType _Administrator;
        public static OrgUserType Administrator { get { return EnsureHasValue(ref _Administrator); } }

        private static OrgUserType _Manager;
        public static OrgUserType Manager { get { return EnsureHasValue(ref _Manager); } }

        private static OrgUserType _TeamUser;
        public static OrgUserType TeamUser { get { return EnsureHasValue(ref _TeamUser); } }

        private static OrgUserType _ExternalUser;
        public static OrgUserType ExternalUser { get { return EnsureHasValue(ref _ExternalUser); } }

    }
}
