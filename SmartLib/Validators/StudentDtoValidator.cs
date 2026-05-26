using FluentValidation;
using SmartLib.Models.Dto;

namespace SmartLib.Validators
{
    public class StudentDtoValidator : AbstractValidator<StudentDto>
    {
        public StudentDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Section).NotEmpty();
            RuleFor(x => x.YearLevel).NotEmpty();
        }
    }
}
