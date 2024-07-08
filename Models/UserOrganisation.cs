namespace UserAuthAndOrg.Models
{
    public class UserOrganisation
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string OrgId { get; set; }
        public Organisation Organisation { get; set; }
    }
}
