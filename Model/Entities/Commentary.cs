using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LightMethods.Survey.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;
using LightMethods.Survey.Models.Services;

namespace LightMethods.Survey.Models.Entities
{
    public class Commentary : Entity
    {
        public Project Project { set; get; }
        public Guid ProjectId { set; get; }

        [Required]
        [StringLength(150)]
        [Display(Name="Short title")]
        public string ShortTitle { set; get; }

        [DataType(DataType.MultilineText)]
        public string Description { set; get; }
        
        [Display(Name="Date")]
        public DateTime Date { set; get; }

        [Display(Name="Severity level")]
        public SeverityLevel SeverityLevel { set; get; }
        public Guid? SeverityLevelId { set; get; }

        public virtual ICollection<CommentaryDocument> Documents { set; get; }

        [NotMapped]
        public IEnumerable<DocumentDTO> DocumentsDTO { set; get; }

        public Commentary()
        {
            Date = DateTimeService.UtcNow;
            DateCreated = DateTimeService.UtcNow;
        }
    }
}