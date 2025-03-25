using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OgrenciBilgiSistemi.Services
{
    public class RaporService : IRaporService
    {
        private readonly VeriTabaniContext _context;

        public RaporService(VeriTabaniContext context)
        {
            _context = context;
        }

        // Bölümlere göre öğrenci sayıları raporu
        public async Task<Dictionary<string, int>> BolumlereGoreOgrenciSayilariAsync()
        {
            var result = await _context.Ogrenciler
                .Where(o => o.AktifMi)
                .GroupBy(o => o.Bolum)
                .Select(g => new { Bolum = g.Key, Sayi = g.Count() })
                .ToDictionaryAsync(x => x.Bolum, x => x.Sayi);

            return result;
        }

        // Derslere göre kayıt sayıları raporu
        public async Task<List<DersKayitRaporu>> DerslereGoreKayitSayilariAsync()
        {
            var result = await _context.Dersler
                .Where(d => d.AktifMi)
                .Select(d => new DersKayitRaporu
                {
                    DersKodu = d.DersKodu,
                    DersAdi = d.DersAdi,
                    KayitSayisi = d.Kayitlar.Count
                })
                .ToListAsync();

            return result;
        }

        // Kullanıcı işlemleri raporu 
        public async Task<List<KullaniciIslemRaporu>> KullaniciIslemleriRaporuAsync(DateTime baslangic, DateTime bitis)
        {
            try
            {
                var kullanicilarDict = await _context.Kullanicilar
                    .Where(k => k.AktifMi)
                    .ToDictionaryAsync(k => k.KullaniciId, k => k.KullaniciAdi);

                var result = await _context.LogKayitlari
                    .Where(l => l.IslemTarihi >= baslangic && l.IslemTarihi <= bitis)
                    .GroupBy(l => new { l.KullaniciId, l.IslemTuru })
                    .Select(g => new KullaniciIslemRaporu
                    {
                        KullaniciAdi = g.Key.KullaniciId.HasValue && kullanicilarDict.ContainsKey(g.Key.KullaniciId.Value) 
                            ? kullanicilarDict[g.Key.KullaniciId.Value] 
                            : "Sistem",
                        IslemTuru = g.Key.IslemTuru,
                        IslemSayisi = g.Count()
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı işlemleri raporu oluşturulurken hata: {ex.Message}", ex);
            }
        }
    }

    // Rapor model sınıfları
    public class DersKayitRaporu
    {
        public string DersKodu { get; set; }
        public string DersAdi { get; set; }
        public int KayitSayisi { get; set; }
    }

    public class KullaniciIslemRaporu
    {
        public string KullaniciAdi { get; set; }
        public string IslemTuru { get; set; }
        public int IslemSayisi { get; set; }
    }
}
