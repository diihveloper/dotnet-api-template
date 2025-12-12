using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using DiihTemplate.Core.Attributes;
using DiihTemplate.Core.Entities;

namespace DiihTemplate.Core.Repositories;

public static class QueryableExtensions
{
    /// <summary>
    ///  </summary>
    /// <param name="query"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> PageBy<T>([NotNull] this IQueryable<T> query, int page, int perPage)
    {
        page = page < 1 ? 1 : page;
        perPage = perPage < 1 ? 1 : perPage;
        return query.Skip((page - 1) * perPage).Take(perPage);
    }

    /// <summary>
    /// Used for paging. Can be used as an alternative to Skip(...).Take(...) chaining.
    /// </summary>
    public static TQueryable PageBy<T, TQueryable>([NotNull] this TQueryable query, int page, int perPage)
        where TQueryable : IQueryable<T>
    {
        page = page < 1 ? 1 : page;
        perPage = perPage < 1 ? 1 : perPage;
        return (TQueryable)query.Skip((page - 1) * perPage).Take(perPage);
    }

    /// <summary>
    /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static IQueryable<T> WhereIf<T>([NotNull] this IQueryable<T> query, bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }

    /// <summary>
    /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static TQueryable WhereIf<T, TQueryable>([NotNull] this TQueryable query, bool condition,
        Expression<Func<T, bool>> predicate)
        where TQueryable : IQueryable<T>
    {
        return condition
            ? (TQueryable)query.Where(predicate)
            : query;
    }

    /// <summary>
    /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static IQueryable<T> WhereIf<T>([NotNull] this IQueryable<T> query, bool condition,
        Expression<Func<T, int, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }

    /// <summary>
    /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static TQueryable WhereIf<T, TQueryable>([NotNull] this TQueryable query, bool condition,
        Expression<Func<T, int, bool>> predicate)
        where TQueryable : IQueryable<T>
    {
        return condition
            ? (TQueryable)query.Where(predicate)
            : query;
    }

    public static IQueryable<T> Search<T>(this IQueryable<T> query, string term) where T : class
    {
        if (string.IsNullOrWhiteSpace(term))
            return query;

        var searchableProps = typeof(T)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<SearchableAttribute>() != null && p.PropertyType == typeof(string))
            .ToList();

        if (!searchableProps.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var efFunctions = Expression.Property(null, typeof(EF).GetProperty(nameof(EF.Functions))!);

        Expression? finalExpression = null;

        // Termo em lowercase
        var pattern = $"%{term.ToLower()}%";

        // Referência ao método string.ToLower()
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;

        // Referência ao método EF.Functions.Like()
        var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
            nameof(DbFunctionsExtensions.Like),
            new[] { typeof(DbFunctions), typeof(string), typeof(string) }
        )!;

        foreach (var prop in searchableProps)
        {
            var property = Expression.Property(parameter, prop.Name);

            // x.Prop.ToLower()
            var loweredProperty = Expression.Call(property, toLowerMethod);

            // EF.Functions.Like(x.Prop.ToLower(), pattern)
            var likeCall = Expression.Call(
                likeMethod,
                efFunctions,
                loweredProperty,
                Expression.Constant(pattern)
            );

            finalExpression = finalExpression == null
                ? likeCall
                : Expression.OrElse(finalExpression, likeCall);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(finalExpression!, parameter);
        return query.Where(lambda);
    }

    public static async Task<PagedResult<TSource>> ToPagedAsync<TSource>(
        this IQueryable<TSource> source,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var list = new List<TSource>();
        page = page < 1 ? 1 : page;
        perPage = perPage < 1 ? 1 : perPage;
        await foreach (var element in source.PageBy<TSource>(page, perPage).AsAsyncEnumerable()
                           .WithCancellation(cancellationToken))
        {
            list.Add(element);
        }

        return new PagedResult<TSource>(list, count, page, perPage);
    }
}