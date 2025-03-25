using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Services
{
    public class LogService : ILogService
    {
        private readonly VeriTabaniContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(VeriTabaniContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogKaydiEkleAsync(int? kullaniciId, string islemTuru, string aciklama)
        {
            try
            {
                var logKaydi = new LogKaydi
                {
                    KullaniciId = kullaniciId,
                    IslemTarihi = DateTime.Now,
                    IslemTuru = islemTuru,
                    Aciklama = aciklama,
                    IPAdresi = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                };

                _context.LogKayitlari.Add(logKaydi);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Log kaydı oluşturulurken hata olursa sessizce devam et
                // Gerçek uygulamada burada alternatif bir log mekanizması kullanılabilir
            }
        }
    }
}