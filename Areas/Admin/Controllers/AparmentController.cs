using BookingRIo.Data;
using BookingRIo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity;

namespace BookingRIo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Moderator")]
    [Controller()]
    public class ApartmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ApartmentController(AppDbContext context,IWebHostEnvironment hostEnvironment )
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }


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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Apartment apartment)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all fields";
                return View(apartment); // Eğer model geçersizse tekrar formu göster
            }

            if (apartment.ImageFile != null && apartment.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploadsFolder); // Klasör yoksa oluştur

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(apartment.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await apartment.ImageFile.CopyToAsync(fileStream); // Async dosya yükleme
                }

                apartment.ImagePath = "/images/" + uniqueFileName; // Veritabanına kaydedilecek URL
            }

            _context.apartments.Add(apartment);
            await _context.SaveChangesAsync(); // Async kaydetme işlemi
            return RedirectToAction("Index"); // Başarılı ekleme sonrası yönlendirme
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomNumber,RoomType,Amount,Status")] Apartment apartment, IFormFile ImageFile)
        {
            if (id != apartment.Id)
            {
                return Json(new { success = false, message = "Invalid apartment ID." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingApartment = await _context.apartments.FindAsync(id);
                    if (existingApartment == null)
                    {
                        return Json(new { success = false, message = "Apartment not found." });
                    }

                    // Güncellenen verileri mevcut kayda uygula
                    existingApartment.RoomNumber = apartment.RoomNumber;
                    existingApartment.RoomType = apartment.RoomType;
                    existingApartment.Amount = apartment.Amount;
                    existingApartment.Status = apartment.Status;

                    // Eğer yeni bir resim yüklenmişse
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Eski resmi sil (Eğer varsa)
                        if (!string.IsNullOrEmpty(existingApartment.ImagePath))
                        {
                            string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, existingApartment.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Yeni resmin adını oluştur
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        // Yeni resmin yolunu kaydet
                        existingApartment.ImagePath = "/uploads/" + uniqueFileName;
                    }

                    _context.Update(existingApartment);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Apartment updated successfully!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { success = false, message = "Error updating apartment." });
                }
            }

            return Json(new { success = false, message = "Invalid data. Please check the inputs." });
        }


        public async Task<IActionResult> Delete(int id)
        {
            var apartment = await _context.apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }

            _context.apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Apartment deleted successfully!";

            return Ok();
        }




    }
}
