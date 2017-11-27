using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class FilledFormLocation : Entity
    {
        public Guid FilledFormId { set; get; }
        public virtual FilledForm FilledForm { set; get; }

        public double Latitude { set; get; }
        public double Longitude { set; get; }
        public double Accuracy { set; get; }
        public string Error { set; get; }
        public string Event { set; get; }
    }
}
