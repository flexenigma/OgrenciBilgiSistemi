using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class OgrenciDersKayitController : Controller
    {
        private readonly VeriTabaniContext _context;
        private readonly ILogService _logService;
        private readonly ILogger<OgrenciDersKayitController> _logger;

        public OgrenciDersKayitController(VeriTabaniContext context, ILogService logService, 
            ILogger<OgrenciDersKayitController> logger)
        {
            _context = context;
            _logService = logService;
            _logger = logger;
        }

        // Öğrenci ders kayıtları listesi
        public async Task<IActionResult> Index()
        {
            try
            {
                // En basit şekilde sadece kayıtları getir
                var kayitlar = await _context.OgrenciDersKayitlari.ToListAsync();
                
                // Sonra ilişkili verileri manuel yükle
                foreach (var kayit in kayitlar)
                {
                    kayit.Ogrenci = await _context.Ogrenciler.FindAsync(kayit.OgrenciId);
                    kayit.Ders = await _context.Dersler.FindAsync(kayit.DersId);
                }
                
                return View(kayitlar);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View(new List<OgrenciDersKayit>());
            }
        }

        // Ders kaydı oluşturma sayfası
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.OgrenciId = new SelectList(await _context.Ogrenciler
                    .Where(o => o.AktifMi)
                    .OrderBy(o => o.AdSoyad)
                    .ToListAsync(), "OgrenciId", "AdSoyad");

                ViewBag.DersId = new SelectList(await _context.Dersler
                    .Where(d => d.AktifMi)
                    .OrderBy(d => d.DersAdi)
                    .ToListAsync(), "DersId", "DersAdi");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ders kaydı oluşturma sayfası açılırken hata oluştu");
                TempData["ErrorMessage"] = "Ders kaydı oluşturma sayfası açılırken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Ders kaydı oluşturma işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Create(OgrenciDersKayit kayit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Aynı ders kaydı var mı kontrol et
                    var mevcutKayit = await _context.OgrenciDersKayitlari
                        .FirstOrDefaultAsync(k => k.OgrenciId == kayit.OgrenciId && k.DersId == kayit.DersId && k.Donem == kayit.Donem);

                    if (mevcutKayit != null)
                    {
                        ModelState.AddModelError("", "Bu öğrenci zaten bu derse kayıtlı.");
                        
                        ViewBag.OgrenciId = new SelectList(await _context.Ogrenciler
                            .Where(o => o.AktifMi)
                            .OrderBy(o => o.AdSoyad)
                            .ToListAsync(), "OgrenciId", "AdSoyad", kayit.OgrenciId);

                        ViewBag.DersId = new SelectList(await _context.Dersler
                            .Where(d => d.AktifMi)
                            .OrderBy(d => d.DersAdi)
                            .ToListAsync(), "DersId", "DersAdi", kayit.DersId);

                        return View(kayit);
                    }

                    // Kayıt tarihi ekle
                    kayit.KayitTarihi = DateTime.Now;

                    // Kontrol noktası - loglama
                    _logger.LogInformation($"Ders kaydı ekleme başlatıldı: Öğrenci ID={kayit.OgrenciId}, Ders ID={kayit.DersId}, Dönem={kayit.Donem}");

                    // Kayıt işlemini gerçekleştir
                    _context.Add(kayit);
                    var result = await _context.SaveChangesAsync();

                    if (result > 0)
                    {
                        _logger.LogInformation($"Ders kaydı başarıyla eklendi: ID={kayit.KayitId}, Öğrenci ID={kayit.OgrenciId}, Ders ID={kayit.DersId}");
                        
                        // Log kaydı ekle
                        await _logService.LogKaydiEkleAsync(
                            null,
                            "Ders Kaydı Ekleme",
                            $"Yeni ders kaydı oluşturuldu: Öğrenci ID {kayit.OgrenciId}, Ders ID {kayit.DersId}");
                        
                        TempData["SuccessMessage"] = "Ders kaydı başarıyla oluşturuldu.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning($"Ders kaydı eklenemedi: Öğrenci ID={kayit.OgrenciId}, Ders ID={kayit.DersId}, hiçbir satır etkilenmedi");
                        ModelState.AddModelError("", "Ders kaydı eklenemedi. Lütfen tekrar deneyin.");
                        
                        ViewBag.OgrenciId = new SelectList(await _context.Ogrenciler
                            .Where(o => o.AktifMi)
                            .OrderBy(o => o.AdSoyad)
                            .ToListAsync(), "OgrenciId", "AdSoyad", kayit.OgrenciId);

                        ViewBag.DersId = new SelectList(await _context.Dersler
                            .Where(d => d.AktifMi)
                            .OrderBy(d => d.DersAdi)
                            .ToListAsync(), "DersId", "DersAdi", kayit.DersId);
                            
                        return View(kayit);
                    }
                }

                ViewBag.OgrenciId = new SelectList(await _context.Ogrenciler
                    .Where(o => o.AktifMi)
                    .OrderBy(o => o.AdSoyad)
                    .ToListAsync(), "OgrenciId", "AdSoyad", kayit.OgrenciId);

                ViewBag.DersId = new SelectList(await _context.Dersler
                    .Where(d => d.AktifMi)
                    .OrderBy(d => d.DersAdi)
                    .ToListAsync(), "DersId", "DersAdi", kayit.DersId);

                return View(kayit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ders kaydı oluşturulurken hata: {ex.Message}, Stack trace: {ex.StackTrace}");
                
                ViewBag.OgrenciId = new SelectList(await _context.Ogrenciler
                    .Where(o => o.AktifMi)
                    .OrderBy(o => o.AdSoyad)
                    .ToListAsync(), "OgrenciId", "AdSoyad", kayit.OgrenciId);

                ViewBag.DersId = new SelectList(await _context.Dersler
                    .Where(d => d.AktifMi)
                    .OrderBy(d => d.DersAdi)
                    .ToListAsync(), "DersId", "DersAdi", kayit.DersId);

                ModelState.AddModelError("", "Ders kaydı oluşturulurken bir hata oluştu: " + ex.Message);
                return View(kayit);
            }
        }

        // Kayıt düzenleme sayfası
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var kayit = await _context.OgrenciDersKayitlari
                    .Include(k => k.Ogrenci)
                    .Include(k => k.Ders)
                    .FirstOrDefaultAsync(k => k.KayitId == id);

                if (kayit == null)
                {
                    return NotFound();
                }

                ViewBag.DersAdi = kayit.Ders.DersAdi;
                ViewBag.OgrenciAdSoyad = kayit.Ogrenci.AdSoyad;

                return View(kayit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ders kaydı düzenleme sayfası açılırken hata oluştu (ID: {id})");
                TempData["ErrorMessage"] = "Ders kaydı düzenleme sayfası açılırken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Kayıt düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ogretmen")]
        public async Task<IActionResult> Edit(int id, OgrenciDersKayit kayit)
        {
            if (id != kayit.KayitId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var mevcut = await _context.OgrenciDersKayitlari
                        .AsNoTracking()
                        .FirstOrDefaultAsync(k => k.KayitId == id);

                    if (mevcut == null)
                    {
                        return NotFound();
                    }

                    // Sadece not ve durum bilgilerini güncelle
                    kayit.OgrenciId = mevcut.OgrenciId;
                    kayit.DersId = mevcut.DersId;
                    kayit.KayitTarihi = mevcut.KayitTarihi;
                    kayit.GuncellemeTarihi = DateTime.Now;

                    _context.Update(kayit);
                    await _context.SaveChangesAsync();

                    // Log kaydı ekle
                    _logger.LogInformation($"Ders kaydı güncellendi: ID {kayit.KayitId}, Yeni Not: {kayit.Not}");
                    
                    TempData["SuccessMessage"] = "Ders kaydı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                var kayitBilgisi = await _context.OgrenciDersKayitlari
                    .Include(k => k.Ogrenci)
                    .Include(k => k.Ders)
                    .FirstOrDefaultAsync(k => k.KayitId == id);

                ViewBag.DersAdi = kayitBilgisi.Ders.DersAdi;
                ViewBag.OgrenciAdSoyad = kayitBilgisi.Ogrenci.AdSoyad;

                return View(kayit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ders kaydı güncellenirken hata oluştu (ID: {id})");
                
                var kayitBilgisi = await _context.OgrenciDersKayitlari
                    .Include(k => k.Ogrenci)
                    .Include(k => k.Ders)
                    .FirstOrDefaultAsync(k => k.KayitId == id);

                if (kayitBilgisi != null)
                {
                    ViewBag.DersAdi = kayitBilgisi.Ders.DersAdi;
                    ViewBag.OgrenciAdSoyad = kayitBilgisi.Ogrenci.AdSoyad;
                }

                ModelState.AddModelError("", "Ders kaydı güncellenirken bir hata oluştu: " + ex.Message);
                return View(kayit);
            }
        }

        // Kayıt silme sayfası
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var kayit = await _context.OgrenciDersKayitlari
                    .Include(k => k.Ogrenci)
                    .Include(k => k.Ders)
                    .FirstOrDefaultAsync(k => k.KayitId == id);

                if (kayit == null)
                {
                    return NotFound();
                }

                return View(kayit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ders kaydı silme sayfası açılırken hata oluştu (ID: {id})");
                TempData["ErrorMessage"] = "Ders kaydı silme sayfası açılırken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Kayıt silme işlemi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var kayit = await _context.OgrenciDersKayitlari.FindAsync(id);
                if (kayit == null)
                {
                    return NotFound();
                }

                _context.OgrenciDersKayitlari.Remove(kayit);
                await _context.SaveChangesAsync();

                // Log kaydı ekle
                _logger.LogInformation($"Ders kaydı silindi: ID {id}");
                
                TempData["SuccessMessage"] = "Ders kaydı başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ders kaydı silinirken hata oluştu (ID: {id})");
                TempData["ErrorMessage"] = "Ders kaydı silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Öğrencinin dersleri
        [Authorize]
        public async Task<IActionResult> OgrenciDersleri(int id)
        {
            try
            {
                var ogrenci = await _context.Ogrenciler.FindAsync(id);
                if (ogrenci == null)
                {
                    return NotFound();
                }

                var kayitlar = await _context.OgrenciDersKayitlari
                    .Include(k => k.Ders)
                    .Where(k => k.OgrenciId == id)
                    .OrderBy(k => k.Ders.DersAdi)
                    .ToListAsync();

                ViewBag.Ogrenci = ogrenci;
                return View(kayitlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Öğrenci dersleri listelenirken hata oluştu (Öğrenci ID: {id})");
                TempData["ErrorMessage"] = "Öğrenci dersleri listelenirken bir hata oluştu.";
                return RedirectToAction("Index", "Ogrenci");
            }
        }

        // Dersin öğrencileri
        [Authorize]
        public async Task<IActionResult> DersOgrencileri(int id)
        {
            try
            {
                var ders = await _context.Dersler.FindAsync(id);
                if (ders == null)
                {
                    return NotFound();
                }

                var kayitlar = await _context.OgrenciDersKayitlari
                    .Include(k => k.Ogrenci)
                    .Where(k => k.DersId == id)
                    .OrderBy(k => k.Ogrenci.AdSoyad)
                    .ToListAsync();

                ViewBag.Ders = ders;
                return View(kayitlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ders öğrencileri listelenirken hata oluştu (Ders ID: {id})");
                TempData["ErrorMessage"] = "Ders öğrencileri listelenirken bir hata oluştu.";
                return RedirectToAction("Index", "Ders");
            }
        }
    }
} 