using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.DTO
{
    public class UpdateSubscriptionEntryDTO
    {
        public Guid RecordId { get; set; }

        public UserSubscriptionType Type { get; set; }
    }
}
