using System.Threading.Tasks;

namespace OgrenciBilgiSistemi.Services
{
    public interface ILogService
    {
        Task LogKaydiEkleAsync(int? kullaniciId, string islemTuru, string aciklama);
    }
} 