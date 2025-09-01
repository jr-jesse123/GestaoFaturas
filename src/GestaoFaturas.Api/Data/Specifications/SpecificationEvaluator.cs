using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Data.Specifications;

public static class SpecificationEvaluator<TEntity> where TEntity : class
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specification, bool evaluateOnlyExpression = false)
    {
        var query = inputQuery;

        // Apply filtering
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // For count operations, we only need the filtering criteria
        if (evaluateOnlyExpression)
        {
            return query;
        }

        // Apply includes
        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply string-based includes
        query = specification.IncludeStrings
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply paging
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip)
                         .Take(specification.Take);
        }

        return query;
    }
}