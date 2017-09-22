using System;

namespace LightMethods.Survey.Models.DTO
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
