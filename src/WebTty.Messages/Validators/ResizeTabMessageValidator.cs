using FluentValidation;

namespace WebTty.Messages.Validators
{
    public class ResizeTabMessageValidator : AbstractValidator<ResizeTabMessage>
    {
        public ResizeTabMessageValidator()
        {
            RuleFor(m => m.TabId).NotEmpty().NotNull();
            RuleFor(m => m.Cols).NotNull().GreaterThan(0);
            RuleFor(m => m.Rows).NotNull().GreaterThan(0);
        }
    }
}
