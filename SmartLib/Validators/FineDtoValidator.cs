using FluentValidation;
using SmartLib.Models.Dto.Create;

namespace SmartLib.Validators
{
    public class CreateFineDtoValidator : AbstractValidator<CreateFineDto>
    {
        public CreateFineDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.RecordId).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
