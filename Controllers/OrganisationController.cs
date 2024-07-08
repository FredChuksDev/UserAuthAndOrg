using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using UserAuthAndOrg.DTOs;
using UserAuthAndOrg.Models;
using UserAuthAndOrg.Services;
using UserAuthAndOrg.Services.Interfaces;

namespace UserAuthAndOrg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/organisations")]
    public class OgranisationController : ControllerBase
    {
        private readonly IOrganisationService organisationService;

        public OgranisationController(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }


        // [GET] /api/organisations
        [HttpGet]
        public async Task<IActionResult> GetAllOrganisations()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        status = "Unauthorized",
                        message = "Authentication required",
                        statusCode = 401
                    });
                }

                var organisations = await organisationService.GetAllOrganisationAsync(userId);

                return Ok(new
                {
                    status = "success",
                    message = "Organisations retrieved successfully",
                    data = organisations.Select(x => new
                    {
                        orgId = x.OrgId,
                        name = x.Name,
                        description = x.Description
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = "Client error",
                    statusCode = 400
                });
            }
        }


        //[GET] /api/organisations/:orgId
        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetOrganisationById(string orgId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        status = "Unauthorized",
                        message = "Authentication required",
                        statusCode = 401
                    });
                }
                var organisation = await organisationService.GetOrganisationByIdAsync(orgId, userId);

                return Ok(new
                {
                    status = "success",
                    message = "Organisation retrieved successfully",
                    data = new
                    {
                        orgId = organisation.OrgId,
                        name = organisation.Name,
                        description = organisation.Description
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = "Client error",
                    statusCode = 400
                });
            }
        }


        //[POST] /api/organisations 
        [HttpPost]
        public async Task<IActionResult> CreateOrganisation([FromBody] OrganisationCreateDto organisationDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        status = "Unauthorized",
                        message = "Authentication required",
                        statusCode = 401
                    });
                }
                var organisation = await organisationService.CreateOrganisationAsync(organisationDto, userId);

                return Created("", new
                {
                    status = "success",
                    message = "Organisation created successfully",
                    data = new
                    {
                        orgId = organisation.OrgId,
                        name = organisation.Name,
                        description = organisation.Description
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = "Client error",
                    statusCode = 400
                });
            }
        }


        // [POST] /api/organisations/:orgId/users
        [HttpPost("{orgId}/users")]
        public async Task<IActionResult> AddUserToOrganisation(string orgId, [FromBody] OrganisationAddUserDto userId)
        {
            try 
            { 
                await organisationService.AddUserToOrganisationAsync(orgId, userId.UserId);

                return Ok(new
                {
                    status = "success",
                    message = "User added to organisation successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = "Client error",
                    statusCode = 400
                });
            }
        }
    }
}
