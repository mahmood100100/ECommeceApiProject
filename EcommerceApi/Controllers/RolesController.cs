using AutoMapper;
using Ecommerce.Core.Entities.DTO;
using Ecommerce.Core.Entities;
using Ecommerce.Core.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class RolesController : ControllerBase
    {
        private readonly IRolesRepository rolesRepository;
        private readonly IMapper mapper;
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(IRolesRepository rolesRepository, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            this.rolesRepository = rolesRepository;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllRoles()
        {
            try
            {
                var roles = await rolesRepository.GetAllRoles();
                var rolesDto = mapper.Map<IEnumerable<RolesResponseDTO>>(roles);

                return Ok(new ApiResponse(200, Result: rolesDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>()
        {
            $"Error retrieving roles: {ex.Message}"
        }, StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest(new ApiResponse(400, "Role name cannot be empty"));
            }

            try
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (roleExists)
                {
                    return BadRequest(new ApiResponse(400, "Role already exists"));
                }

                await rolesRepository.AddRole(roleName);

                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "Failed to retrieve the created role"));
                }

                var roleDto = mapper.Map<RolesResponseDTO>(role);

                return Ok(new ApiResponse(201, "Role created successfully", roleDto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>()
        {
            $"Error creating role: {ex.Message}"
        }, StatusCodes.Status500InternalServerError));
            }

        }

        [HttpDelete("{roleName}")]
        public async Task<ActionResult<ApiResponse>> RemoveRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest(new ApiResponse(400, "Role name cannot be empty"));
            }

            try
            {
                await rolesRepository.RemoveRole(roleName);
                return Ok(new ApiResponse(200, "Role removed successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>()
        {
            $"Error removing role: {ex.Message}"
        }, StatusCodes.Status500InternalServerError));
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateRole(string id, [FromBody] string newRoleName)
        {
            if (string.IsNullOrEmpty(newRoleName))
            {
                return BadRequest(new ApiResponse(400, "New role name cannot be empty"));
            }

            try
            {
                await rolesRepository.UpdateRole(id, newRoleName);
                var updatedRole = await roleManager.FindByIdAsync(id);
                var roleDto = mapper.Map<RolesResponseDTO>(updatedRole);

                return Ok(new ApiResponse(200, "Role updated successfully", roleDto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>()
        {
            $"Error updating role: {ex.Message}"
        }, StatusCodes.Status500InternalServerError));
            }
        }
    }
}
