using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace OgrenciBilgiSistemi.Models
{
    public class Ders
    {
        public int DersId { get; set; }

        [Required(ErrorMessage = "Ders kodu zorunludur")]
        [Display(Name = "Ders Kodu")]
        [StringLength(20)]
        public string DersKodu { get; set; }

        [Required(ErrorMessage = "Ders adı zorunludur")]
        [Display(Name = "Ders Adı")]
        [StringLength(100)]
        public string DersAdi { get; set; }

        [Required(ErrorMessage = "Kredi değeri zorunludur")]
        [Display(Name = "Kredi")]
        [Range(1, 10, ErrorMessage = "Kredi 1 ile 10 arasında olmalıdır")]
        public int Kredi { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool AktifMi { get; set; } = true;

        // Navigation property
        public ICollection<OgrenciDersKayit> Kayitlar { get; set; }
    }
}