using BookingRIo.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingRIo.Areas.Admin.ViewModels
{
    public class UserRoleVM
    {

        public User User { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
