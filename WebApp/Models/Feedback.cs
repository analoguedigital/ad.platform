using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class FeedbackDTO
    {
        public Guid Id { get; set; }

        public OrgUserDTO AddedBy { get; set; }

        public Guid AddedById { get; set; }

        public OrganisationDTO Organisation { get; set; }

        public Guid OrganisationId { get; set; }

        public DateTime AddedAt { get; set; }

        public string Comment { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}