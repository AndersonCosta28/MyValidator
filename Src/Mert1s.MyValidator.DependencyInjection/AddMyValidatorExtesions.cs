namespace Mert1s.MyValidator.DependencyInjection;

public static class AddMyValidatorExtesions
{
    public static IServiceCollection AddMyValidator(
            this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            var validationTypes = types.Where(IsType);

            foreach (var validationType in validationTypes)
            {
                if (validationType.BaseType is null)
                    continue;
                var typeGeneric = validationType.BaseType.GenericTypeArguments[0];
                var validationBuilder = typeof(ValidatorBuilder<>).MakeGenericType(typeGeneric);

                services.AddTransient(validationBuilder, validationType);
            }
        }

        return services;
    }

    public static IServiceCollection AddMyValidator(
    this IServiceCollection services, params IEnumerable<Type> validationTypes)
    {
        foreach (var validationType in validationTypes)
            services.AddTransient(typeof(ValidatorBuilder<>), validationType);

        return services;
    }

    private static bool IsType(Type type)
    {
        var baseType = type.BaseType;

        if (baseType is null)
            return false;

        if (!baseType.IsGenericType)
            return false;

        try
        {
            if (baseType.GetGenericTypeDefinition() != typeof(ValidatorBuilder<>))
                return false;

            if (type.IsAbstract)
                return false;

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}
