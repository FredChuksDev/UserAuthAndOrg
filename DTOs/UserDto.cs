using System.ComponentModel.DataAnnotations;

namespace UserAuthAndOrg.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? Phone { get; set; }
    }

    public class UserLoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
