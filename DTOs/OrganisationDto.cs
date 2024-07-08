using System.ComponentModel.DataAnnotations;

namespace UserAuthAndOrg.DTOs
{
    public class OrganisationCreateDto
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
    }

    public class OrganisationAddUserDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
