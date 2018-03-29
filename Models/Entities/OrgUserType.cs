using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.DAL;

namespace LightMethods.Survey.Models.Entities
{
    public class OrgUserType : Entity, IEnumEntity
    {
        public int Order { get; set; }

        public string Name { get; set; }

        public string SystemName { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<string> GetRoles()
        {
            yield return Role.ORG_USER;

            if (Id == OrgUserTypesRepository.Administrator.Id)
            {
                yield return Role.ORG_ADMINSTRATOR;
            }
            else if (Id == OrgUserTypesRepository.Manager.Id)
            {
                yield return Role.ORG_TEAM_MANAGER;
                yield return Role.ORG_PROJECT_MANAGMENT;
                yield return Role.ORG_TEMPLATES_MANAGMENT;
            }
        }
    }
}
