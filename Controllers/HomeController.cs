using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using System.Diagnostics;

namespace OgrenciBilgiSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly VeriTabaniContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(VeriTabaniContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Başlık bilgisi
                ViewBag.Message = "Öğrenci Bilgi Sistemi";
                ViewBag.Subtitle = "Hoş Geldiniz";
                
                // Veritabanı istatistiklerini getir
                ViewBag.OgrenciSayisi = await _context.Ogrenciler.Where(o => o.AktifMi).CountAsync();
                ViewBag.DersSayisi = await _context.Dersler.Where(d => d.AktifMi).CountAsync();
                ViewBag.KayitSayisi = await _context.OgrenciDersKayitlari.CountAsync();
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veritabanı erişimi sırasında hata oluştu");
                ViewBag.Message = "Öğrenci Bilgi Sistemi";
                ViewBag.Subtitle = "Sistem Bakımda";
                ViewBag.Error = "Sistem şu anda bakım modunda. Lütfen daha sonra tekrar deneyiniz.";
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}