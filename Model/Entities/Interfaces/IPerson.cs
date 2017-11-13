using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public interface IPerson :IEntity
    {
        bool AdditionalFieldsNeeded { get; }

        [Required]
        [Display(Name = "First names")]
        [StringLength(30)]
        string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        string Surname { get; set; }

        [Required]
        string Gender { get; set; }

        [Display(Name = "Date of birth")]
        DateTime? DateOfBirth { get; set; }
    }
}
