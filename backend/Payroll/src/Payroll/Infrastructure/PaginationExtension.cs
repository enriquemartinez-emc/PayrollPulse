using System.Linq.Expressions;

namespace Payroll.Infrastructure;

public static class PaginationExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        PaginationQuery pagination,
        CancellationToken ct = default
    )
    {
        // Dynamic sorting
        if (!string.IsNullOrWhiteSpace(pagination.SortBy))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, pagination.SortBy);
            var lambda = Expression.Lambda(property, parameter);

            string methodName =
                pagination.SortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                [typeof(T), property.Type],
                query.Expression,
                Expression.Quote(lambda)
            );

            query = query.Provider.CreateQuery<T>(resultExpression);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        return PaginatedList<T>.Create(
            items,
            totalCount,
            pagination.PageNumber,
            pagination.PageSize
        );
    }
}
