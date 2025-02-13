namespace BookingRIo.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; } 
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string GuestName { get; set; }
        public string GuestEmail { get; set; }
        public string TotalAmount { get; set; }

        public Apartment Apartment { get; set; } 
    }

}
