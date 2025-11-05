using Mert1s.MyValidator;

internal class PersonValidator : ValidatorBuilder<Person>
{
    public PersonValidator() =>
        this.RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrEmpty(x))
            .Message(x => $"The name '{x}' cannot be null or empty.");
}
