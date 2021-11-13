using System.ComponentModel.DataAnnotations;

namespace K1.JwtSample.Models.Dto.Requests
{
    public class UserRegistrationDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}