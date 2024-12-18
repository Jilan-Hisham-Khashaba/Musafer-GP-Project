using System.ComponentModel.DataAnnotations;

namespace Gp.Api.Dtos
{
    public class ForgetPasswordDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
    }
}
