namespace Mert1s.MyValidator;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extensões para validar coleções dentro de RuleBuilder.
/// </summary>
public static class RuleBuilderCollectionExtensions
{
    /// <summary>
    /// Garante que a coleção não seja nula e contenha ao menos um item.
    /// </summary>
    public static RuleBuilder<TInstance, IEnumerable<TItem>> NotEmpty<TInstance, TItem>(this RuleBuilder<TInstance, IEnumerable<TItem>> builder)
        => builder.Must(collection => collection != null && collection.Any(), (collection, _) => "Collection must contain at least one element");

    /// <summary>
    /// Garante que a coleção não seja nula e contenha ao menos um item. (suporta ICollection<T>)
    /// </summary>
    public static RuleBuilder<TInstance, ICollection<TItem>> NotEmpty<TInstance, TItem>(this RuleBuilder<TInstance, ICollection<TItem>> builder)
        => builder.Must(collection => collection != null && collection.Count > 0, (collection, _) => "Collection must contain at least one element");

    /// <summary>
    /// Garante que a coleção não seja nula e contenha ao menos um item. (suporta IList<T>)
    /// </summary>
    public static RuleBuilder<TInstance, IList<TItem>> NotEmptyList<TInstance, TItem>(this RuleBuilder<TInstance, IList<TItem>> builder)
        => builder.Must(collection => collection != null && collection.Count > 0, (collection, _) => "Collection must contain at least one element");

    /// <summary>
    /// Garante que a coleção não seja nula e contenha ao menos um item. (suporta IReadOnlyCollection<T>)
    /// </summary>
    public static RuleBuilder<TInstance, IReadOnlyCollection<TItem>> NotEmptyReadOnly<TInstance, TItem>(this RuleBuilder<TInstance, IReadOnlyCollection<TItem>> builder)
        => builder.Must(collection => collection != null && collection.Count > 0, (collection, _) => "Collection must contain at least one element");

    // ---- Overloads specifically for List<TItem> to help extension resolution when concrete type is List<T> ----

    /// <summary>
    /// Garante que a lista não seja nula e contenha ao menos um item. (suporta List<T>)
    /// </summary>
    public static RuleBuilder<TInstance, List<TItem>> NotEmpty<TInstance, TItem>(this RuleBuilder<TInstance, List<TItem>> builder)
        => builder.Must(collection => collection != null && collection.Count > 0, (collection, _) => "Collection must contain at least one element");

    /// <summary>
    /// Garante que a lista contenha ao menos `count` elementos. (suporta List<T>)
    /// </summary>
    public static RuleBuilder<TInstance, List<TItem>> HasAtLeast<TInstance, TItem>(this RuleBuilder<TInstance, List<TItem>> builder, int count)
        => builder.Must(collection => collection != null && collection.Count >= count, (collection, _) => $"Collection must contain at least {count} elements");

    /// <summary>
    /// Garante que a lista contenha um número de elementos dentro do intervalo fechado [min, max]. (suporta List<T>)
    /// </summary>
    public static RuleBuilder<TInstance, List<TItem>> HasCountBetween<TInstance, TItem>(this RuleBuilder<TInstance, List<TItem>> builder, int min, int max)
        => builder.Must(collection => collection != null && collection.Count >= min && collection.Count <= max, (collection, _) => $"Collection must contain between {min} and {max} elements");

    // ---- Overloads specifically for HashSet<TItem> to help extension resolution when concrete type is HashSet<T> ----

    /// <summary>
    /// Garante que o HashSet não seja nulo e contenha ao menos um item. (suporta HashSet<T>)
    /// </summary>
    public static RuleBuilder<TInstance, HashSet<TItem>> NotEmpty<TInstance, TItem>(this RuleBuilder<TInstance, HashSet<TItem>> builder)
        => builder.Must(collection => collection != null && collection.Count > 0, (collection, _) => "Collection must contain at least one element");

    /// <summary>
    /// Garante que o HashSet contenha ao menos `count` elementos. (suporta HashSet<T>)
    /// </summary>
    public static RuleBuilder<TInstance, HashSet<TItem>> HasAtLeast<TInstance, TItem>(this RuleBuilder<TInstance, HashSet<TItem>> builder, int count)
        => builder.Must(collection => collection != null && collection.Count >= count, (collection, _) => $"Collection must contain at least {count} elements");

    /// <summary>
    /// Garante que o HashSet contenha um número de elementos dentro do intervalo fechado [min, max]. (suporta HashSet<T>)
    /// </summary>
    public static RuleBuilder<TInstance, HashSet<TItem>> HasCountBetween<TInstance, TItem>(this RuleBuilder<TInstance, HashSet<TItem>> builder, int min, int max)
        => builder.Must(collection => collection != null && collection.Count >= min && collection.Count <= max, (collection, _) => $"Collection must contain between {min} and {max} elements");

    /// <summary>
    /// Garante que o array não seja nulo e contenha ao menos um item. (suporta T[])
    /// </summary>
    public static RuleBuilder<TInstance, TItem[]> NotEmpty<TInstance, TItem>(this RuleBuilder<TInstance, TItem[]> builder)
        => builder.Must(collection => collection != null && collection.Length > 0, (collection, _) => "Collection must contain at least one element");

    /// <summary>
    /// Garante que a coleção ReadOnlyList não seja nula e contenha ao menos um item. (suporta IReadOnlyList<T>)
    /// </summary>
    public static RuleBuilder<TInstance, IReadOnlyList<TItem>> NotEmpty<TInstance, TItem>(this RuleBuilder<TInstance, IReadOnlyList<TItem>> builder)
        => builder.Must(collection => collection != null && collection.Count > 0, (collection, _) => "Collection must contain at least one element");

    /// <summary>
    /// Garante que o ISet não seja nulo e contenha ao menos um item. (suporta ISet<T>)
    /// </summary>
    public static RuleBuilder<TInstance, ISet<TItem>> NotEmpty<TInstance, TItem>(this RuleBuilder<TInstance, ISet<TItem>> builder)
        => builder.Must(collection => collection != null && collection.Count > 0, (collection, _) => "Collection must contain at least one element");

    /// <summary>
    /// Garante que a coleção contenha ao menos `count` elementos.
    /// </summary>
    public static RuleBuilder<TInstance, IEnumerable<TItem>> HasAtLeast<TInstance, TItem>(this RuleBuilder<TInstance, IEnumerable<TItem>> builder, int count)
        => builder.Must(collection => collection != null && collection.Count() >= count, (collection, _) => $"Collection must contain at least {count} elements");

    /// <summary>
    /// Garante que a coleção contenha ao menos `count` elementos. (suporta ICollection<T>)
    /// </summary>
    public static RuleBuilder<TInstance, ICollection<TItem>> HasAtLeast<TInstance, TItem>(this RuleBuilder<TInstance, ICollection<TItem>> builder, int count)
        => builder.Must(collection => collection != null && collection.Count >= count, (collection, _) => $"Collection must contain at least {count} elements");

    /// <summary>
    /// Garante que a coleção contenha ao menos `count` elementos. (suporta IList<T>)
    /// </summary>
    public static RuleBuilder<TInstance, IList<TItem>> HasAtLeastList<TInstance, TItem>(this RuleBuilder<TInstance, IList<TItem>> builder, int count)
        => builder.Must(collection => collection != null && collection.Count >= count, (collection, _) => $"Collection must contain at least {count} elements");

    /// <summary>
    /// Garante que a coleção contenha ao menos `count` elementos. (suporta IReadOnlyCollection<T>)
    /// </summary>
    public static RuleBuilder<TInstance, IReadOnlyCollection<TItem>> HasAtLeastReadOnly<TInstance, TItem>(this RuleBuilder<TInstance, IReadOnlyCollection<TItem>> builder, int count)
        => builder.Must(collection => collection != null && collection.Count >= count, (collection, _) => $"Collection must contain at least {count} elements");

    /// <summary>
    /// Garante que a coleção contenha um número de elementos dentro do intervalo fechado [min, max].
    /// </summary>
    public static RuleBuilder<TInstance, IEnumerable<TItem>> HasCountBetween<TInstance, TItem>(this RuleBuilder<TInstance, IEnumerable<TItem>> builder, int min, int max)
        => builder.Must(collection => collection != null && collection.Count() >= min && collection.Count() <= max, (collection, _) => $"Collection must contain between {min} and {max} elements");

    /// <summary>
    /// Garante que a coleção contenha um número de elementos dentro do intervalo fechado [min, max]. (suporta ICollection<T>)
    /// </summary>
    public static RuleBuilder<TInstance, ICollection<TItem>> HasCountBetween<TInstance, TItem>(this RuleBuilder<TInstance, ICollection<TItem>> builder, int min, int max)
        => builder.Must(collection => collection != null && collection.Count >= min && collection.Count <= max, (collection, _) => $"Collection must contain between {min} and {max} elements");

    /// <summary>
    /// Garante que a coleção contenha um número de elementos dentro do intervalo fechado [min, max]. (suporta IList<T>)
    /// </summary>
    public static RuleBuilder<TInstance, IList<TItem>> HasCountBetweenList<TInstance, TItem>(this RuleBuilder<TInstance, IList<TItem>> builder, int min, int max)
        => builder.Must(collection => collection != null && collection.Count >= min && collection.Count <= max, (collection, _) => $"Collection must contain between {min} and {max} elements");

    /// <summary>
    /// Garante que a coleção contenha um número de elementos dentro do intervalo fechado [min, max]. (suporta IReadOnlyCollection<T>)
    /// </summary>
    public static RuleBuilder<TInstance, IReadOnlyCollection<TItem>> HasCountBetweenReadOnly<TInstance, TItem>(this RuleBuilder<TInstance, IReadOnlyCollection<TItem>> builder, int min, int max)
        => builder.Must(collection => collection != null && collection.Count >= min && collection.Count <= max, (collection, _) => $"Collection must contain between {min} and {max} elements");
}
