using System.ComponentModel.DataAnnotations;

namespace Gp.Api.Dtos
{
    public class ChangePasswordDto
    {

        [Required]
        public string Email { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }

     

      
        public string? DisplayName { get; set; }

    }
}
