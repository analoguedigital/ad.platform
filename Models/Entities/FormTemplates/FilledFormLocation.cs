using System;

namespace LightMethods.Survey.Models.Entities
{
    public class FilledFormLocation : Entity
    {
        public Guid FilledFormId { set; get; }

        public virtual FilledForm FilledForm { set; get; }

        public double Latitude { set; get; }

        public double Longitude { set; get; }

        // formatted address coming from Geocoding API
        public string Address { get; set; }

        public double Accuracy { set; get; }

        public string Error { set; get; }

        public string Event { set; get; }
    }
}
