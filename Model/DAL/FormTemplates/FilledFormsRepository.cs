using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class FilledFormsRepository : Repository<FilledForm>
    {
        public FilledFormsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        public FilledForm GetFullFilledFormAndTemplate(Guid id)
        {
            return FindIncluding(id
                , f => f.FormValues
                , f => f.FormTemplate.MetricGroups.Select(g => g.Metrics));

        }

        //private static object _lock;
        //public int GetSerialNumber(Guid organisationId, Guid formTemplateId)
        //{
        //    lock (_lock)
        //    {
        //        return All
        //            .Where(f => f.FormTemplateId == formTemplateId && f.Project.OrganisationId == organisationId)
        //            .DefaultIfEmpty()
        //            .Max(f => f == null ? 0 : f.Serial) + 1;
        //    }
        //}


        public FilledForm InsertOrUpdate(FilledForm filledForm, Project project)
        {
            FilledForm result = null;
            using (var uow = new UnitOfWork(Context))
            {
                if (filledForm.Id == Guid.Empty)
                {
                    filledForm.ProjectId = project.Id;
                    uow.FormValuesRepository.InsertOrUpdate(filledForm.FormValues);
                    filledForm.FormValues.ToList().ForEach(a => a.Id = Guid.NewGuid());
                    InsertOrUpdate(filledForm);
                    result = filledForm;
                }
                else
                {
                    var dbEntity = Find(filledForm.Id);
                    UpdateCore(uow, dbEntity, filledForm, project);
                    result = dbEntity;
                }
            }
            return result;
        }

        protected FilledForm UpdateCore(UnitOfWork uow, FilledForm dest, FilledForm src, Project project)
        {


            // Context.Entry(dest).CurrentValues.SetValues(src);

            // dest.ProjectId = project.Id;
            //  InsertOrUpdate(dest);

            //   if (src.FormValues != null)
            //      src.FormValues.Where(v=>v.FilledFormId == Guid.Empty).ToList().ForEach(a => a.FilledForm = dest);
            uow.FormValuesRepository.Update(dest.FormValues, src.FormValues);

            return dest;
        }


        public override void Delete(FilledForm entity)
        {
            entity.FormValues.ToList().ForEach(v => CurrentUOW.FormValuesRepository.Delete(v));

            base.Delete(entity);

        }
    }
}
