using FluentValidation;
using OgrenciBilgiSistemi.Models;
using System;

namespace OgrenciBilgiSistemi.Validators
{
    public class OgrenciValidator : AbstractValidator<Ogrenci>
    {
        public OgrenciValidator()
        {
            RuleFor(o => o.OgrenciNo)
                .NotEmpty().WithMessage("Öðrenci numarasý boþ olamaz.")
                .Length(5, 20).WithMessage("Öðrenci numarasý 5-20 karakter arasýnda olmalýdýr.")
                .Matches("^[0-9]+$").WithMessage("Öðrenci numarasý sadece rakamlardan oluþmalýdýr.");

            RuleFor(o => o.AdSoyad)
                .NotEmpty().WithMessage("Ad soyad boþ olamaz.")
                .Length(5, 100).WithMessage("Ad soyad 5-100 karakter arasýnda olmalýdýr.")
                .Matches("^[a-zA-ZðüþýöçÐÜÞÝÖÇ ]+$").WithMessage("Ad soyad sadece harflerden oluþmalýdýr.");

            RuleFor(o => o.Bolum)
                .NotEmpty().WithMessage("Bölüm boþ olamaz.")
                .Length(3, 100).WithMessage("Bölüm 3-100 karakter arasýnda olmalýdýr.");

            RuleFor(o => o.GirisTarihi)
                .NotEmpty().WithMessage("Giriþ tarihi boþ olamaz.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Giriþ tarihi bugünden ileri bir tarih olamaz.");

            RuleFor(o => o.Email)
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .When(o => !string.IsNullOrEmpty(o.Email));

            RuleFor(o => o.Telefon)
                .Matches("^[0-9 ()-+]+$").WithMessage("Geçerli bir telefon numarasý giriniz.")
                .When(o => !string.IsNullOrEmpty(o.Telefon));
        }
    }
}