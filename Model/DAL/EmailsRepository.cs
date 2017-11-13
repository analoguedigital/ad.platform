using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DAL
{
    public class EmailsRepository : Repository<Email>
    {
        public EmailsRepository(UnitOfWork uow) : base(uow) { }
    }
}
