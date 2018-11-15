using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public class Address : Entity, IAddress
    {

        [Required]
        [MaxLength(30)]
        [Display(Name = "Address line 1")]
        public string AddressLine1 { get; set; }

        [MaxLength(30)]
        [Display(Name = "Address line 2")]
        public string AddressLine2 { get; set; }

        [MaxLength(30)]
        [Display(Name = "Address line 3")]
        public string AddressLine3 { get; set; }

        [MaxLength(30)]
        [Display(Name = "City")]
        public string Town { get; set; }

        [MaxLength(30)]
        [Display(Name = "State")]
        public string County { get; set; }

        [MaxLength(30)]
        public string Country { get; set; }

        [MaxLength(10)]
        [Display(Name = "Zip code")]
        public string Postcode { get; set; }

        [MaxLength(50)]
        public string Note { set; get; }

        public override string ToString()
        {
            var items = new[] { AddressLine1, AddressLine2, AddressLine3, Town, County, Country, Postcode, Note };

            return string.Join(", ", items.Where(i => i.HasValue()));
        }
    }
}
