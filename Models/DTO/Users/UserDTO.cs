using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LightMethods.Survey.Models.Entities.User;

namespace LightMethods.Survey.Models.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public DateTime? LastLogin { get; set; }

        public GenderType? Gender { get; set; }

        public DateTime? Birthdate { get; set; }

        public string Address { get; set; }

        public bool IsActive { get; set; }
    }
}
