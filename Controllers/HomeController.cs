using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingRIo.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingRIo.Controllers
{   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            // Kullanıcı bilgilerini al
            var userName = User.Identity.Name;  // Kullanıcı adı
            var userRole = User.IsInRole("Admin") ? "Admin" : (User.IsInRole("Moderator") ? "Moderator" : "User");  // Kullanıcı rolü

            // Kullanıcı adını ve rolünü View'a göndermek
            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
