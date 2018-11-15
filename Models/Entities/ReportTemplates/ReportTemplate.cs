using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public class ReportTemplate : Entity
    {
        public Guid OrganisationId { set; get; }

        public Organisation Organisation { set; get; }

        [MaxLength(100)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { set; get; }

        public virtual ICollection<ReportItem> Items { get; set; }

        [Required(ErrorMessage = "The report category is required.")]
        public Guid CategoryId { set; get; }

        public ReportTemplateCategory Category { set; get; }

        [Required]
        [Display(Name = "Published?")]
        [UIHint("YesNo")]
        public bool IsPublished { set; get; }

        public ReportTemplate()
        {
            IsPublished = false;
            Items = new List<ReportItem>();
        }

        public ReportItem AddItem(ReportItem item)
        {
            item.Order = GetMaxItemOrder() + 1;
            item.ReportTemplate = this;
            Items.Add(item);

            return item;
        }

        public int GetMaxItemOrder()
        {
            if (Items == null || !Items.Any())
                return 1000;

            return Items.Max(g => g.Order);
        }

        public void Publish()
        {
            IsPublished = true;
        }
    }
}
