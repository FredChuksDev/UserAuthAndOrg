using Microsoft.EntityFrameworkCore;
using UserAuthAndOrg.Models;

namespace UserAuthAndOrg.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<UserOrganisation> UserOrganisations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserOrganisation>()
               .HasKey(uo => new { uo.UserId, uo.OrgId });

            modelBuilder.Entity<UserOrganisation>()
                .HasOne(uo => uo.User)
                .WithMany(u => u.UserOrganisations)
                .HasForeignKey(uo => uo.UserId);

            modelBuilder.Entity<UserOrganisation>()
                .HasOne(uo => uo.Organisation)
                .WithMany(o => o.UserOrganisations)
                .HasForeignKey(uo => uo.OrgId);
        }
    }
}
