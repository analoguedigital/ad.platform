using System;

namespace LightMethods.Survey.Models.Entities
{
    public interface ICaseItem
    {
        Guid ProjectId { get; set; }

        Project Project { get; set; }
    }
}
