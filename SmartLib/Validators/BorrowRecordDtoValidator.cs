using FluentValidation;
using SmartLib.Models.Dto;

namespace SmartLib.Validators
{
    public class BorrowRecordDtoValidator : AbstractValidator<BorrowRecordDto>
    {
        public BorrowRecordDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.BookId).GreaterThan(0);
            RuleFor(x => x.BorrowDate).NotEqual(default(DateTime));
            RuleFor(x => x.DueDate).NotEqual(default(DateTime));
            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(x => x.BorrowDate)
                .WithMessage("DueDate must be on or after BorrowDate.");
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
