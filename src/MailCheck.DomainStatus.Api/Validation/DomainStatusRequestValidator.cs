using System.Collections.Generic;
using FluentValidation;

namespace MailCheck.DomainStatus.Api.Validation
{
    public class DomainStatusRequestValidator : AbstractValidator<List<string>>
    {
        public DomainStatusRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(_ => _)
                .NotNull()
                .WithMessage("A \"domains\" field is required.")
                .NotEmpty()
                .WithMessage("The \"domains\" field should not be empty.");
        }
    }
}
