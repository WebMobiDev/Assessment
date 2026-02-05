using Assessment.Api.Dtos;
using Assessment.Api.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Assessment.Api.UnitTests;

public class CreateUserRequestValidatorTests
{
    private readonly CreateUserRequestValidator _validator = new();

    [Fact]
    public void Invalid_email_fails()
    {
        var model = new CreateUserRequest { Email = "not-an-email", DisplayName = "Test", IsActive = true };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Empty_display_name_fails()
    {
        var model = new CreateUserRequest { Email = "test@example.com", DisplayName = "", IsActive = true };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Valid_model_passes()
    {
        var model = new CreateUserRequest { Email = "test@example.com", DisplayName = "Test User", IsActive = true };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
