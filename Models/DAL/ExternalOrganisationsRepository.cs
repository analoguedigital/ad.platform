using LightMethods.Survey.Models.Entities;
using System;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class ExternalOrganisationsRepository : Repository<ExternalOrganisation>
    {
        internal ExternalOrganisationsRepository(UnitOfWork uow) : base(uow) { }

        public override void InsertOrUpdate(ExternalOrganisation entity)
        {
            base.InsertOrUpdate(entity);
        }

        public ExternalOrganisation InsertOrUpdate(ExternalOrganisation org, Project project)
        {
            ExternalOrganisation result = null;
            using (var uow = new UnitOfWork(Context))
            {
                if (org.Id == Guid.Empty)
                {
                    org.ProjectId = project.Id;
                    uow.AddressesRepository.InsertOrUpdate(org.Address);
                    if (org.Address != null)
                        org.Address.Id = Guid.NewGuid();

                    uow.ExternalOrgContactNumbersRepository.InsertOrUpdate(org.ContactNumbers);
                    if (org.ContactNumbers != null)
                        org.ContactNumbers.ToList().ForEach(c => c.Id = Guid.NewGuid());

                    InsertOrUpdate(org);
                    result = org;
                }
                else
                {
                    var dbEntity = Find(org.Id);
                    UpdateCore(uow, dbEntity, org, project);
                    result = dbEntity;
                }
            }
            return result;
        }

        protected ExternalOrganisation UpdateCore(UnitOfWork uow, ExternalOrganisation dest, ExternalOrganisation src, Project project)
        {

            Context.Entry(dest).CurrentValues.SetValues(src);
            dest.ProjectId = project.Id;
            InsertOrUpdate(dest);

            if (src.ContactNumbers != null)
                src.ContactNumbers.ToList().ForEach(a => a.Organisation = dest);
            uow.ExternalOrgContactNumbersRepository.Update(dest.ContactNumbers, src.ContactNumbers);

            return dest;
        }

    }
}
