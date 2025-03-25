using System;
using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Models
{
    public class Kullanici
    {
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [Display(Name = "Kullanıcı Adı")]
        [StringLength(50)]
        public string KullaniciAdi { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [Display(Name = "Şifre")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Sifre { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [Display(Name = "E-posta")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100)]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Rol seçimi zorunludur")]
        [Display(Name = "Kullanıcı Rolü")]
        public int RolId { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool AktifMi { get; set; } = true;

        [Display(Name = "Oluşturma Tarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? GuncellemeTarihi { get; set; }

        // Navigation property
        public Rol Rol { get; set; }
    }

    public class Rol
    {
        public int RolId { get; set; }

        [Required]
        [StringLength(50)]
        public string RolAdi { get; set; }

        // Navigation property
        public ICollection<Kullanici> Kullanicilar { get; set; }
    }
}