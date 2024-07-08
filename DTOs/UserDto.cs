using System.ComponentModel.DataAnnotations;

namespace UserAuthAndOrg.DTOs
{
    public class UserRegisterDto
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Phone { get; set; }
    }

    public class UserLoginDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
