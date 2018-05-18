﻿using System;

namespace LightMethods.Survey.Models.DTO
{
    public class VoucherDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public int Period { get; set; }

        public bool IsRedeemed { get; set; }

        public Guid OrganisationId { get; set; }

        public OrganisationDTO Organisation { get; set; }
    }
}
