using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OgrenciBilgiSistemi.Services;
using System.Security.Claims;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize(Roles = "Admin,Ogretmen")]  // Sadece admin ve öğretmenler raporları görebilir
    public class RaporController : Controller
    {
        private readonly IRaporService _raporService;

        public RaporController(IRaporService raporService)
        {
            _raporService = raporService;
        }

        // Ana rapor sayfası
        public IActionResult Index()
        {
            return View();
        }

        // Bölümlere göre öğrenci sayıları raporu
        public async Task<IActionResult> BolumOgrenciRaporu()
        {
            try
            {
                var model = await _raporService.BolumlereGoreOgrenciSayilariAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Rapor görüntüleme hatası: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Derslere göre kayıt sayıları raporu
        public async Task<IActionResult> DersKayitRaporu()
        {
            try
            {
                var model = await _raporService.DerslereGoreKayitSayilariAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Rapor görüntüleme hatası: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Kullanıcı işlemleri raporu
        public async Task<IActionResult> KullaniciIslemleriRaporu(DateTime? baslangic, DateTime? bitis)
        {
            try
            {
                if (!baslangic.HasValue)
                    baslangic = DateTime.Now.AddDays(-30);

                if (!bitis.HasValue)
                    bitis = DateTime.Now;

                var model = await _raporService.KullaniciIslemleriRaporuAsync(baslangic.Value, bitis.Value);
                ViewBag.Baslangic = baslangic.Value;
                ViewBag.Bitis = bitis.Value;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Rapor görüntüleme hatası: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}