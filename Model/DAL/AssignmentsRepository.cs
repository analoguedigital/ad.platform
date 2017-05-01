using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class AssignmentsRepository : Repository<Assignment>
    {
        public AssignmentsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

    }
}
