using Microsoft.EntityFrameworkCore;
using UserAuthAndOrg.Data;
using UserAuthAndOrg.DTOs;
using UserAuthAndOrg.Models;
using UserAuthAndOrg.Services.Interfaces;

namespace UserAuthAndOrg.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public OrganisationService(AppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        public async Task<Organisation> CreateOrganisationAsync(OrganisationCreateDto organisationDto, string userId)
        {
            var organisation = new Organisation
            {
                OrgId = Guid.NewGuid().ToString(),
                Name = organisationDto.Name,
                Description = organisationDto.Description
            };

            var userOrganisation = new UserOrganisation
            {
                UserId = userId,
                OrgId = organisation.OrgId
            };

            context.Organisations.Add(organisation);
            context.UserOrganisations.Add(userOrganisation);
            await context.SaveChangesAsync();

            return organisation;
        }

        public async Task AddUserToOrganisationAsync(string orgId, string userId)
        {
            var userOrganisation = new UserOrganisation
            {
                UserId = userId,
                OrgId = orgId
            };

            context.UserOrganisations.Add(userOrganisation);
            await context.SaveChangesAsync();
        }

        public async Task<List<Organisation>> GetAllOrganisationAsync(string userId)
        {
            var organisations = await context.UserOrganisations
                .Where(uo => uo.UserId == userId)
                .Select(uo => uo.Organisation)
                .ToListAsync();

            return organisations;
        }

        public async Task<Organisation> GetOrganisationByIdAsync(string orgId, string userId)
        {
            var organisation = await context.UserOrganisations
                .Where(uo => uo.OrgId == orgId && uo.UserId == userId)
                .Select(uo => uo.Organisation)
                .FirstOrDefaultAsync();

            if (organisation == null)
            {
                throw new Exception("Organisation not found or you do not have access to it.");
            }

            return organisation;
        }
    }
}
