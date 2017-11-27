using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LightMethods.Survey.Models.DAL;
using System.Data.Entity;

namespace LightMethods.Survey.Models.Entities
{
    public class ReportList : ReportItem
    {
        [Required]
        public Guid DataTypeId { set; get; }

        [Display(Name = "Data")]
        public virtual ReportListDataType DataType { set; get; }

        public IEnumerable<Commentary> GetCommetaries(Project CurrentProject, DateTime sDate, DateTime eDate)
        {

            using (var uow = new UnitOfWork(new SurveyContext()))
            {
                return uow.CommentariesRepository
                    .All
                    .Where(c => c.ProjectId == CurrentProject.Id)
                    .Where(c => c.Date <= eDate && c.Date >= sDate)
                    .AsNoTracking()
                    .ToList();
            }
        }
    }
}
