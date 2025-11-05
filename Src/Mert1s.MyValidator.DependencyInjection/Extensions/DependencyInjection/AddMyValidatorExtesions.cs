namespace Mert1s.MyValidator.AspNetCore.Extensions.DependencyInjection;

public static class AddMyValidatorExtesions
{
    public static IServiceCollection AddMyValidator(
            this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            var validationTypes = types.Where(isType).ToList();

            foreach (var validationType in validationTypes)
            {
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

    private static bool isType(Type type)
    {
        var name = type.FullName;
        var isGenericType = type.IsGenericType;
        if (isGenericType)
            return false;
        try
        {
            var isGenericTypeDefinition = type.BaseType is not null && type.BaseType?.GetGenericTypeDefinition() == typeof(ValidatorBuilder<>);
            return !isGenericType && isGenericTypeDefinition && !type.IsAbstract;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}
