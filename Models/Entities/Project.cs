using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class Project : Entity
    {
        [NotMapped]
        public virtual bool AdditionalFieldsNeeded { get { return true; } }

        [Required]
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        [Display(Name = "Project number")]
        public string Number { get; set; }

        [Required]
        [Display(Name = "Project name")]
        public string Name { get; set; }

        #region IDocumentOwner
        public IEnumerable<Document> Documents { get; set; }
        #endregion

        [Display(Name = "Start date")]
        public DateTime? StartDate { set; get; }

        [Display(Name = "End date")]
        public DateTime? EndDate { set; get; }

        public bool Flagged { set; get; }

        public bool Archived { set; get; }

        public string Notes { set; get; }

        public virtual ICollection<FormTemplate> FormTemplates { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }

        public virtual IEnumerable<KeyLocation> KeyLocations { get; set; }

        [Display(Name = "Created by")]
        public virtual User CreatedBy { set; get; }

        public Guid? CreatedById { set; get; }

        public Project()
        {
            StartDate = DateTime.Today;
            Flagged = Archived = false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
