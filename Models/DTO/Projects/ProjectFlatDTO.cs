using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class ProjectFlatDTO
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public DateTime? LastEntry { get; set; }

        public ProjectCreatorFlatDTO CreatedBy { get; set; }

        public int AssignmentsCount { get; set; }

        public int TeamsCount { get; set; }

        public bool IsAggregate { get; set; }
    }

    public class ProjectCreatorFlatDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public AccountType AccountType { get; set; }
    }
}
