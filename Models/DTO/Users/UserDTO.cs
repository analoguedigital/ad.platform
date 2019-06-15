using System;

namespace LightMethods.Survey.Models.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }

        public int AccountType { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime? LastLogin { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Address { get; set; }

        public bool IsActive { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public byte SecurityQuestion { get; set; }

        public string SecurityAnswer { get; set; }
    }
}
