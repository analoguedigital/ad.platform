using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class Adult : Entity, IAdult
    {
        [NotMapped]
        public virtual bool AdditionalFieldsNeeded { get { return true; } }

        [ScaffoldColumn(false)]
        public Guid ProjectId { get; set; }

        [ScaffoldColumn(false)]
        public Project Project { get; set; }

        public virtual AdultTitle Title { get; set; }

        public Guid? TitleId { set; get; }

        [Required]
        [Display(Name = "First names")]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string Surname { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        // additional field
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [NotMapped]
        public string Gender
        {
            get { return GenderData == "F" ? "Female" : "Male"; }
            set { GenderData = value[0].ToString().ToUpper();  }
        }

        [Required]
        [MaxLength(1)]
        public string GenderData { get; set; }

        public virtual ICollection<AdultContactNumber> ContactNumbers { get; set; }

        public virtual ICollection<AdultAddress> Addresses { get; set; }

        public string FullName
        {
            get
            {
                var result = FirstName;
                result += " " + Surname;
                return result;
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
