using FluentValidation;
using OgrenciBilgiSistemi.Models;
using System;

namespace OgrenciBilgiSistemi.Validators
{
    public class KullaniciValidator : AbstractValidator<Kullanici>
    {
        public KullaniciValidator()
        {
            RuleFor(k => k.KullaniciAdi)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .Length(3, 50).WithMessage("Kullanıcı adı 3-50 karakter arasında olmalıdır.")
                .Matches("^[a-zA-Z0-9_.]+$").WithMessage("Kullanıcı adı sadece harf, rakam, nokta ve alt çizgi içerebilir.");

            RuleFor(k => k.Sifre)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches("[A-Z]").WithMessage("Şifrede en az bir büyük harf olmalıdır.")
                .Matches("[a-z]").WithMessage("Şifrede en az bir küçük harf olmalıdır.")
                .Matches("[0-9]").WithMessage("Şifrede en az bir rakam olmalıdır.");

            RuleFor(k => k.Email)
                .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(k => k.AdSoyad)
                .NotEmpty().WithMessage("Ad soyad boş olamaz.")
                .Length(5, 100).WithMessage("Ad soyad 5-100 karakter arasında olmalıdır.");

            RuleFor(k => k.RolId)
                .NotEmpty().WithMessage("Kullanıcı rolü seçilmelidir.");
        }
    }
}