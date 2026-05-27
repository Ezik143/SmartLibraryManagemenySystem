using FluentValidation;
using SmartLib.Models.Dto.Create;

namespace SmartLib.Validators
{
    public class CreateApplicationUserDtoValidator : AbstractValidator<CreateApplicationUserDto>
    {
        public CreateApplicationUserDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .Matches("[A-Z]")
                .Matches("[^a-zA-Z0-9]");
        }
    }
}
