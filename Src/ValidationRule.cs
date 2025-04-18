namespace MyValidator;

internal partial class ValidationRule<TInstance, TProperty> : IValidationRule<TInstance>
{
    public Func<TInstance, TProperty> PropertySelector { get; }
    public Func<TProperty, TInstance, bool> Condition { get; }
    public Func<TProperty, TInstance, string> ErrorMessageFunc { get; set; }
    public INestedValidator NestedValidator { get; set; }

    public string PathName { get; set; }

    public ValidationRule(
        Expression<Func<TInstance, TProperty>> propertySelector,
        Expression<Func<TProperty, TInstance, bool>> condition
        )
    {
        this.PathName = GetPropertyName(propertySelector);
        this.PropertySelector = propertySelector.Compile();
        this.Condition = condition.Compile();
    }

    public ValidationRule(
        Expression<Func<TInstance, TProperty>> propertySelector,
        Expression<Func<TProperty, bool>> condition
    )
    {
        this.PathName = GetPropertyName(propertySelector);
        this.PropertySelector = propertySelector.Compile();

        var conditionCompiled = condition.Compile();
        this.Condition = (property, instance) => conditionCompiled(property);
    }

    public ValidationRule(
    Expression<Func<TInstance, TProperty>> propertySelector,
    INestedValidator nestedValidator
) : this(propertySelector, (_, _) => true) =>
    this.NestedValidator = nestedValidator;

    public string GetErrorMessage(TInstance instance)
    {
        var property = this.PropertySelector.Invoke(instance);
        return this.ErrorMessageFunc.Invoke(property, instance) ?? "Erro de validação.";
    }

    public ValidationResult Validate(TInstance instance)
    {
        var result = new ValidationResult();

        var value = this.PropertySelector(instance);

        if (!this.Condition(value, instance))
            result.AddError(this.PathName, this.GetErrorMessage(instance));

        if (this.NestedValidator != null && value != null)
            this.ValidateNested(value, result);


        return result;
    }

    private void ValidateNested(object value, ValidationResult result)
    {
        if (value is IEnumerable<object> list)
        {
            var i = 0;
            foreach (var item in list)
            {
                var nestedResult = this.NestedValidator.ValidateWithResult(item);
                result.Merge($"{this.PathName}[{i}]", nestedResult);
                i++;
            }
        }
        else
        {
            var nestedResult = this.NestedValidator.ValidateWithResult(value);
            result.Merge(this.PathName, nestedResult);
        }
    }

    public static string GetPropertyName(Expression<Func<TInstance, TProperty>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (expression.Body is MemberExpression memberExpression)
            return memberExpression.Member.Name;

        if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
            return operand.Member.Name;

        throw new ArgumentException("A expressão fornecida não é válida.", nameof(expression));
    }
}
