using UserAuthAndOrg.DTOs;
using UserAuthAndOrg.Models;

namespace UserAuthAndOrg.Services.Interfaces
{
    public interface IOrganisationService
    {
        Task<List<Organisation>> GetAllOrganisationAsync(string userId);
        Task<Organisation> GetOrganisationByIdAsync(string orgId, string userId);
        Task<Organisation> CreateOrganisationAsync(OrganisationCreateDto org, string userId);
        Task AddUserToOrganisationAsync(string orgId, string userId);
    }
}
