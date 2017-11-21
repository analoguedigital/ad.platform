using System;

namespace LightMethods.Survey.Models.DTO
{
    public class FilledFormLocationDTO
    {
        public Guid FilledFormId { set; get; }

        public double Latitude { set; get; }

        public double Longitude { set; get; }

        public double Accuracy { set; get; }

        public string Error { set; get; }

        public string Event { set; get; }
    }
}
