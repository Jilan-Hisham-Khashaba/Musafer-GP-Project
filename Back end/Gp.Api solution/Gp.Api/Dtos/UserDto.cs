using System.ComponentModel.DataAnnotations;

namespace GP.APIs.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public string? Token { get; set; }

        public string Password { get; set; }


        public string? UserName { get; set; }
        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }

        public string? verificationCode {  get; set; }

        public string? photo {  get; set; }

        public DateTime? lastLogin { get; set; }= DateTime.Now;


    }
}
