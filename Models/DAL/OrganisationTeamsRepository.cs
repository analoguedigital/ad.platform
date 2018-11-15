using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class OrganisationTeamsRepository : Repository<OrganisationTeam>
    {
        internal OrganisationTeamsRepository(UnitOfWork uow) : base(uow) { }
    }
}
