using Assessment.Api.Dtos;
using FluentValidation;

namespace Assessment.Api.Validation;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleForEach(x => x.GroupIds)
            .GreaterThan(0);
    }
}
