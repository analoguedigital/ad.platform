using System;

namespace LightMethods.Survey.Models.DTO
{
    public class CloneReqDTO
    {
        public string Code { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Colour { get; set; }

        // this shouldn't be nullable! it is possible
        // to store nulls in the DB, but a thread should belong 
        // to a project explicitly.
        public Guid? ProjectId { get; set; }
    }
}
