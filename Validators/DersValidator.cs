using FluentValidation;
using OgrenciBilgiSistemi.Models;
namespace OgrenciBilgiSistemi.Validators
{
    public class DersValidator : AbstractValidator<Ders>
    {
        public DersValidator()
        {
            RuleFor(d => d.DersKodu)
                .NotEmpty().WithMessage("Ders kodu boş olamaz.")
                .Length(2, 20).WithMessage("Ders kodu 2-20 karakter arasında olmalıdır.")
                .Matches("^[A-Z0-9]+$").WithMessage("Ders kodu sadece büyük harf ve rakamlardan oluşmalıdır.");

            RuleFor(d => d.DersAdi)
                .NotEmpty().WithMessage("Ders adı boş olamaz.")
                .Length(3, 100).WithMessage("Ders adı 3-100 karakter arasında olmalıdır.");

            RuleFor(d => d.Kredi)
                .NotEmpty().WithMessage("Kredi değeri boş olamaz.")
                .InclusiveBetween(1, 10).WithMessage("Kredi değeri 1-10 arasında olmalıdır.");
        }
    }
}