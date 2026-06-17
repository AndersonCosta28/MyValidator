using UnitTests.Models;

internal class FatherValidator : ValidatorBuilder<Father>
{
    public FatherValidator()
    {
        this.RuleFor(x => x.Wife)
            .NotNull()
            .SetValidator(new WifeValidator());

        this.RulesFor(x => x.Children)
            .Must(x => x.All(y => y.Gender == Gender.Feminino))
            .Message(x => "All children must be female.")
            .SetValidator(new ChildrenValidator());
    }
}