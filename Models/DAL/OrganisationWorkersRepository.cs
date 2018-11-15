using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class OrganisationWorkersRepository : AdultsGenericsRepository<OrganisationWorker>
    {
        internal OrganisationWorkersRepository(UnitOfWork uow) : base(uow) { }
    }
}
