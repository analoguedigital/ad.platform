using System;

namespace WebApi.Models
{
    public class ChangePasswordDTO
    {
        public Guid UserId { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}