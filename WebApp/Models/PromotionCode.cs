using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class PromotionCodeDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public decimal Amount { get; set; }

        public bool IsRedeemed { get; set; }

        public Guid OrganisationId { get; set; }

        public OrganisationDTO Organisation { get; set; }
    }
}