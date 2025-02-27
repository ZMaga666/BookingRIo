using Microsoft.AspNetCore.Identity;

namespace BookingRIo.Models
    {
        public class User:IdentityUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            //  public string PhotoUrl { get; set; }  
            public string ConfirmationToken { get; set; }

        }
}
