using Assessment.Api.Dtos;
using FluentValidation;

namespace Assessment.Api.Validation;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        When(x => x.Email is not null, () =>
        {
            RuleFor(x => x.Email!)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256);
        });

        When(x => x.DisplayName is not null, () =>
        {
            RuleFor(x => x.DisplayName!)
                .NotEmpty()
                .MaximumLength(200);
        });

        When(x => x.GroupIds is not null, () =>
        {
            RuleForEach(x => x.GroupIds!)
                .GreaterThan(0);
        });
    }
}
