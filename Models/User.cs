using System.ComponentModel.DataAnnotations;

namespace UserAuthAndOrg.Models
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? Phone { get; set; }
        public List<UserOrganisation> UserOrganisations { get; set; }
    }
}
