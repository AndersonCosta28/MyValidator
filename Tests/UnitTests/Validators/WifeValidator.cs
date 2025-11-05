internal class WifeValidator : ValidatorBuilder<Person>
{
    public WifeValidator() =>
        this.RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .Message(x => "Wife's name is required.");
}
