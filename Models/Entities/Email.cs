using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.Entities
{
    public class Email : Entity
    {
        [Required]
        [StringLength(150)]
        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        [Required]
        [StringLength(150)]
        public string Subject { get; set; }

        [Required]
        public string Content { get; set; }

        public bool IsSent { get; set; }
    }
}
