using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DAL
{
    public class PlatformUsersRepository : Repository<PlatformUser>
    {
        public PlatformUsersRepository(UnitOfWork uow) : base(uow) { }
    }
}
