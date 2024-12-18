using System.ComponentModel.DataAnnotations;

namespace Gp.Api.Dtos
{
    public class ResetPasswordDto
    {
     
        public string? Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Token { get; set; }
    }
}
