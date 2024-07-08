using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthAndOrg.Data;
using UserAuthAndOrg.Models;
using UserAuthAndOrg.Services.Interfaces;

namespace UserAuthAndOrg.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public UserService(AppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            if (await context.Users.AnyAsync(u => u.Email == user.Email))
                throw new Exception("Email already exists");

            user.Password = BCrypt.Net.BCrypt.HashPassword(password);

            var organisation = new Organisation
            {
                Name = $"{user.FirstName}'s Organisation"
            };

            var userOrganisation = new UserOrganisation
            {
                User = user,
                Organisation = organisation
            };

            user.UserOrganisations = new List<UserOrganisation> { userOrganisation };
            organisation.UserOrganisations = new List<UserOrganisation> { userOrganisation };

            await context.Users.AddAsync(user);
            await context.Organisations.AddAsync(organisation);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Invalid login credentials");

            return user;
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddMinutes(Convert.ToInt32(configuration["Jwt:ExpiryMinutes"]));

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) { return null; }
            return user;
        }


        public async Task<User> GetUserInSameOrganisationAsync(string currentUserId, string targetUserId)
        {
            var currentUserOrganisations = await context.UserOrganisations
                .Where(uo => uo.UserId == currentUserId)
                .Select(uo => uo.OrgId)
                .ToListAsync();

            var targetUserOrganisations = await context.UserOrganisations
                .Where(uo => uo.UserId == targetUserId)
                .Select(uo => uo.OrgId)
                .ToListAsync();

            if (!currentUserOrganisations.Intersect(targetUserOrganisations).Any())
            {
                throw new Exception("You do not have access to this user's details.");
            }

            return await GetUserByIdAsync(targetUserId);
        }

    }

}
