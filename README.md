# MyValidator

MyValidator is a .NET library designed to simplify and enhance data validation in your applications. It provides a robust and extensible framework for validating user input, ensuring data integrity, and reducing boilerplate code.

## Features

- **Customizable Rules**: Easily define and apply custom validation rules.
- **Built-in Validators**: Includes a variety of pre-built validators for common use cases.
- **Extensibility**: Create your own validation logic by extending the framework.
- **Error Handling**: Provides detailed error messages for invalid inputs.
- **Lightweight**: Minimal dependencies and optimized for performance.

## Installation

To install MyValidator, use the NuGet Package Manager:

```bash
dotnet add package MyValidator --source "https://nuget.pkg.github.com/AndersonCosta28/index.json"
```

OR
```bash
dotnet nuget add source "https://nuget.pkg.github.com/AndersonCosta28/index.json" --name "AndersonCosta28Source"
dotnet add package MyValidator
```

## Usage

Here’s a quick example of how to use MyValidator:

```csharp
using MyValidator;

var validator = new Validator();
validator.AddRule("Name", value => !string.IsNullOrEmpty(value), "Name cannot be empty");
validator.AddRule("Age", value => int.TryParse(value, out var age) && age > 18, "Age must be greater than 18");

var result = validator.Validate(new Dictionary<string, string>
{
    { "Name", "John" },
    { "Age", "25" }
});

if (result.IsValid)
{
    Console.WriteLine("Validation passed!");
}
else
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.Key}: {error.Message}");
    }
}
```

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests to improve the library.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For questions or feedback, please contact [your-email@example.com].