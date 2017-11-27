using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public class DataListRelationship : Entities.Entity
    {
        [Index]
        public Guid OwnerId { set; get; }
        public virtual DataList Owner { set; get; }

        [Display(Name = "Type")]
        public virtual DataList DataList { set; get; }
        public Guid DataListId { set; get; }

        [Required]
        public string Name { set; get; }

        public int Order { set; get; }


        public DataListRelationship Clone(DataList owner)
        {
            return new DataListRelationship
            {
                DataListId = DataListId,
                Name = Name,
                Owner = owner,
                Order = Order
            };
        }

    }
}
