using GP.core.Entities.identity;
using System.ComponentModel.DataAnnotations;

namespace GP.APIs.Dtos
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }


        public string? ProfilePicture {  get; set; }


        //public Address Address { get; set; }


        public string Password { get; set; }
    }
}
