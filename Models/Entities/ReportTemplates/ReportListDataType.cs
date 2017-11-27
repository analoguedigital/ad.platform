using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class ReportListDataType : Entity, IEnumEntity
    {
        [ScaffoldColumn(false)]
        public int Order { get; set; }


        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public string SystemName { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
