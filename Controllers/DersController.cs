using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class DersController : Controller
    {
        private readonly VeriTabaniContext _context;
        private readonly ILogService _logService;

        public DersController(VeriTabaniContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // Ders listesi
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                IQueryable<Ders> dersler = _context.Dersler;

                // Arama filtresi
                if (!string.IsNullOrEmpty(searchString))
                {
                    dersler = dersler.Where(d =>
                        d.DersAdi.Contains(searchString) ||
                        d.DersKodu.Contains(searchString));
                }

                return View(await dersler.ToListAsync());
            }
            catch (Exception ex)
            {
                // Hata log kaydı
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Ders listesi görüntüleme hatası: {ex.Message}");

                return View("Error");
            }
        }

        // Ders detayı
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var ders = await _context.Dersler
                    .Include(d => d.Kayitlar)
                    .ThenInclude(k => k.Ogrenci)
                    .FirstOrDefaultAsync(d => d.DersId == id);

                if (ders == null)
                {
                    return NotFound();
                }

                return View(ders);
            }
            catch (Exception ex)
            {
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Ders detay görüntüleme hatası: {ex.Message}");

                return View("Error");
            }
        }

        // Ders oluşturma sayfası
        [HttpGet]
        [Authorize(Roles = "Admin,Ogretmen")] // Sadece admin ve öğretmenler erişebilir
        public IActionResult Create()
        {
            return View();
        }

        // Ders oluşturma işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Create([Bind("DersKodu,DersAdi,Kredi,AktifMi")] Ders ders)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Ders kodu zaten var mı kontrol et
                    if (await _context.Dersler.AnyAsync(d => d.DersKodu == ders.DersKodu))
                    {
                        ModelState.AddModelError("DersKodu", "Bu ders kodu zaten kullanılıyor.");
                        return View(ders);
                    }

                    _context.Add(ders);
                    await _context.SaveChangesAsync();

                    // Log kaydı
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int kullaniciId))
                    {
                        await _logService.LogKaydiEkleAsync(
                            kullaniciId,
                            "Ders Ekleme",
                            $"{ders.DersKodu} kodlu ders eklendi.");
                    }
                    else
                    {
                        await _logService.LogKaydiEkleAsync(
                            null,
                            "Ders Ekleme",
                            $"{ders.DersKodu} kodlu ders eklendi.");
                    }

                    return RedirectToAction(nameof(Index));
                }
                return View(ders);
            }
            catch (Exception ex)
            {
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Ders ekleme hatası: {ex.Message}");

                ModelState.AddModelError("", "Ders eklenirken bir hata oluştu.");
                return View(ders);
            }
        }

        // Ders düzenleme sayfası
        [HttpGet]
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ders = await _context.Dersler.FindAsync(id);
            if (ders == null)
            {
                return NotFound();
            }

            return View(ders);
        }

        // Ders düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Edit(int id, [Bind("DersId,DersKodu,DersAdi,Kredi,AktifMi")] Ders ders)
        {
            if (id != ders.DersId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Ders kodu başka bir derste var mı kontrol et
                    if (await _context.Dersler.AnyAsync(d =>
                        d.DersKodu == ders.DersKodu &&
                        d.DersId != ders.DersId))
                    {
                        ModelState.AddModelError("DersKodu", "Bu ders kodu zaten kullanılıyor.");
                        return View(ders);
                    }

                    _context.Update(ders);
                    await _context.SaveChangesAsync();

                    // Log kaydı
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int kullaniciId))
                    {
                        await _logService.LogKaydiEkleAsync(
                            kullaniciId,
                            "Ders Güncelleme",
                            $"{ders.DersKodu} kodlu ders güncellendi.");
                    }
                    else
                    {
                        await _logService.LogKaydiEkleAsync(
                            null,
                            "Ders Güncelleme",
                            $"{ders.DersKodu} kodlu ders güncellendi.");
                    }

                    return RedirectToAction(nameof(Index));
                }
                return View(ders);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DersExists(ders.DersId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Ders güncelleme hatası: {ex.Message}");

                ModelState.AddModelError("", "Ders güncellenirken bir hata oluştu.");
                return View(ders);
            }
        }

        // Ders silme sayfası
        [Authorize(Roles = "Admin")]  // Sadece admin silebilir
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ders = await _context.Dersler
                .FirstOrDefaultAsync(d => d.DersId == id);

            if (ders == null)
            {
                return NotFound();
            }

            return View(ders);
        }

        // Ders silme işlemi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ders = await _context.Dersler.FindAsync(id);
                if (ders == null)
                {
                    return NotFound();
                }

                // Fiziksel silme yerine pasif hale getirme
                ders.AktifMi = false;
                _context.Update(ders);
                await _context.SaveChangesAsync();

                // Log kaydı
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int kullaniciId))
                {
                    await _logService.LogKaydiEkleAsync(
                        kullaniciId,
                        "Ders Silme",
                        $"{ders.DersKodu} kodlu ders pasif hale getirildi.");
                }
                else
                {
                    await _logService.LogKaydiEkleAsync(
                        null,
                        "Ders Silme",
                        $"{ders.DersKodu} kodlu ders pasif hale getirildi.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Ders silme hatası: {ex.Message}");

                return View("Error");
            }
        }

        private bool DersExists(int id)
        {
            return _context.Dersler.Any(e => e.DersId == id);
        }
    }
}