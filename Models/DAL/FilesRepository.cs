using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class FilesRepository : Repository<File>
    {
        public FilesRepository(UnitOfWork uow)
            : base(uow)
        {

        }

    }
}
