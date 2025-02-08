using BookingRIo.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingRIo.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>opt):base(opt)
        {
            
        }

        public DbSet<Apartment> apartments { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
