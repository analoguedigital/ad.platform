﻿using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DAL
{
    public class SubscriptionPlansRepository : Repository<SubscriptionPlan>
    {
        public SubscriptionPlansRepository(UnitOfWork uow) : base(uow) { }
    }
}