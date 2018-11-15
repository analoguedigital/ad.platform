﻿using System;

namespace LightMethods.Survey.Models.DTO
{
    public class OrgInvitationDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }

        public int Limit { get; set; }

        public int Used { get; set; }

        public bool IsActive { get; set; }

        public OrganisationDTO Organisation { get; set; }
    }
}
