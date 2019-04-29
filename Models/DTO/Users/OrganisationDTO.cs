using System;

namespace LightMethods.Survey.Models.DTO
{
    public class OrganisationDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public OrgUserDTO RootUser { get; set; }

        public string RootUserEmail { get; set; }

        public string TelNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Town { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        public string LogoUrl { get; set; }

        public string TermsAndConditions { get; set; }

        public bool RequiresAgreement { get; set; }

        public string FacebookId { get; set; }

        public string TwitterId { get; set; }

        public string InstagramId { get; set; }

        public string SkypeId { get; set; }

        public string LinkedinUrl { get; set; }

        public string YouTubeUrl { get; set; }

        public Guid DefaultLanguageId { set; get; }

        public Guid DefaultCalendarId { set; get; }

        public bool SubscriptionEnabled { get; set; }

        public decimal? SubscriptionMonthlyRate { get; set; }
    }
}
