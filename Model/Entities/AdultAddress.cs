using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class AdultAddress: Address
    {

        public Guid AdultId { set; get; }
        public  virtual Adult Adult { set; get; }

        public Guid AddressTypeId { get; set; }
        [Display(Name="Type")]
        public virtual AddressType AddressType { get; set; }

    }
}
