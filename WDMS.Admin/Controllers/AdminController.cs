using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WDMS.Application.DTOs;
using WDMS.Application.DTOs.Request;
using WDMS.Applocation.Services;
using WDMS.Infrastructure.Utils;

namespace WDMS.Admin.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly JwtUtils _jwtUtils;

        public AdminController(IAdminService adminService, JwtUtils jwtUtils)
        {
            _adminService = adminService;
            _jwtUtils = jwtUtils;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequest()); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(loginRequest);

                var admin = await _adminService.GetAdminByEmail(loginRequest.Email);
                if (admin == null || !PasswordHelper.VerifyPasswordHash(loginRequest.Password, admin.PasswordHash))
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(loginRequest);
                }

                var token = await _adminService.GenerateJwtTokenAsync(loginRequest.Email, loginRequest.Password);
                if (string.IsNullOrWhiteSpace(token))
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while generating the token.");
                    return View(loginRequest);
                }


                HttpContext.Session.SetString("JwtToken", token);
                HttpContext.Session.SetInt32("AdminId", admin.AdminId);


                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                        new Claim(ClaimTypes.Email, admin.Email ?? string.Empty),
                        new Claim(ClaimTypes.Name,  admin.Email ?? $"admin-{admin.AdminId}"),
                        new Claim(ClaimTypes.Role,  admin.AccessLevel.ToString()),       
                        new Claim("access_level",   admin.AccessLevel.ToString())
                    };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    });

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(loginRequest);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Login");
        }

        [RequireLogin]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var admins = await _adminService.GetAllAdminsAsync();
            return View(admins);
        }


        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpGet]
        public IActionResult Create()
        {

            return View(new AdminRequest());
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            try
            {

                await _adminService.CreateAdminAsync(request);
                TempData["Success"] = "Admin created successfully.";
                return RedirectToAction("List");
            }
            catch (UnauthorizedAccessException ex)
            {

                return Forbid();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "An error occurred.");
                return View(request);
            }
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var admin = await _adminService.GetAdminByIdAsync(id);
                if (admin == null) return NotFound();
                var model = new AdminRequest
                {
                    AdminId = admin.AdminId,
                    Email = admin.Email,
                    AccessLevel = admin.AccessLevel
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminRequest request)
        {

            if (!ModelState.IsValid) return View(request);

            try
            {
                await _adminService.UpdateAdminAsync(request);
                TempData["Success"] = "Admin updated successfully.";
                return RedirectToAction("List");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred.");
                return View(request);
            }
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _adminService.DeleteAdminAsync(id);
                TempData["Success"] = "Admin deleted.";
                return RedirectToAction("List");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the admin.";
                return RedirectToAction("List");
            }
        }
    }
}
