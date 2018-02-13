using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.Entities
{
    public class Role : IdentityRole<Guid, UserRole>, IEntity
    {
        public const string SYSTEM_ADMINSTRATOR = "System administrator";
        public const string PLATFORM_ADMINISTRATOR = "Platform administrator";

        public const string ORG_ADMINSTRATOR = "Organisation administrator";
        public const string ORG_TEAM_USER = "Organisation team user";
        public const string ORG_TEAM_MANAGER = "Organisation team manager";
        public const string ORG_USER = "Organisation user";

        public const string ORG_USER_MANAGMENT = "Users manager";
        public const string ORG_PROJECT_MANAGMENT = "Project manager";
        public const string ORG_TEMPLATES_MANAGMENT = "Form templates manager";

        public const string RESTRICTED_USER = "Restricted user";
    }
}
