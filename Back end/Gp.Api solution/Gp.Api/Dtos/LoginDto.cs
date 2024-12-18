using System.ComponentModel.DataAnnotations;

namespace GP.APIs.Dtos
{
    public class LoginDto
    {
        [Required] //Email feild is req
        [EmailAddress]
        public string Email { get; set; }
     
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        public string? Token { get; set; }
    }
}
