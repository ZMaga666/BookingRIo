using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingRIo.Areas.Admin.Controllers
{
    [Area("Admin")]
 //  [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();

            return View(roles);
        }

        public IActionResult CreateRole ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var findRole = await _roleManager.FindByNameAsync(roleName);
            if (findRole != null)
            {
                return View();
            }

            IdentityRole newRole = new()
            {
                Name = roleName
            };
            await _roleManager.CreateAsync(newRole);
            return RedirectToAction("Index");
        }
    }
}



