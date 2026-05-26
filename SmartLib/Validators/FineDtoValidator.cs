using FluentValidation;
using SmartLib.Models.Dto;

namespace SmartLib.Validators
{
    public class FineDtoValidator : AbstractValidator<FineDto>
    {
        public FineDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.RecordId).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
