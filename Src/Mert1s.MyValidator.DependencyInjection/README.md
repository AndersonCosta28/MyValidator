# Mert1s.MyValidator.DependencyInjection

Integration helpers to register MyValidator services with Microsoft DI.

This small package contains extension methods to wire up `Mert1s.MyValidator` validators into an ASP.NET Core application's `IServiceCollection`.

Usage
--
```csharp
// in Startup.cs or Program.cs (minimal host)
services.AddMyValidator();

// Optionally register specific validators
services.AddSingleton<IValidator<MyModel>, MyModelValidator>();
```

Packaging
--
This project includes a `README.md` and will be packed with the NuGet package when `dotnet pack` is executed. If an `icon.png` file is present in the project folder it will be included as the package icon.

License
--
MIT
