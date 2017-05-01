﻿using LightMethods.Survey.Models.DAL;
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

    public class ApplicationUserStore : UserStore<User, Role, Guid, UserLogin, UserRole, UserClaim>, IUserStore<User, Guid>, IDisposable
    {
        public ApplicationUserStore()
            : this(new SurveyContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(SurveyContext context)
            : base(context)
        {
        }
    }
}
