using OgrenciBilgiSistemi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OgrenciBilgiSistemi.Services;

namespace OgrenciBilgiSistemi.Services
{
    public interface IRaporService
    {
        Task<Dictionary<string, int>> BolumlereGoreOgrenciSayilariAsync();
        Task<List<DersKayitRaporu>> DerslereGoreKayitSayilariAsync();
        Task<List<KullaniciIslemRaporu>> KullaniciIslemleriRaporuAsync(DateTime baslangic, DateTime bitis);
    }
} 