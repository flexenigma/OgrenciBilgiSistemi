using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    public class OgrenciDersKayit
    {
        public int KayitId { get; set; }

        [Required]
        public int OgrenciId { get; set; }

        [Required]
        public int DersId { get; set; }

        [Required(ErrorMessage = "Dönem bilgisi zorunludur")]
        [Display(Name = "Dönem")]
        [StringLength(20)]
        public string Donem { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? GuncellemeTarihi { get; set; }

        [Display(Name = "Not")]
        [Range(0, 100, ErrorMessage = "Not 0 ile 100 arasında olmalıdır")]
        [Column("Puan")]
        public int? Not { get; set; }

        [Display(Name = "Not Durumu")]
        [Range(0, 100, ErrorMessage = "Not 0 ile 100 arasında olmalıdır")]
        public int? NotDurumu { get; set; }

        // Navigation properties
        public Ogrenci Ogrenci { get; set; }
        public Ders Ders { get; set; }
    }
}