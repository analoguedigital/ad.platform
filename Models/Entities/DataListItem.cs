using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace LightMethods.Survey.Models.Entities
{
    public class DataListItem : Entity, IArchivable
    {
        [Index]
        public Guid DataListId { set; get; }

        public virtual DataList DataList { set; get; }

        [MaxLength(256)]
        public string Text { set; get; }

        public string Description { set; get; }

        [Required]
        public int Value { set; get; }

        public int Order { set; get; }

        public DateTime? DateArchived { get; set; }

        public bool MustBeArchived(SurveyContext context)
        {
            return context.FormValues.Any(v => v.GuidValue == this.Id || v.RowDataListItemId == this.Id);
        }

        public virtual ICollection<DataListItemAttr> Attributes { set; get; }

        public DataListItem GetAttrValue(Guid relationshipId)
        {
            return Attributes.SingleOrDefault(attr => attr.RelationshipId == relationshipId).Value;
        }

        public DataListItem Clone(DataList dataList)
        {
            return new DataListItem
            {
                DataList = dataList,
                Description = Description,
                Text = Text,
                Value = Value,
                Order = Order,
                // TODO: ATTRibues
                DateArchived = DateArchived.HasValue ? DateTimeService.UtcNow : (DateTime?)null
            };

        }
    }

    public class DataListItemAttr : Entity
    {
        [Index]
        public Guid OwnerId { set; get; }

        public virtual DataListItem Owner { set; get; }

        public Guid RelationshipId { set; get; }

        public virtual DataListRelationship Relationship { set; get; }

        public Guid ValueId { set; get; }

        public virtual DataListItem Value { set; get; }

    }

}
