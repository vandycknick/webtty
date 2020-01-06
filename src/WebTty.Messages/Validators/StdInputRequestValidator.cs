using FluentValidation;

namespace WebTty.Messages.Validators
{
    public class StdInputRequestValidator : AbstractValidator<StdInputRequest>
    {
        public StdInputRequestValidator()
        {
            RuleFor(m => m.TabId).NotNull().NotEmpty();
            RuleFor(m => m.Payload).NotNull();
        }
    }
}
