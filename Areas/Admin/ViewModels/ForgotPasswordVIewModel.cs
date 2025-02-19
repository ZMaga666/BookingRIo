using System.ComponentModel.DataAnnotations;

namespace BookingRIo.Areas.Admin.ViewModels
{
    public class ForgotPasswordVIewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
