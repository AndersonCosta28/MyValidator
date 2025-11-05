namespace UnitTests;
internal class GuidModel { public Guid Value { get; set; } }

public class RuleBuilderGuidExtensionsTests
{
    [Fact]
    public void Guid_Extensions_Should_Produce_Expected_Messages()
    {
        // Case1: empty guid -> NotEmpty should fail
        var modelEmpty = new GuidModel { Value = Guid.Empty };
        var validatorEmpty = new GuidValidator_EmptyCheck();
        var messagesEmpty = validatorEmpty.Validate(modelEmpty).SelectMany(r => r.Errors).Select(e => e.Message).ToList();
        Assert.Contains($"{Guid.Empty} is empty", messagesEmpty);

        // Case2: non-empty guid not in allowed list -> IsOneOf should fail
        var guid = Guid.NewGuid();
        var model = new GuidModel { Value = guid };
        var validatorNotAllowed = new GuidValidator_NotAllowedCheck();
        var messages = validatorNotAllowed.Validate(model).SelectMany(r => r.Errors).Select(e => e.Message).ToList();
        Assert.Contains($"{guid} is not an allowed value", messages);
    }

    [Fact]
    public void Guid_IsOneOf_Should_Pass_When_Allowed()
    {
        var guid = Guid.NewGuid();
        var model = new GuidModel { Value = guid };
        var validator = new GuidValidator_AllowedCheck(guid);
        var results = validator.Validate(model);

        Assert.All(results, r => Assert.True(r.IsValid));
    }

    private class GuidValidator_EmptyCheck : ValidatorBuilder<GuidModel>
    {
        public GuidValidator_EmptyCheck() => this.RuleFor(x => x.Value)
        .NotEmpty();
    }

    private class GuidValidator_NotAllowedCheck : ValidatorBuilder<GuidModel>
    {
        public GuidValidator_NotAllowedCheck() => this.RuleFor(x => x.Value)
        .IsOneOf(Guid.Empty); // allow only empty -> non-empty will fail
    }

    private class GuidValidator_AllowedCheck : ValidatorBuilder<GuidModel>
    {
        public GuidValidator_AllowedCheck(Guid allowed) => this.RuleFor(x => x.Value).IsOneOf(allowed);
    }
}
