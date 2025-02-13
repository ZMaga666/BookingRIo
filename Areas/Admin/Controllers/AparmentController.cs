using BookingRIo.Data;
using BookingRIo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingRIo.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Controller()]
    public class ApartmentController : Controller
    {
        private readonly AppDbContext _context;
        public ApartmentController(AppDbContext context)
        {
            _context = context;
        }

        /*  // [Area("Admin")]

         public IActionResult Index()
         {
             return View(_context.apartments.ToList());
         }
         [HttpPost]
        public IActionResult Create(Apartment model)
         { if(ModelState.IsValid)
             {
                 _context.apartments.Add(model);
                 _context.SaveChanges();
                 return RedirectToAction("Index");
             }
             ModelState.AddModelError("", "All section is required");
             return View(model);


         }*/

        public async Task<IActionResult> Index()
        {
           var apartments = await _context.apartments.ToListAsync();
            return View(apartments);
        }

        [HttpGet]
        public IActionResult Create()
        
        {
           
            return View(new Apartment());
        }

      
        [HttpPost]
        public IActionResult Create(/*[Bind("RoomNumber,RoomType,Price")]*/ Apartment apartment)
        {  

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all fields";
                return View(apartment); 
            }

            _context.apartments.Add(apartment);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Edit(int id)
        {
            var apartment = await _context.apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            return View(apartment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomNumber,RoomType,Price,Status")] Apartment apartment)
        {
            if (id != apartment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.apartments.Any(e => e.Id == apartment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(apartment);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var apartment = await _context.apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            return View(apartment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.apartments.FindAsync(id);
            _context.apartments.Remove(apartment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
