using System;
using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Models
{
    public class LogKaydi
    {
        [Key]
        public int LogId { get; set; }

        public int? KullaniciId { get; set; }

        [Display(Name = "İşlem Tarihi")]
        public DateTime IslemTarihi { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "İşlem Türü")]
        [StringLength(50)]
        public string IslemTuru { get; set; }

        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        [Display(Name = "IP Adresi")]
        [StringLength(50)]
        public string IPAdresi { get; set; }

        // Navigation property
        public Kullanici Kullanici { get; set; }
    }
}
