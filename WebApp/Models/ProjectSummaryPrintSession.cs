using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public class ProjectSummaryPrintSessionDTO
    {
        public Guid Id { set; get; }

        public Guid ProjectId { set; get; }

        public List<Guid> SurveyIds { set; get; } = new List<Guid>();

        public List<string> RemovedItemIds { set; get; } = new List<string>();
    }
}