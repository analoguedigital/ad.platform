using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class ReportItem : Entity
    {

        public virtual ReportTemplate ReportTemplate { set; get; }
        public virtual Guid ReportTemplateId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public int Order { set; get; }


        public void MoveUp()
        {
            var otherItems = ReportTemplate.Items.Where(g => g.Order < Order).ToList();
            if (!otherItems.Any())
                return;

            var MaxSmaller = otherItems.Max(g => g.Order);
            if (MaxSmaller > 0)
            {

                var smaller = otherItems.Where(g => g.Order == MaxSmaller).First();
                smaller.Order = Order;
                Order = MaxSmaller;
            }
        }

        public void MoveDown()
        {
            var otherItems = ReportTemplate.Items.Where(g => g.Order > Order).ToList();
            if (!otherItems.Any())
                return;

            var MinBigger = otherItems.Min(g => g.Order);
            if (MinBigger > 0)
            {

                var bigger = otherItems.Where(g => g.Order == MinBigger).First();
                bigger.Order = Order;
                Order = MinBigger;
            }
        }

    }
}
