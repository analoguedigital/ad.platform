using System;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.DTO
{
    public class CreateOrganisation
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Root user email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(30)]
        public string RootUserEmail { get; set; }

        [Display(Name = "First Name")]
        public string RootUserFirstName { get; set; }

        [Display(Name = "Surname")]
        public string RootUserSurname { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Root password")]
        [MaxLength(30)]
        public string RootPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("RootPassword", ErrorMessage = "The root password and confirmation password do not match.")]
        [Display(Name = "Confirm root password")]
        public string RootConfirmPassword { get; set; }

        [StringLength(30)]
        [Display(Name = "Telephone Number")]
        public string TelNumber { get; set; }

        [StringLength(30)]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [StringLength(30)]
        [Display(Name = "Address Line 1")]
        public string AddressLine2 { get; set; }

        [StringLength(30)]
        public string Town { get; set; }

        [StringLength(30)]
        public string County { get; set; }

        [StringLength(7)]
        public string Postcode { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(150)]
        public string Website { get; set; }

        [StringLength(150)]
        public string LogoUrl { get; set; }

        public string TermsAndConditions { get; set; }

        public bool RequiresAgreement { get; set; }

        // social profiles
        [StringLength(50)]
        public string FacebookId { get; set; }

        [StringLength(50)]
        public string TwitterId { get; set; }

        [StringLength(50)]
        public string InstagramId { get; set; }

        [StringLength(50)]
        public string SkypeId { get; set; }

        [StringLength(100)]
        public string LinkedinUrl { get; set; }

        [StringLength(100)]
        public string YouTubeUrl { get; set; }

        [Display(Name="Default language")]
        public Guid DefaultLanguageId { set; get; }

        [Display(Name="Default calendar")]
        public Guid DefaultCalendarId { set; get; }
    }
}