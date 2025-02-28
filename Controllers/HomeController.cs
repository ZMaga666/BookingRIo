using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingRIo.Models;
using Microsoft.AspNetCore.Identity;
using BookingRIo.Data;

namespace BookingRIo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, AppDbContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var userName = User.Identity.Name;
            var userRole = User.IsInRole("Admin") ? "Admin" : (User.IsInRole("Moderator") ? "Moderator" : "User");

            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;

            // Apartmanları getir
            var apartments = _dbContext.apartments.ToList();
            return View(apartments);
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
