using FluentValidation;
using SmartLib.Models.Dto.Create;

namespace SmartLib.Validators
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Section).NotEmpty();
            RuleFor(x => x.YearLevel).NotEmpty();
        }
    }
}
