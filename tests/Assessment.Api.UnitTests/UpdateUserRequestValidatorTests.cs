using Assessment.Api.Dtos;
using Assessment.Api.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Assessment.Api.UnitTests;

public class UpdateUserRequestValidatorTests
{
    private readonly UpdateUserRequestValidator _validator = new();

    [Fact]
    public void Invalid_email_fails()
    {
        var model = new UpdateUserRequest { Email = "bad", DisplayName = "Test", IsActive = true };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Empty_display_name_fails()
    {
        var model = new UpdateUserRequest { Email = "test@example.com", DisplayName = "", IsActive = true };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Valid_model_passes()
    {
        var model = new UpdateUserRequest { Email = "test@example.com", DisplayName = "Updated", IsActive = false };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
