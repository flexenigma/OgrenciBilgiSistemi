using FluentValidation;
using OgrenciBilgiSistemi.Models;
namespace OgrenciBilgiSistemi.Validators
{
    public class OgrenciDersKayitValidator : AbstractValidator<OgrenciDersKayit>
    {
        public OgrenciDersKayitValidator()
        {
            RuleFor(k => k.OgrenciId)
                .NotEmpty().WithMessage("��renci se�ilmelidir.");

            RuleFor(k => k.DersId)
                .NotEmpty().WithMessage("Ders se�ilmelidir.");

            RuleFor(k => k.Donem)
                .NotEmpty().WithMessage("D�nem bilgisi bo� olamaz.")
                .Length(3, 20).WithMessage("D�nem bilgisi 3-20 karakter aras�nda olmal�d�r.");

            RuleFor(k => k.NotDurumu)
                .InclusiveBetween(0, 100).WithMessage("Not de�eri 0-100 aras�nda olmal�d�r.")
                .When(k => k.NotDurumu.HasValue);
        }
    }
}