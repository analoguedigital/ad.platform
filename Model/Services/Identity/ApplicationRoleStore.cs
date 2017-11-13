using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services.Identity
{
    public class ApplicationRoleStore : RoleStore<Role, Guid, UserRole>, IQueryableRoleStore<Role, Guid>, IRoleStore<Role, Guid>, IDisposable
    {

        public ApplicationRoleStore()
            : base(new SurveyContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(SurveyContext context)
            : base(context)
        {
        }
    }
}
