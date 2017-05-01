using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;
using System.Collections.Specialized;
using AppHelper;

namespace LightMethods.Survey.Models.DAL
{

    public class AdultsGenericsRepository<T> : Repository<T>, IAdultsRepository<T> where T : Adult, new()
    {
        public AdultsGenericsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        public T InsertOrUpdate(T adult, Project project)
        {
            T result = null;
            using (var uow = new UnitOfWork(Context))
            {
                if (adult.Id == Guid.Empty)
                {
                    adult.ProjectId = project.Id;
                    uow.AdultAddressesRepository.InsertOrUpdate(adult.Addresses);
                    if (adult.Addresses != null)
                        adult.Addresses.ToList().ForEach(a => a.Id = Guid.NewGuid());

                    uow.AdultContactNumbersRepository.InsertOrUpdate(adult.ContactNumbers);
                    if (adult.ContactNumbers != null)
                        adult.ContactNumbers.ToList().ForEach(c => c.Id = Guid.NewGuid());
                    InsertOrUpdate(adult);
                    result = adult;
                }
                else
                {
                    var dbEntity = Find(adult.Id);
                    UpdateCore(uow, dbEntity, adult, project);
                    result = dbEntity;
                }
            }
            return result;
        }

        public T CreateOrUpdate(T destAdult, T srcAdult, Project project)
        {

            using (var uow = new UnitOfWork(Context))
            {
                if (destAdult == null)
                {
                    destAdult = new T() { Project = project };
                    Context.Adults.Add(destAdult);
                }

                UpdateCore(uow, destAdult, srcAdult, project);
                //Context.Entry(dest).CurrentValues.SetValues(src);
                //dest.ProjectId = project.Id;
                //InsertOrUpdate(dest);

                //if (src.ContactNumbers != null)
                //    src.ContactNumbers.ToList().ForEach(a => a.Adult = dest);
                //uow.ContactNumbersRepository.Update(dest.ContactNumbers, src.ContactNumbers);

                //if (src.Addresses != null)
                //    src.Addresses.ToList().ForEach(a => a.Adult = dest);
                //uow.AdultAddressesRepository.Update(dest.Addresses, src.Addresses);

            }
            return destAdult;
        }

        protected T UpdateCore(UnitOfWork uow, T dest, T src, Project project)
        {


            Context.Entry(dest).CurrentValues.SetValues(src);
            dest.ProjectId = project.Id;
            InsertOrUpdate(dest);

            if (src.ContactNumbers != null)
                src.ContactNumbers.ToList().ForEach(a => a.Adult = dest);
            uow.AdultContactNumbersRepository.Update(dest.ContactNumbers, src.ContactNumbers);

            if (src.Addresses != null)
                src.Addresses.ToList().ForEach(a => a.Adult = dest);
            uow.AdultAddressesRepository.Update(dest.Addresses, src.Addresses);


            return dest;
        }

    }

    public class AdultsRepository : AdultsGenericsRepository<Adult>
    {
        public AdultsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

    }
}
