using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WDMS.Admin.Models;

namespace WDMS.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var jwtToken = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }


    }
}