# Mert1s.MyValidator

Lightweight, extensible validation library for .NET

Overview
--
`Mert1s.MyValidator` is a small validation library for .NET (target: `net9.0`) that makes it easy to declare strongly-typed validation rules using builders. The primary API allows you to create validators by subclassing `ValidatorBuilder<T>` and chaining rules using `RuleFor` and `RulesFor`.

Key features
--
- Fluent rule definition via `RuleFor` and `RulesFor`.
- Customizable error messages via `Message`.
- Synchronous and asynchronous rules (`Must` / `MustAsync`).
- Nested validators and collection validation.
- Result types: `ValidationResult` and `ValidationError`.

Package info
--
- `PackageId`: `Mert1s.MyValidator`
- `TargetFramework`: `net9.0`
- `Version` in project: `1.0.4.3`

Installation
--
From NuGet (when published):

```powershell
dotnet add package Mert1s.MyValidator
```

Or build locally from the solution:

```powershell
dotnet build Mert1s.MyValidator.sln -c Release
```

Quickstart
--
Minimal example showing how to create a validator:

```csharp
using Mert1s.MyValidator;

public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class PersonValidator : ValidatorBuilder<Person>
{
    public PersonValidator()
    {
        this.RuleFor(x => x.Name)
            .NotNullOrWhiteSpace()
            .Message("Name is required");

        this.RuleFor(x => x.Age)
            .GreaterThanOrEqual(18)
            .Message((age, _) => $"Age must be >= 18. Value: {age}");
    }
}

// Usage
var validator = new PersonValidator();
var results = validator.Validate(new Person { Name = "", Age = 16 });

foreach (var r in results)
{
    foreach (var err in r.Errors)
        Console.WriteLine($"{err.Path}: {err.Message}");
}
```

Cascade behavior (CascadeMode)
--
This library provides a `CascadeMode` setting to control whether additional rules for the same property should be executed after the first failure.

- Global default (validator-level): set `ValidatorBuilder<T>.CascadeMode` (default is `CascadeMode.Continue`).
- Per-property override: call `SetCascadeMode(CascadeMode)` on the `RuleBuilder` returned by `RuleFor(...)`.

Examples:

1) Global `Stop` (stop after first failing rule per property):

```csharp
var validator = new PersonValidator();
validator.CascadeMode = CascadeMode.Stop;

// For a rule chain under the same property, only the first failing rule will report an error.
```

2) Per-property override (continue despite global Stop):

```csharp
public class ExampleValidator : ValidatorBuilder<Person>
{
    public ExampleValidator()
    {
        // Global default remains Continue unless set on the validator
        this.RuleFor(x => x.Name)
            .SetCascadeMode(CascadeMode.Stop) // Stop for this property
            .Must(n => !string.IsNullOrEmpty(n)).Message("Name required")
            .Must(n => n.Length >= 3).Message("Name too short");

        this.RuleFor(x => x.Age)
            .SetCascadeMode(CascadeMode.Continue) // Ensure this property runs all rules
            .GreaterThanOrEqual(18).Message("Must be an adult")
            .LessThanOrEqual(120).Message("Unrealistic age");
    }
}
```

API and main concepts
--
- `ValidatorBuilder<T>`: base class to create a strongly-typed validator; exposes `RuleFor` and `RulesFor`.
- `RuleFor<TProperty>(Expression<Func<T, TProperty>>)` : starts a `RuleBuilder` for a property.
- `RulesFor<TItem>(Expression<Func<T, IEnumerable<TItem>>>)` : starts a `CollectionRuleBuilder` for collection validation.
- `RuleBuilder<TInstance, TProperty>`: chain conditions with `Must`, `MustAsync`, messages (`Message`) and `SetValidator` for nested validators; also `SetCascadeMode` to override cascade for that property.
- `CollectionRuleBuilder<TInstance, TProperty>`: validate collections or set a validator for each item (`SetValidator`).
- `ValidationResult`: contains `List<ValidationError> Errors` and `IsValid`.
- `ValidationError`: contains `PropertyName`, `Path` and `Message`.

Advanced examples
--
1) Collection validation with nested validator:

```csharp
public class Item { public int Quantity { get; set; } }

public class ItemValidator : ValidatorBuilder<Item>
{
    public ItemValidator() => this.RuleFor(x => x.Quantity).GreaterThan(0);
}

public class Order { public IEnumerable<Item> Items { get; set; } = Array.Empty<Item>(); }

public class OrderValidator : ValidatorBuilder<Order>
{
    public OrderValidator() => this.RulesFor(x => x.Items).SetValidator(new ItemValidator());
}

var orderValidator = new OrderValidator();
var orderResults = orderValidator.Validate(new Order { Items = new[] { new Item { Quantity = 0 } } });
// `orderResults` will contain merged errors coming from ItemValidator with proper paths
```

2) Asynchronous rule:

```csharp
this.RuleFor(x => x.SomeValue).MustAsync(async (value, ct) =>
{
    await Task.Delay(1, ct);
    return value != null; 
});
```

Reading results
--
`ValidatorBuilder<T>.Validate` returns `List<ValidationResult>`. Each `ValidationResult` contains the `Errors` collection with `ValidationError` objects which include `Path` and `Message`.

Running tests
--
Unit tests demonstrate usage of many extensions (for example, `RuleBuilderStringExtensionsTests`). To run all tests:

```powershell
dotnet test Tests\UnitTests\UnitTests.csproj
```

Contributing
--
- Open an issue to suggest improvements or report bugs.
- Send small, focused PRs; include tests when appropriate.

License
--
MIT License (see `LICENSE` in repository root).

Contact
--
Open an issue on GitHub: https://github.com/AndersonCosta28/MyValidator
