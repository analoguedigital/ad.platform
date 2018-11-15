using LightMethods.Survey.Models.DAL;
using System;

namespace LightMethods.Survey.Models.Entities
{
    public interface IArchivable
    {
        DateTime? DateArchived { set; get; }

        bool MustBeArchived(SurveyContext context);
    }
}
