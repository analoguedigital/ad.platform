using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LightMethods.Survey.Models.Entities
{
    public interface IEntity
    {
        [Key]
        Guid Id { get; set; }
    }
}
