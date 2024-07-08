using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using UserAuthAndOrg.Data;
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
        private readonly AppDbContext contextd;

        public AuthController(IUserService userService, AppDbContext contextd)
        {
            this.userService = userService;
            this.contextd = contextd;
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

            // Validate the model
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(dto, serviceProvider: null, items: null);
            bool isValid = Validator.TryValidateObject(dto, context, validationResults, true);

            if (string.IsNullOrEmpty(dto.FirstName))
            {
                validationResults.Add(new ValidationResult("FirstName must not be null", new[] { "FirstName" }));
            }
            if (string.IsNullOrEmpty(dto.LastName))
            {
                validationResults.Add(new ValidationResult("LastName must not be null", new[] { "LastName" }));
            }
            if (string.IsNullOrEmpty(dto.Email))
            {
                validationResults.Add(new ValidationResult("Email must not be null.", new[] { "Email" }));
            }
            if (contextd.Users.Any(u => u.Email == dto.Email))
            {
                validationResults.Add(new ValidationResult("Email must be unique.", new[] { "Email" }));
            }
            if (string.IsNullOrEmpty(dto.Password))
            {
                validationResults.Add(new ValidationResult("Password must not be null.", new[] { "Password" }));
            }
            if (!isValid || validationResults.Count > 0)
            {
                var errors = validationResults.Select(result => new
                {
                    field = result.MemberNames.FirstOrDefault(),
                    message = result.ErrorMessage
                });
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
                return UnprocessableEntity(new
                {
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => new { field = e.Exception?.TargetSite?.Name ?? "", message = e.ErrorMessage })
                });
            }

            // Validate the model
            var validationResults = new List<ValidationResult>();
            if (string.IsNullOrEmpty(dto.Email))
            {
                validationResults.Add(new ValidationResult("Email must not be null.", new[] { "Email" }));
            }
            if (string.IsNullOrEmpty(dto.Password))
            {
                validationResults.Add(new ValidationResult("Password must not be null.", new[] { "Password" }));
            }
            if (validationResults.Count > 0)
            {
                var errors = validationResults.Select(result => new
                {
                    field = result.MemberNames.FirstOrDefault(),
                    message = result.ErrorMessage
                });
                return UnprocessableEntity(new { errors });
            }

            try
            {
                var user = await userService.AuthenticateAsync(dto.Email, dto.Password);
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
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = "Authentication failed",
                    statusCode = 401
                });
            }
        }
    }
}
