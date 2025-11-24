using System.Collections;

namespace Mert1s.MyValidator;

internal partial class ValidationRule<TInstance, TProperty> : IValidationRule<TInstance>
{
    public Func<TInstance, TProperty> PropertySelector { get; } = default!;
    public Func<TProperty, TInstance, bool> Condition { get; } = default!;
    public Func<TProperty, TInstance, string> ErrorMessageFunc { get; set; } = default!;
    public INestedValidator NestedValidator { get; set; } = default!;

    public string PathName { get; set; }
    public CascadeMode? CascadeMode { get; set; }
    public Func<TInstance, bool>? When { get; set; }
    public Func<TInstance, CancellationToken, Task<bool>>? WhenAsync { get; set; }

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
        this.Condition = (property, instance) =>
        {
            try
            {
                return conditionCompiled(property);
            }
            catch (OperationCanceledException)
            {
                // Preserve cancellation
                throw;
            }
            catch (Exception ex)
            {
                // Log diagnostic information to aid debugging
                try
                {
                    var propValue = property == null ? "<null>" : property?.ToString() ?? "<unprintable>";
                    Console.WriteLine($"[MyValidator][Condition Exception] Path='{this.PathName}' PropertyValue='{propValue}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
                }
                catch { /* swallow any logging exceptions */ }

                // On exception, treat the condition as failed
                return false;
            }
        };
    }

    public ValidationRule(
    Expression<Func<TInstance, TProperty>> propertySelector,
    INestedValidator nestedValidator
   ) : this(propertySelector, (_, _) => true) =>
    this.NestedValidator = nestedValidator;

    public string GetErrorMessage(TInstance instance)
    {
        var property = this.PropertySelector.Invoke(instance);
        if (this.ErrorMessageFunc == null)
            return "Erro de validação.";
        var msg = this.ErrorMessageFunc.Invoke(property, instance);
        return msg ?? "Erro de validação.";
    }

    public ValidationResult Validate(TInstance instance)
    {
        var result = new ValidationResult();

        var value = this.PropertySelector(instance);

        // Check conditional predicates (When / WhenAsync)
        if (this.When != null)
        {
            bool whenResult = false;
            try
            {
                whenResult = this.When(instance);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyValidator][When Exception] Path='{this.PathName}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
                // If When throws, skip the rule
                whenResult = false;
            }

            if (!whenResult)
                return result;
        }

        if (this.WhenAsync != null)
        {
            var shouldRun = false;
            try
            {
                shouldRun = this.WhenAsync.Invoke(instance, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyValidator][WhenAsync Exception] Path='{this.PathName}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
                shouldRun = false;
            }

            if (!shouldRun)
                return result;
        }

        bool conditionOk = false;
        try
        {
            // Log property before evaluating condition for tracing
            try
            {
                var propStr = value == null ? "<null>" : value?.ToString() ?? "<unprintable>";
                Console.WriteLine($"[MyValidator][Condition Evaluate] Path='{this.PathName}' PropertyValue='{propStr}'");
            }
            catch { }

            conditionOk = this.Condition(value, instance);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MyValidator][Condition Threw] Path='{this.PathName}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
            conditionOk = false;
        }

        if (!conditionOk)
            result.AddError(this.PathName, this.GetErrorMessage(instance));

        if (this.NestedValidator != null && value != null)
        {
            try
            {
                this.ValidateNested(value, result);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyValidator][Nested Validate Exception] Path='{this.PathName}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
            }
        }

        return result;
    }

    public async Task<ValidationResult> ValidateAsync(TInstance instance, CancellationToken cancellation = default)
    {
        var result = new ValidationResult();

        var value = this.PropertySelector(instance);

        if (this.When != null)
        {
            bool whenResult = false;
            try
            {
                whenResult = this.When(instance);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyValidator][When Exception] Path='{this.PathName}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
                whenResult = false;
            }

            if (!whenResult)
                return result;
        }

        if (this.WhenAsync != null)
        {
            var shouldRun = false;
            try
            {
                shouldRun = await this.WhenAsync.Invoke(instance, cancellation).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyValidator][WhenAsync Exception] Path='{this.PathName}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
                shouldRun = false;
            }

            if (!shouldRun)
                return result;
        }

        bool conditionOk = false;
        try
        {
            try
            {
                var propStr = value == null ? "<null>" : value?.ToString() ?? "<unprintable>";
                Console.WriteLine($"[MyValidator][Condition Evaluate Async] Path='{this.PathName}' PropertyValue='{propStr}'");
            }
            catch { }

            conditionOk = this.Condition(value, instance);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MyValidator][Condition Threw Async] Path='{this.PathName}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
            conditionOk = false;
        }

        if (!conditionOk)
            result.AddError(this.PathName, this.GetErrorMessage(instance));

        if (this.NestedValidator != null && value != null)
        {
            try
            {
                await this.ValidateNestedAsync(value, result, cancellation).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyValidator][Nested Validate Async Exception] Path='{this.PathName}' Exception='{ex.GetType().FullName}: {ex.Message}'\n{ex.StackTrace}");
            }
        }

        return result;
    }

    private void ValidateNested(object value, ValidationResult result)
    {
        if (value is System.Collections.IEnumerable list)
        {
            var i = 0;
            foreach (var item in list)
            {
                var nestedResult = this.NestedValidator.Validate(item);
                result.Merge(this.PathName, $"{this.PathName}[{i}]", nestedResult);
                i++;
            }
        }
        else
        {
            var nestedResult = this.NestedValidator.Validate(value);
            result.Merge(this.PathName, this.PathName, nestedResult);
        }
    }

    private async Task ValidateNestedAsync(object value, ValidationResult result, CancellationToken cancellation)
    {
        if (value is System.Collections.IEnumerable list)
        {
            var i = 0;
            foreach (var item in list)
            {
                var nestedResults = await this.NestedValidator.ValidateAsync(item, cancellation).ConfigureAwait(false);
                result.Merge(this.PathName, $"{this.PathName}[{i}]", nestedResults);
                i++;
            }
        }
        else
        {
            var nestedResult = await this.NestedValidator.ValidateAsync(value, cancellation).ConfigureAwait(false);
            result.Merge(this.PathName, this.PathName, nestedResult);
        }
    }

    public static string GetPropertyName(Expression<Func<TInstance, TProperty>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        // Typical case: member access like x => x.Prop
        if (expression.Body is MemberExpression memberExpression)
            return memberExpression.Member.Name;

        // Handle conversions / casts where the operand is a member access: x => (SomeType)x.Prop
        if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
            return operand.Member.Name;

        // If the expression targets the whole instance (x => x) or a cast of the parameter (x => (T)x), return empty path
        if (expression.Body is ParameterExpression)
            return string.Empty;

        if (expression.Body is UnaryExpression unary && unary.Operand is ParameterExpression)
            return string.Empty;

        throw new ArgumentException("A expressão fornecida não é válida.", nameof(expression));
    }
}
