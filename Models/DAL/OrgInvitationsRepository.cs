using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class OrgInvitationsRepository : Repository<OrganisationInvitation>
    {
        public OrgInvitationsRepository(UnitOfWork uow) : base(uow) { }
    }
}
