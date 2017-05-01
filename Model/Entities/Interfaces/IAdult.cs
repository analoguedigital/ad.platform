﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMethods.Survey.Models.Entities
{
    public interface IAdult : IPerson, ICaseItem
    {
        
        AdultTitle Title { set; get; }

        string EmailAddress { get; set; }

        ICollection<AdultContactNumber> ContactNumbers { get; set; }

        ICollection<AdultAddress> Addresses { get; set; }

    }
}
