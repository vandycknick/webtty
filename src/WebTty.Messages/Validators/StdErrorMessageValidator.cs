using FluentValidation;

namespace WebTty.Messages.Validators
{
    public class StdErrorMessageValidator : AbstractValidator<StdErrorMessage>
    {
        public StdErrorMessageValidator()
        {
            RuleFor(m => m .TabId).NotEmpty().NotNull();
            RuleFor(m => m.Data).NotEmpty().NotNull();
        }
    }
}
