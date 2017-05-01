using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightMethods.Survey.Models.Entities
{
    public abstract class Entity : IEntity, IValidatableObject
    {

        [Key]//,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        //[ReadOnly(true)]
        //[ScaffoldColumn(false)]
        //public bool IsNew { get { return Id == Guid.Empty; } }

        [ReadOnly(true)]
        [ScaffoldColumn(false)]

        [Display(Name = "Date Created")]
        public virtual DateTime DateCreated { get; set; }

        [ReadOnly(true)]
        [ScaffoldColumn(false)]

        public DateTime DateUpdated { get; set; }

        public Entity()
        {
            Id = Guid.Empty;

            DateCreated = DateUpdated = DateTime.Now;
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is Entity)
                return this.Id == ((Entity)obj).Id;
            return false;
        }

        public static bool operator ==(Entity a, Entity b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Id == b.Id;
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}