using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class FormValuesRepository : Repository<FormValue>
    {
        public FormValuesRepository(UnitOfWork uow) : base(uow) { }

        public override void Delete(FormValue entity)
        {
            entity.Attachments.ToList().ForEach(a => this.CurrentUOW.AttachmentsRepository.Delete(a));
            base.Delete(entity);
        }
    }
}
