using FluentValidation;
using SmartLib.Models.Dto.Create;

namespace SmartLib.Validators
{
    public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
    {
        public CreateBookDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);
            RuleFor(x => x.Author)
                .NotEmpty()
                .MaximumLength(200);
            RuleFor(x => x.Isbn)
                .NotEmpty()
                .MaximumLength(20)
                .WithMessage("ISBN must be 10 or 13 digits.");
            RuleFor(x => x.Category)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Category));
            RuleFor(x => x.AvailableCopies).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PublishedYear)
                .GreaterThan(0)
                .When(x => x.PublishedYear.HasValue);
        }
    }
}
