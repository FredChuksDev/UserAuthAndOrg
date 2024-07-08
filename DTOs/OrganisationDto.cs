using System.ComponentModel.DataAnnotations;

namespace UserAuthAndOrg.DTOs
{
    public class OrganisationCreateDto
    {
        public string Name { get; set; }

        public string? Description { get; set; }
    }

    public class OrganisationAddUserDto
    {
        public string UserId { get; set; }
    }
}
