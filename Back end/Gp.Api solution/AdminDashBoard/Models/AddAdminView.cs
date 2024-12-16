using System.ComponentModel.DataAnnotations;

namespace AdminDashBoard.Models
{
    public class AddAdminView
    {
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        public string DisplayName { get; set; }

        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public int? NationalId { get; set; }

        public string? Id { get; set; }

        public string? AddressCountry { get; set; }


        public string? City { get; set; }
        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public string? ProfilePicture { get; set; }
    }
}
