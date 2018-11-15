using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public class DataList : Entity, IArchivable
    {
        public const int MAX_RELATIONSHIPS = 10;

        public virtual Organisation Organisation { set; get; }

        public Guid OrganisationId { set; get; }

        [MaxLength(50)]
        public string Name { set; get; }

        public bool IsAdHoc { get { return string.IsNullOrEmpty(Name); } }

        public virtual ICollection<DataListItem> AllItems { set; get; }

        public IEnumerable<DataListItem> Items { get { return AllItems.Where(i => !i.IsArchived()).OrderBy(i => i.Order); } }

        public virtual ICollection<DataListRelationship> NotOrderedRelationships { set; get; }

        public IEnumerable<DataListRelationship> Relationships { get { return NotOrderedRelationships?.OrderBy(r => r.Order); } }

        public DateTime? DateArchived { get; set; }

        public void MoveUp(DataListItem item)
        {
            var otherItems = this.Items.Where(i => i.Order < item.Order).ToList();
            if (!otherItems.Any())
                return;

            var MaxSmaller = otherItems.Max(i => i.Order);
            if (MaxSmaller > 0)
            {

                var smaller = otherItems.Where(i => i.Order == MaxSmaller).Last();
                smaller.Order = item.Order;
                item.Order = MaxSmaller;
            }
        }

        public void MoveDown(DataListItem item)
        {
            var otherItems = this.Items.Where(i => i.Order > item.Order).ToList();
            if (!otherItems.Any())
                return;

            var MinBigger = otherItems.Min(i => i.Order);
            if (MinBigger > 0)
            {

                var bigger = otherItems.Where(i => i.Order == MinBigger).First();
                bigger.Order = item.Order;
                item.Order = MinBigger;
            }
        }

        //public void AddRelationship(DataListRelationship relationship)
        //{
        //    if (AllRelationships.Count() == MAX_RELATIONSHIPS)
        //        throw new ValidationException(string.Format("A new relationship cannot be added to {0}!", Name));

        //    if (Relationship1Id == null) Relationship1 = relationship;
        //    else if (Relationship2Id == null) Relationship2 = relationship;
        //    else if (Relationship3Id == null) Relationship3 = relationship;
        //    else if (Relationship4Id == null) Relationship4 = relationship;
        //    else if (Relationship5Id == null) Relationship5 = relationship;
        //    else if (Relationship6Id == null) Relationship6 = relationship;
        //    else if (Relationship7Id == null) Relationship7 = relationship;
        //    else if (Relationship8Id == null) Relationship8 = relationship;
        //    else if (Relationship9Id == null) Relationship9 = relationship;
        //}

        //public void RemoveRelationship(DataListRelationship relationship)
        //{
        //    if (Relationship1Id == relationship.Id) { Relationship1Id = null; this.AllItems.ToList().ForEach(i => i.Attr1Id = null); }
        //    else if (Relationship2Id == relationship.Id) { Relationship2Id = null; this.AllItems.ToList().ForEach(i => i.Attr2Id = null); }
        //    else if (Relationship3Id == relationship.Id) { Relationship3Id = null; this.AllItems.ToList().ForEach(i => i.Attr3Id = null); }
        //    else if (Relationship4Id == relationship.Id) { Relationship4Id = null; this.AllItems.ToList().ForEach(i => i.Attr4Id = null); }
        //    else if (Relationship5Id == relationship.Id) { Relationship5Id = null; this.AllItems.ToList().ForEach(i => i.Attr5Id = null); }
        //    else if (Relationship6Id == relationship.Id) { Relationship6Id = null; this.AllItems.ToList().ForEach(i => i.Attr6Id = null); }
        //    else if (Relationship7Id == relationship.Id) { Relationship7Id = null; this.AllItems.ToList().ForEach(i => i.Attr7Id = null); }
        //    else if (Relationship8Id == relationship.Id) { Relationship8Id = null; this.AllItems.ToList().ForEach(i => i.Attr8Id = null); }
        //    else if (Relationship9Id == relationship.Id) { Relationship9Id = null; this.AllItems.ToList().ForEach(i => i.Attr9Id = null); }
        //}

        public void MoveUp(DataListRelationship item)
        {
            var otherItems = this.Relationships.Where(i => i.Order < item.Order).ToList();
            if (!otherItems.Any())
                return;

            var MaxSmaller = otherItems.Max(i => i.Order);
            if (MaxSmaller > 0)
            {

                var smaller = otherItems.Where(i => i.Order == MaxSmaller).Last();
                smaller.Order = item.Order;
                item.Order = MaxSmaller;
            }
        }

        public void MoveDown(DataListRelationship item)
        {
            var otherItems = this.Relationships.Where(i => i.Order > item.Order).ToList();
            if (!otherItems.Any())
                return;

            var MinBigger = otherItems.Min(i => i.Order);
            if (MinBigger > 0)
            {

                var bigger = otherItems.Where(i => i.Order == MinBigger).First();
                bigger.Order = item.Order;
                item.Order = MinBigger;
            }
        }

        //public void MoveUp(DataListRelationship relationship)
        //{
        //    PropertyInfo relationshipSource = null;
        //    PropertyInfo relationshipTarget = null;

        //    foreach (PropertyInfo property in this.GetType().GetProperties().Where(p => p.PropertyType == typeof(Guid?) && p.Name.StartsWith("Relationship")).OrderBy(p => p.Name))
        //    {
        //        var val = (Guid?)(property.GetValue(this));
        //        if (val == relationship.Id)
        //        {
        //            relationshipSource = property;
        //            break;
        //        }
        //        else
        //        {
        //            if (val != null)
        //                relationshipTarget = property;
        //        }
        //    }

        //    if (relationshipSource != null && relationshipTarget != null)
        //    {
        //        relationshipSource.SetValue(this, relationshipTarget.GetValue(this));
        //        relationshipTarget.SetValue(this, relationship.Id);
        //    }
        //}

        //public void MoveDown(DataListRelationship relationship)
        //{
        //    PropertyInfo relationshipSource = null;
        //    PropertyInfo relationshipTarget = null;

        //    foreach (PropertyInfo property in this.GetType().GetProperties().Where(p => p.PropertyType == typeof(Guid?) && p.Name.StartsWith("Relationship")).OrderBy(p => p.Name))
        //    {
        //        var val = (Guid?)property.GetValue(this);
        //        if (val == relationship.Id)
        //        {
        //            relationshipSource = property;
        //        }
        //        else
        //        {
        //            if (relationshipSource != null && val != null)
        //            {
        //                relationshipTarget = property;
        //                break;
        //            }
        //        }
        //    }

        //    if (relationshipSource != null && relationshipTarget != null)
        //    {
        //        relationshipSource.SetValue(this, relationshipTarget.GetValue(this));
        //        relationshipTarget.SetValue(this, relationship.Id);
        //    }
        //}

        #region Relationships

        //public IEnumerable<DataListRelationship> AllRelationships
        //{
        //    get
        //    {
        //        if (Relationship1Id != null) yield return Relationship1;
        //        if (Relationship2Id != null) yield return Relationship2;
        //        if (Relationship3Id != null) yield return Relationship3;
        //        if (Relationship4Id != null) yield return Relationship4;
        //        if (Relationship5Id != null) yield return Relationship5;
        //        if (Relationship6Id != null) yield return Relationship6;
        //        if (Relationship7Id != null) yield return Relationship7;
        //        if (Relationship8Id != null) yield return Relationship8;
        //        if (Relationship9Id != null) yield return Relationship9;
        //    }
        //}

        //public Guid? Relationship1Id { set; get; }
        //public virtual DataListRelationship Relationship1 { set; get; }

        //public Guid? Relationship2Id { set; get; }
        //public virtual DataListRelationship Relationship2 { set; get; }

        //public Guid? Relationship3Id { set; get; }
        //public virtual DataListRelationship Relationship3 { set; get; }

        //public Guid? Relationship4Id { set; get; }
        //public virtual DataListRelationship Relationship4 { set; get; }

        //public Guid? Relationship5Id { set; get; }
        //public virtual DataListRelationship Relationship5 { set; get; }

        //public Guid? Relationship6Id { set; get; }
        //public virtual DataListRelationship Relationship6 { set; get; }

        //public Guid? Relationship7Id { set; get; }
        //public virtual DataListRelationship Relationship7 { set; get; }

        //public Guid? Relationship8Id { set; get; }
        //public virtual DataListRelationship Relationship8 { set; get; }

        //public Guid? Relationship9Id { set; get; }
        //public virtual DataListRelationship Relationship9 { set; get; }

        #endregion

        public DataList()
        {
            AllItems = new List<DataListItem>();
        }

        public bool MustBeArchived(DAL.SurveyContext context)
        {
            var itemIds = AllItems.Select(i => i.Id);
            return context.FormValues.Any(v => v.GuidValue.HasValue && itemIds.Contains(v.GuidValue.Value));
        }

        public DataList Clone()
        {
            var list = new DataList
            {
                Name = Name,
                OrganisationId = OrganisationId
            };

            //list.Relationship1 = Relationship1Id.HasValue ? Relationship1.Clone(list) : (DataListRelationship)null;
            //list.Relationship2 = Relationship2Id.HasValue ? Relationship2.Clone(list) : (DataListRelationship)null;
            //list.Relationship3 = Relationship3Id.HasValue ? Relationship3.Clone(list) : (DataListRelationship)null;
            //list.Relationship4 = Relationship4Id.HasValue ? Relationship4.Clone(list) : (DataListRelationship)null;
            //list.Relationship5 = Relationship5Id.HasValue ? Relationship5.Clone(list) : (DataListRelationship)null;
            //list.Relationship6 = Relationship6Id.HasValue ? Relationship6.Clone(list) : (DataListRelationship)null;
            //list.Relationship7 = Relationship7Id.HasValue ? Relationship7.Clone(list) : (DataListRelationship)null;
            //list.Relationship8 = Relationship8Id.HasValue ? Relationship8.Clone(list) : (DataListRelationship)null;
            //list.Relationship9 = Relationship9Id.HasValue ? Relationship9.Clone(list) : (DataListRelationship)null;

            foreach (var item in Relationships)
                list.NotOrderedRelationships.Add(item.Clone(list));

            foreach (var item in Items)
                list.AllItems.Add(item.Clone(list));

            return list;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!this.AllItems.Any())
                yield return new ValidationResult("Data list is empty! Add data items first.", new[] { "dataList" });

            var items = this.AllItems.Select(x => x.Value).ToList();
            if (items.Count != items.Distinct().Count())
                yield return new ValidationResult("data items cannot contain duplicate values.", new[] { "dataList" });
        }

    }
}
