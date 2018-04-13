using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class Organisation : Entity
    {
        [Display(Name = "Organisation Name")]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(30)]
        [Display(Name = "Telephone Number")]
        public string TelNumber { get; set; }

        [StringLength(30)]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [StringLength(30)]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [StringLength(30)]
        public string Town { get; set; }

        [StringLength(30)]
        public string County { get; set; }

        [StringLength(8)]
        public string Postcode { get; set; }

        [ReadOnly(true)]
        public bool IsActive { set; get; }

        [ReadOnly(true)]
        public Guid? RootUserId { set; get; }

        [ReadOnly(true)]
        [Display(Name = "Root User")]
        public virtual OrgUser RootUser { get; set; }

        [Display(Name = "Default language")]
        public virtual Language DefaultLanguage { set; get; }
        public Guid DefaultLanguageId { set; get; }

        [Display(Name = "Default calendar")]
        public virtual Calendar DefaultCalendar { set; get; }
        public Guid DefaultCalendarId { set; get; }

        public virtual ICollection<OrgUser> OrgUsers { get; set; }
        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<FormTemplate> FormTemplates { get; set; }
        public virtual ICollection<FormTemplateCategory> FormTemplateCategories { get; set; }

        public virtual ICollection<ReportTemplate> ReportTemplates { get; set; }
        public virtual ICollection<ReportTemplateCategory> ReportTemplateCategories { get; set; }

        public virtual ICollection<DataList> DataLists { set; get; }

        public bool SubscriptionEnabled { get; set; }

        public decimal? SubscriptionMonthlyRate { get; set; }

        public virtual ICollection<Voucher> Vouchers { get; set; }

        public virtual ICollection<OrganisationTeam> Teams { get; set; }

        public virtual ICollection<OrganisationInvitation> Invitations { get; set; }

        public Organisation()
        {
            IsActive = true;
        }

        public string Status
        {
            get
            {
                return IsActive ? "Active" : "Deactivated";
            }
        }

        public ICollection<DataList> GetGlobalDataLists()
        {
            if (DataLists == null)
                return new List<DataList>();

            return DataLists.Where(d => !d.IsAdHoc).ToList();
        }

    }
}
