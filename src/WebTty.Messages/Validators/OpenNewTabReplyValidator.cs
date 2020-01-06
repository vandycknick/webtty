using FluentValidation;

namespace WebTty.Messages.Validators
{
    public class OpenNewTabReplyValidator : AbstractValidator<OpenNewTabReply>
    {
        public OpenNewTabReplyValidator()
        {
            RuleFor(m => m.Id).NotEmpty().NotNull();
        }
    }
}
