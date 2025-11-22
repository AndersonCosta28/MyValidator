using System.Linq;
using Mert1s.MyValidator;
using Xunit;

namespace UnitTests;

public record PersonDto(string Name);

public record UpdatePersonCommand(int Id, string Name) : PersonDto(Name);

public class PersonDtoValidator : ValidatorBuilder<PersonDto>
{
    public PersonDtoValidator()
    {
        this.RuleFor(x => x.Name).NotEmpty().Message("Name is required");
    }
}

public class UpdatePersonCommandValidator : ValidatorBuilder<UpdatePersonCommand>
{
    public UpdatePersonCommandValidator()
    {
        // This uses a cast of the instance to a base DTO type, similar to the reported case
        this.RuleFor(x => (PersonDto)x).SetValidator(new PersonDtoValidator());
        this.RuleFor(x => x.Id).GreaterThan(0).Message("Id must be greater than zero");
    }
}

public class ValidationRuleCastTests
{
    [Fact]
    public void SetValidator_With_CastExpression_DoesNotThrow_And_NestedValidationRuns()
    {
        var validator = new UpdatePersonCommandValidator();
        var cmd = new UpdatePersonCommand(1, string.Empty);

        var results = validator.Validate(cmd);

        // Ensure validation executed and nested validator produced at least one error
        var errors = results.SelectMany(r => r.Errors).ToList();
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Message.Contains("Name is required"));
    }

    [Fact]
    public void SetValidator_With_CastExpression_InvalidId_FailsValidation()
    {
        var validator = new UpdatePersonCommandValidator();
        var cmd = new UpdatePersonCommand(0, "John Doe");
        var results = validator.Validate(cmd);
        // Ensure validation errors
        var errors = results.SelectMany(r => r.Errors).ToList();
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Message.Contains("Id must be greater than zero"));
    }

    [Fact]
    public void SetValidator_With_CastExpression_InvalidIdAndName_FailsValidation()
    {
        var validator = new UpdatePersonCommandValidator();
        var cmd = new UpdatePersonCommand(0, string.Empty);
        var results = validator.Validate(cmd);
        // Ensure validation errors
        var errors = results.SelectMany(r => r.Errors).ToList();
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Message.Contains("Id must be greater than zero"));
        Assert.Contains(errors, e => e.Message.Contains("Name is required"));
    }

    [Fact]
    public void SetValidator_With_CastExpression_ValidData_PassesValidation()
    {
        var validator = new UpdatePersonCommandValidator();
        var cmd = new UpdatePersonCommand(1, "John Doe");
        var results = validator.Validate(cmd);
        // Ensure no validation errors
        var errors = results.SelectMany(r => r.Errors).ToList();
        Assert.Empty(errors);
    }
}
