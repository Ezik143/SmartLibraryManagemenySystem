using FluentValidation;
using SmartLib.Models.Dto.Create;

namespace SmartLib.Validators
{
    public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
    {
        public CreateTeacherDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.DepartmentId).GreaterThan(0);
        }
    }
}
