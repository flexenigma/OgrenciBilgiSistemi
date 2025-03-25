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
                .NotEmpty().WithMessage("��renci numaras� bo� olamaz.")
                .Length(5, 20).WithMessage("��renci numaras� 5-20 karakter aras�nda olmal�d�r.")
                .Matches("^[0-9]+$").WithMessage("��renci numaras� sadece rakamlardan olu�mal�d�r.");

            RuleFor(o => o.AdSoyad)
                .NotEmpty().WithMessage("Ad soyad bo� olamaz.")
                .Length(5, 100).WithMessage("Ad soyad 5-100 karakter aras�nda olmal�d�r.")
                .Matches("^[a-zA-Z������������ ]+$").WithMessage("Ad soyad sadece harflerden olu�mal�d�r.");

            RuleFor(o => o.Bolum)
                .NotEmpty().WithMessage("B�l�m bo� olamaz.")
                .Length(3, 100).WithMessage("B�l�m 3-100 karakter aras�nda olmal�d�r.");

            RuleFor(o => o.GirisTarihi)
                .NotEmpty().WithMessage("Giri� tarihi bo� olamaz.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Giri� tarihi bug�nden ileri bir tarih olamaz.");

            RuleFor(o => o.Email)
                .EmailAddress().WithMessage("Ge�erli bir e-posta adresi giriniz.")
                .When(o => !string.IsNullOrEmpty(o.Email));

            RuleFor(o => o.Telefon)
                .Matches("^[0-9 ()-+]+$").WithMessage("Ge�erli bir telefon numaras� giriniz.")
                .When(o => !string.IsNullOrEmpty(o.Telefon));
        }
    }
}