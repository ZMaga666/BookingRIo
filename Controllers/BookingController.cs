using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BookingRIo.Models;
using BookingRIo.Data;

public class BookingController : Controller
{
    private readonly BookingRIo.Data.AppDbContext _context;

    public BookingController(   AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bookings = await _context.bookings.Include(b => b.Apartment).ToListAsync();
        return View(bookings);
    }

    public IActionResult Create()
    {
        ViewBag.Apartments = _context.apartments.ToList();
        return View();
    }

    // AJAX: 
    [HttpGet]
    public async Task<JsonResult> CheckAvailability(int apartmentId)
    {
        var bookedDates = await _context.bookings
            .Where(b => b.ApartmentId == apartmentId)
            .Select(b => new { CheckIn = b.CheckInDate, CheckOut = b.CheckOutDate })
            .ToListAsync();

        return Json(bookedDates);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        ViewBag.Apartments = _context.apartments.ToList();

        if (ModelState.IsValid)
        {
            var overlappingBookings = await _context.bookings
                .Where(b => b.ApartmentId == booking.ApartmentId &&
                            ((booking.CheckInDate >= b.CheckInDate && booking.CheckInDate < b.CheckOutDate) ||
                             (booking.CheckOutDate > b.CheckInDate && booking.CheckOutDate <= b.CheckOutDate)))
                .ToListAsync();

            if (overlappingBookings.Any())
            {
                ModelState.AddModelError("", "This room is not available for these dates.");
                ViewBag.Apartments = _context.apartments.ToList();
                return View(booking);
            }

            var apartment = await _context.apartments.FindAsync(booking.ApartmentId);
            if (apartment == null)
            {
                return NotFound();
            }

            booking.TotalAmount = (booking.CheckOutDate - booking.CheckInDate).Days * apartment.Amount;

            _context.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(booking);
    }
}
