using System;

namespace WebApi.Models
{
    public class ChangePhoneNumberModel
    {
        public string PhoneNumber { get; set; }
    }

    public class VerifyChangedNumberModel
    {
        public string PhoneNumber { get; set; }

        public string Code { get; set; }
    }

    public class VerifyPhoneNumberModel
    {
        public string PhoneNumber { get; set; }

        public string Code { get; set; }
    }

    public class AddPhoneNumberModel
    {
        public string PhoneNumber { get; set; }
    }

    public class SendEmailConfirmationModel
    {
        public string Email { get; set; }
    }

    public class ConfirmEmailModel
    {
        public Guid UserId { get; set; }

        public string Code { get; set; }
    }

    public class CreateSuperUserDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }
    }

}