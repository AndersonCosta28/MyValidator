using Mert1s.MyValidator;

internal class ChildrenValidator : ValidatorBuilder<Person>
{
    public ChildrenValidator() =>
        this.RuleFor(x => x.DateOfBirth)
            .Must(x => x <= DateTime.Today)
            .Message(x => "Child's date of birth cannot be in the future.");
}
