﻿using System;

namespace LightMethods.Survey.Models.DTO
{
    public class SubscriptionDTO
    {
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Note { get; set; }

        public Guid PaymentRecordId { get; set; }

        public PaymentRecordDTO PaymentRecord { get; set; }

        public Guid OrgUserId { get; set; }

        public OrgUserDTO OrgUser { get; set; }
    }

    public class LatestSubscriptionDTO
    {
        public DateTime? Date { get; set; }
    }
}