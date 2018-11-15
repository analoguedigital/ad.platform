using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public class KeyLocation: Entity 
    {
        [Required]
        [Display(Name="Start date")]
        public DateTime StartDate { set; get; }

        [Display(Name="End date")]
        public DateTime? EndDate { set; get; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public Guid ProjectId { get; set; }

        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        public Guid AddressId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { set; get; }
    }
}
