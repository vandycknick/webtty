using FluentValidation;

namespace WebTty.Messages.Validators
{
    public class StdOutMessageValidator : AbstractValidator<StdOutMessage>
    {
        public StdOutMessageValidator()
        {
            RuleFor(m => m.TabId).NotNull().NotEmpty();
            RuleFor(m => m.Data).NotNull();
        }
    }
}
