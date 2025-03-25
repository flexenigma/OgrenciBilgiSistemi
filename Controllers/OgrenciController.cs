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
    public class OgrenciController : Controller
    {
        private readonly VeriTabaniContext _context;
        private readonly ILogService _logService;

        public OgrenciController(VeriTabaniContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // Öğrenci listesi
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                IQueryable<Ogrenci> ogrenciler = _context.Ogrenciler;

                // Arama filtresi
                if (!string.IsNullOrEmpty(searchString))
                {
                    ogrenciler = ogrenciler.Where(o =>
                        o.AdSoyad.Contains(searchString) ||
                        o.OgrenciNo.Contains(searchString) ||
                        o.Bolum.Contains(searchString));
                }

                return View(await ogrenciler.ToListAsync());
            }
            catch (Exception ex)
            {
                // Hata log kaydı
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Öğrenci listesi görüntüleme hatası: {ex.Message}");

                return View("Error");
            }
        }

        // Öğrenci detayı
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var ogrenci = await _context.Ogrenciler
                    .Include(o => o.Kayitlar)
                    .ThenInclude(k => k.Ders)
                    .FirstOrDefaultAsync(o => o.OgrenciId == id);

                if (ogrenci == null)
                {
                    return NotFound();
                }

                return View(ogrenci);
            }
            catch (Exception ex)
            {
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Öğrenci detay görüntüleme hatası: {ex.Message}");

                return View("Error");
            }
        }

        // Öğrenci oluşturma sayfası
        [HttpGet]
        [Authorize(Roles = "Admin,Ogretmen")] // Sadece admin ve öğretmenler erişebilir
        public IActionResult Create()
        {
            return View();
        }

        // Öğrenci oluşturma işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Create([Bind("OgrenciNo,AdSoyad,Bolum,GirisTarihi,Telefon,Email,AktifMi")] Ogrenci ogrenci)
        {
            try
            {
                // ModelState hatalarını göster
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();
                    
                    foreach (var error in errors)
                    {
                        TempData["ErrorMessage"] = error;
                    }
                    
                    return View(ogrenci);
                }

                // Öğrenci numarası zaten var mı kontrol et
                if (await _context.Ogrenciler.AnyAsync(o => o.OgrenciNo == ogrenci.OgrenciNo))
                {
                    ModelState.AddModelError("OgrenciNo", "Bu öğrenci numarası zaten kullanılıyor.");
                    return View(ogrenci);
                }

                // Doğrudan kaydet - VeriTabaniContext içinde retry mekanizması var
                _context.Add(ogrenci);
                var result = await _context.SaveChangesAsync();

                // Başarı durumu kontrolü
                if (result > 0)
                {
                    // Log kaydı
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int kullaniciId))
                    {
                        await _logService.LogKaydiEkleAsync(
                            kullaniciId,
                            "Öğrenci Ekleme",
                            $"{ogrenci.OgrenciNo} numaralı öğrenci eklendi.");
                    }
                    else
                    {
                        await _logService.LogKaydiEkleAsync(
                            null,
                            "Öğrenci Ekleme",
                            $"{ogrenci.OgrenciNo} numaralı öğrenci eklendi.");
                    }

                    TempData["SuccessMessage"] = "Öğrenci başarıyla kaydedildi.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // SaveChanges sonucunda hiçbir satır etkilenmediyse
                    ModelState.AddModelError("", "Öğrenci kaydedilemedi. Lütfen tekrar deneyin.");
                    return View(ogrenci);
                }
            }
            catch (Exception ex)
            {
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Öğrenci ekleme hatası: {ex.Message}");

                ModelState.AddModelError("", "Öğrenci eklenirken bir hata oluştu: " + ex.Message);
                return View(ogrenci);
            }
        }

        // Öğrenci düzenleme sayfası
        [HttpGet]
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ogrenci = await _context.Ogrenciler.FindAsync(id);
            if (ogrenci == null)
            {
                return NotFound();
            }

            return View(ogrenci);
        }

        // Öğrenci düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Edit(int id, [Bind("OgrenciId,OgrenciNo,AdSoyad,Bolum,GirisTarihi,Telefon,Email,AktifMi")] Ogrenci ogrenci)
        {
            if (id != ogrenci.OgrenciId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Öğrenci numarası başka bir öğrencide var mı kontrol et
                    if (await _context.Ogrenciler.AnyAsync(o =>
                        o.OgrenciNo == ogrenci.OgrenciNo &&
                        o.OgrenciId != ogrenci.OgrenciId))
                    {
                        ModelState.AddModelError("OgrenciNo", "Bu öğrenci numarası zaten kullanılıyor.");
                        return View(ogrenci);
                    }

                    _context.Update(ogrenci);
                    await _context.SaveChangesAsync();

                    // Log kaydı
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int kullaniciId))
                    {
                        await _logService.LogKaydiEkleAsync(
                            kullaniciId,
                            "Öğrenci Güncelleme",
                            $"{ogrenci.OgrenciNo} numaralı öğrenci güncellendi.");
                    }
                    else
                    {
                        await _logService.LogKaydiEkleAsync(
                            null,
                            "Öğrenci Güncelleme",
                            $"{ogrenci.OgrenciNo} numaralı öğrenci güncellendi.");
                    }

                    return RedirectToAction(nameof(Index));
                }
                return View(ogrenci);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OgrenciExists(ogrenci.OgrenciId))
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
                    $"Öğrenci güncelleme hatası: {ex.Message}");

                ModelState.AddModelError("", "Öğrenci güncellenirken bir hata oluştu.");
                return View(ogrenci);
            }
        }

        // Öğrenci silme sayfası
        [Authorize(Roles = "Admin")]  // Sadece admin silebilir
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ogrenci = await _context.Ogrenciler
                .FirstOrDefaultAsync(o => o.OgrenciId == id);

            if (ogrenci == null)
            {
                return NotFound();
            }

            return View(ogrenci);
        }

        // Öğrenci silme işlemi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ogrenci = await _context.Ogrenciler.FindAsync(id);
                if (ogrenci == null)
                {
                    return NotFound();
                }

                // Fiziksel silme yerine pasif hale getirme
                ogrenci.AktifMi = false;
                _context.Update(ogrenci);
                await _context.SaveChangesAsync();

                // Log kaydı
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int kullaniciId))
                {
                    await _logService.LogKaydiEkleAsync(
                        kullaniciId,
                        "Öğrenci Silme",
                        $"{ogrenci.OgrenciNo} numaralı öğrenci pasif hale getirildi.");
                }
                else
                {
                    await _logService.LogKaydiEkleAsync(
                        null,
                        "Öğrenci Silme",
                        $"{ogrenci.OgrenciNo} numaralı öğrenci pasif hale getirildi.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _logService.LogKaydiEkleAsync(
                    null,
                    "Hata",
                    $"Öğrenci silme hatası: {ex.Message}");

                return View("Error");
            }
        }

        private bool OgrenciExists(int id)
        {
            return _context.Ogrenciler.Any(e => e.OgrenciId == id);
        }
    }
}