using LightMethods.Survey.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public interface IArchivable
    {
        DateTime? DateArchived { set; get; }

        bool MustBeArchived(SurveyContext context);
    }
}
