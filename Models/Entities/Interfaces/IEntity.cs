using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public interface IEntity
    {
        [Key]
        Guid Id { get; set; }
    }
}
