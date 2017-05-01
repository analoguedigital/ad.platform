using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class FormTemplateCategoriesRepository : Repository<FormTemplateCategory>
    {
        public FormTemplateCategoriesRepository(UnitOfWork uow)
            : base(uow)
        {

        }


    }
}
