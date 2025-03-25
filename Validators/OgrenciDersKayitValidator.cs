using FluentValidation;
using OgrenciBilgiSistemi.Models;
namespace OgrenciBilgiSistemi.Validators
{
    public class OgrenciDersKayitValidator : AbstractValidator<OgrenciDersKayit>
    {
        public OgrenciDersKayitValidator()
        {
            RuleFor(k => k.OgrenciId)
                .NotEmpty().WithMessage("Öðrenci seçilmelidir.");

            RuleFor(k => k.DersId)
                .NotEmpty().WithMessage("Ders seçilmelidir.");

            RuleFor(k => k.Donem)
                .NotEmpty().WithMessage("Dönem bilgisi boþ olamaz.")
                .Length(3, 20).WithMessage("Dönem bilgisi 3-20 karakter arasýnda olmalýdýr.");

            RuleFor(k => k.NotDurumu)
                .InclusiveBetween(0, 100).WithMessage("Not deðeri 0-100 arasýnda olmalýdýr.")
                .When(k => k.NotDurumu.HasValue);
        }
    }
}