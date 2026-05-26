using FluentValidation;
using System.Text.RegularExpressions;
using SmartLib.Models.Dto;

namespace SmartLib.Validators
{
    public class BookDtoValidator : AbstractValidator<BookDto>
    {
        public BookDtoValidator()
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
