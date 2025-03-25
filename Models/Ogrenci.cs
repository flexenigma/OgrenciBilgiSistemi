using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace OgrenciBilgiSistemi.Models
{
    public class Ogrenci
    {
        public int OgrenciId { get; set; }

        [Required(ErrorMessage = "Öğrenci numarası zorunludur")]
        [Display(Name = "Öğrenci Numarası")]
        [StringLength(20)]
        public string OgrenciNo { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100)]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Bölüm alanı zorunludur")]
        [Display(Name = "Bölüm")]
        [StringLength(100)]
        public string Bolum { get; set; }

        [Required(ErrorMessage = "Giriş tarihi zorunludur")]
        [Display(Name = "Giriş Tarihi")]
        [DataType(DataType.Date)]
        public DateTime GirisTarihi { get; set; }

        [Display(Name = "Telefon")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string Telefon { get; set; }

        [Display(Name = "E-posta")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool AktifMi { get; set; } = true;

        // Navigation property
        public ICollection<OgrenciDersKayit> Kayitlar { get; set; }
    }
}