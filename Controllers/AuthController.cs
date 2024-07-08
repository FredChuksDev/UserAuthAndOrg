using Microsoft.AspNetCore.Mvc;
using UserAuthAndOrg.DTOs;
using UserAuthAndOrg.Models;
using UserAuthAndOrg.Services.Interfaces;

namespace UserAuthAndOrg.Controllers
{

    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }


        // [POST] /auth/register 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(new
                {
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => new { field = e.Exception?.TargetSite?.Name ?? "", message = e.ErrorMessage })
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => new { field = e.Exception?.TargetSite?.Name ?? "Unknown", message = e.ErrorMessage })
                    .ToList();

                return UnprocessableEntity(new { errors });
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone
            };

            try
            {
                var createdUser = await userService.RegisterAsync(user, dto.Password);
                var token = userService.GenerateJwtToken(createdUser);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Registration successful",
                    data = new
                    {
                        accessToken = token.Result,
                        user = new
                        {
                            userId = createdUser.UserId,
                            firstName = createdUser.FirstName,
                            lastName = createdUser.LastName,
                            email = createdUser.Email,
                            phone = createdUser.Phone
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = "Registration unsuccessful",
                    statusCode = 400
                });
            }
        }




        //[POST] /auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = "Authentication failed",
                    statusCode = 401
                });
            }

            var user = await userService.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = "Authentication failed",
                    statusCode = 401
                });
            }

            var token = userService.GenerateJwtToken(user);
            return Ok(new
            {
                status = "success",
                message = "Login successful",
                data = new
                {
                    accessToken = token.Result,
                    user = new
                    {
                        userId = user.UserId,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        phone = user.Phone
                    }
                }
            });
        }
    }
}
