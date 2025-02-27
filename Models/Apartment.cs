using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace BookingRIo.Models
{
    public class Apartment
    {
        public int Id { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public double Amount { get; set; }
        public string? Status { get; set; } 
        public string? ImagePath { get; set; }

        [NotMapped]
        public  IFormFile? ImageFile { get; set; }

    }

}
