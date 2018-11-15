using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public class MetricGroup : Entity, IHasDataList, IHasAdHocDataList
    {
        [Required]
        public String Title { get; set; }

        [Display(Name = "Contextual help")]
        [DataType(DataType.MultilineText)]
        public string HelpContext { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { set; get; }

        public int Order { set; get; }

        [Index]
        public Guid FormTemplateId { get; set; }

        public virtual FormTemplate FormTemplate { get; set; }

        [NotMapped]
        [Display(Name = "Is repeater")]
        public bool IsRepeater { get { return this.Type > 0; } }

        [NotMapped]
        public MetricGroupType Type { get { return GetTypeValue(); } }

        /// Data List Repeater
        public Guid? DataListId { set; get; }

        public virtual DataList DataList { set; get; }

        /// Iterative Repeater
        [Display(Name = "Number of rows")]
        [Range(0, 100)]
        public int? NumberOfRows { set; get; }

        [Display(Name = "Can add more rows")]
        [UIHint("YesNo")]
        public bool CanAddMoreRows { set; get; }

        public virtual ICollection<Metric> Metrics { set; get; }

        /// <summary>
        /// A metric group is single if it is the only group on a page
        /// </summary>
        [NotMapped]
        public bool IsSingle
        {
            get
            {
                return FormTemplate == null ? true : !FormTemplate.MetricGroups.Any(g => g.Page == this.Page && g.Id != this.Id);
            }
        }

        [NotMapped]
        public int PageOrder
        {
            get
            {
                return Page * 1000 + Order;
            }
        }

        public MetricGroup()
        {
            Order = 1;
            Page = 1;
            Metrics = new List<Metric>();
        }

        public Metric AddMetric(Metric metric)
        {
            metric.Order = GetMaxMetricOrder() + 1;
            metric.MetricGroup = this;
            metric.MetricGroupId = this.Id;
            metric.FormTemplateId = this.FormTemplateId;
            Metrics.Add(metric);

            return metric;
        }

        public int GetMaxMetricOrder()
        {
            if (Metrics == null || !Metrics.Any())
                return 1000;

            return Metrics.Max(g => g.Order);
        }

        public void MoveUp()
        {
            if (IsSingle)
            {
                MoveUpOnePage();
            }
            else
            {
                var otherGroups = FormTemplate.MetricGroups.Where(g => g.PageOrder < PageOrder).ToList();
                if (!otherGroups.Any())
                    return;

                if (otherGroups.OrderByDescending(g => g.PageOrder).First().Page != this.Page)
                    MoveUpOnePage();
                else
                    MoveUpWithinPage();
            }
        }

        public void MoveDown()
        {
            if (IsSingle)
            {
                MoveDownOnePage();
            }
            else
            {
                var otherGroups = FormTemplate.MetricGroups.Where(g => g.PageOrder > PageOrder).ToList();
                if (!otherGroups.Any())
                    return;

                if (otherGroups.OrderBy(g => g.PageOrder).First().Page != this.Page)
                    MoveDownOnePage();
                else
                    MoveDownWithinPage();
            }
        }

        private void MoveDownWithinPage()
        {
            var otherGroups = FormTemplate.MetricGroups.Where(g => g.Order > Order && g.Page == Page).ToList();
            if (!otherGroups.Any())
                return;

            var MinBigger = otherGroups.Min(g => g.Order);
            if (MinBigger > 0)
            {

                var bigger = otherGroups.Where(g => g.Order == MinBigger).First();
                bigger.Order = Order;
                Order = MinBigger;
            }
        }

        private void MoveUpWithinPage()
        {
            var otherGroups = FormTemplate.MetricGroups.Where(g => g.Order < Order && g.Page == Page).ToList();

            var MaxSmaller = otherGroups.Max(g => g.Order);
            if (MaxSmaller > 0)
            {

                var smaller = otherGroups.Where(g => g.Order == MaxSmaller).First();
                smaller.Order = Order;
                Order = MaxSmaller;
            }
        }

        private void MoveUpOnePage()
        {
            if (Page <= 1) return;

            var lastPageGroups = FormTemplate.MetricGroups.Where(g => g.Page == Page - 1).ToList();
            var currentPageGroups = FormTemplate.MetricGroups.Where(g => g.Page == Page).ToList();

            lastPageGroups.ForEach(g => g.Page = this.Page);
            var newPageNumber = this.Page - 1;
            currentPageGroups.ForEach(g => g.Page = newPageNumber);
        }

        private void MoveDownOnePage()
        {
            var nextPageGroups = FormTemplate.MetricGroups.Where(g => g.Page == Page + 1).ToList();
            if (nextPageGroups.Any())
            {
                var currentPageGroups = FormTemplate.MetricGroups.Where(g => g.Page == Page).ToList();

                nextPageGroups.ForEach(g => g.Page = this.Page);
                var newPageNumber = this.Page + 1;
                currentPageGroups.ForEach(g => g.Page = newPageNumber);
            }
        }

        public void MergeUp()
        {
            if (Page == 1)
                return;

            // Shift next pages 
            FormTemplate.MetricGroups.Where(g => g.Page > Page).ToList().ForEach(g => g.Page--);

            // Find the max order 
            var maxOrderInPreviusPage = FormTemplate.MetricGroups.Where(g => g.Page == Page - 1).Max(g => g.Order);
            Page--;
            Order = maxOrderInPreviusPage + 1;
        }

        public void Detach()
        {
            // Shift next pages 
            FormTemplate.MetricGroups.Where(g => g.Page > Page).ToList().ForEach(g => g.Page++);

            Page++;
            Order = 1;
        }

        internal void PrepareForDelete()
        {
            if (IsSingle)
                FormTemplate.MetricGroups.Where(g => g.Page >= Page).ToList().ForEach(g => g.Page--);
        }

        private MetricGroupType GetTypeValue()
        {
            if (DataListId.HasValue || DataList != null)
                return MetricGroupType.DataListRepeater;

            if (NumberOfRows.HasValue)
                return MetricGroupType.IterativeRepeater;

            return MetricGroupType.Single;
        }

        public MetricGroup Clone(FormTemplate cloneTemplate)
        {
            var clonedGroup = new MetricGroup
            {
                FormTemplate = cloneTemplate,
                CanAddMoreRows = CanAddMoreRows,
                DataList = DataList?.IsAdHoc ?? false ? DataList.Clone() : (DataList)null, // Create a new datalist only if it is an ad-hoc
                DataListId = DataList?.IsAdHoc ?? true ? (Guid?)null : DataListId,
                HelpContext = HelpContext,
                NumberOfRows = NumberOfRows,
                Order = Order,
                Page = Page,
                Title = Title
            };

            foreach (var metric in Metrics)
                clonedGroup.Metrics.Add(metric.Clone(cloneTemplate, clonedGroup));

            return clonedGroup;
        }
    }
}
