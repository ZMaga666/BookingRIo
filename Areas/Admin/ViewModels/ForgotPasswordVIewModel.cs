using System.ComponentModel.DataAnnotations;

namespace BookingRIo.Areas.Admin.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
