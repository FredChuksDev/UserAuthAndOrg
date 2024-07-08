using System.ComponentModel.DataAnnotations;

namespace UserAuthAndOrg.Models
{
    public class Organisation
    {
        [Key]
        public string OrgId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public List<UserOrganisation> UserOrganisations { get; set; }
    }
}
