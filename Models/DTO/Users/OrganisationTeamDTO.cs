using System.Collections.Generic;

namespace LightMethods.Survey.Models.DTO
{
    public class OrganisationTeamDTO
    {
        public string Id { get; set; }

        public OrganisationDTO Organisation { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Colour { get; set; }

        public bool IsActive { get; set; }

        public ICollection<OrgTeamUserDTO> Users { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is OrganisationTeamDTO item))
                return false;

            return this.Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
