using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WDMS.Application.DTOs;
using WDMS.Application.DTOs.Request;
using WDMS.Infrastructure.Services;
using WDMS.Infrastructure.Utils;

namespace WDMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly JwtUtils _jwtUtils;

        public AdminController(IAdminService adminService, JwtUtils jwtUtils)
        {
            _adminService = adminService;
            _jwtUtils = jwtUtils;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var token = await _adminService.GenerateJwtTokenAsync(loginRequest.Email, loginRequest.Password);

                if (token == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminResponse>>> GetAdmins()
        {
            try
            {
                var admins = await _adminService.GetAllAdminsAsync();

                if (admins == null || !admins.Any())
                {
                    return NotFound(new { message = "No admins found." });
                }

                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving admins.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminResponse>> GetAdminById(int id)
        {
            try
            {
                var admin = await _adminService.GetAdminByIdAsync(id);

                if (admin == null)
                {
                    return NotFound(new { message = $"Admin with ID {id} not found." });
                }

                return Ok(admin);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "An error occurred while retrieving the admin.",
                        error = ex.Message
                    });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAdmin(AdminRequest adminRequest)
        {
            try
            {
                if (adminRequest == null)
                {
                    return BadRequest(new { message = "Admin request body is null." });
                }

                var createdAdmin = await _adminService.CreateAdminAsync(adminRequest);
                return CreatedAtAction(nameof(GetAdminById), new { id = createdAdmin.AdminId }, createdAdmin);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "An error occurred while creating the admin.",
                        error = ex.Message
                    });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAdmin(int id, AdminRequest adminRequest)
        {
            try
            {
                if (id != adminRequest.AdminId)
                {
                    return BadRequest(new { message = "Admin ID in the request does not match the provided ID." });
                }

                var updated = await _adminService.UpdateAdminAsync(adminRequest);
                if (!updated)
                {
                    return NotFound(new { message = $"Admin with ID {id} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "An error occurred while updating the admin.",
                        error = ex.Message
                    });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdmin(int id)
        {
            try
            {
                var deleted = await _adminService.DeleteAdminAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Admin with ID {id} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "An error occurred while deleting the admin.",
                        error = ex.Message
                    });
            }
        }
    }
}