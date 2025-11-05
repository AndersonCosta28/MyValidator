using Mert1s.MyValidator;

internal class FatherValidator : ValidatorBuilder<Father>
{
    public FatherValidator()
    {
        var ruleForWife = this.RuleFor(x => x.Wife);

        ruleForWife.SetValidator(new WifeValidator());

        this.RulesFor(x => x.Children)
            .Must(x => x.All(y => y.Gender == Gender.Feminino))
            .Message(x => "All children must be female.")
            .SetValidator(new ChildrenValidator());
    }
}