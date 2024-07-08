using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAuthAndOrg.Services.Interfaces;

namespace UserAuthAndOrg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IOrganisationService organisationService;


        public UserController(IUserService userService, IOrganisationService organisationService)
        {
            this.userService = userService;
            this.organisationService = organisationService;
        }


        // [GET] /api/users/:id 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = new Models.User();
                if (id == "me")
                {
                    user = await userService.GetUserByIdAsync(currentUserId);
                }
                else
                {
                    user = await userService.GetUserInSameOrganisationAsync(currentUserId, id);
                }

                return Ok(new
                {
                    status = "success",
                    message = "Retrieved User Successfully",
                    data = new
                    {
                        userId = user.UserId,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        phone = user.Phone
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = ex.Message,
                    statusCode = 400
                });
            }
        }
    }
}
