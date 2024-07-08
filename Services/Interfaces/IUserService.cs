using UserAuthAndOrg.Models;

namespace UserAuthAndOrg.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(User user, string password);
        Task<User> AuthenticateAsync(string email, string password);
        Task<string> GenerateJwtToken(User user);
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserInSameOrganisationAsync(string id, string userId);
    }
}
