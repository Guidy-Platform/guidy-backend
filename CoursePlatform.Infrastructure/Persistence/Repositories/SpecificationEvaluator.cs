using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Persistence.Repositories;

public static class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(
        IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery;

        if (spec.IsNoTracking)
            query = query.AsNoTracking();

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        if (spec.IgnoreQueryFilters)
            query = query.IgnoreQueryFilters();


        query = spec.Includes
            .Aggregate(query, (q, include) => q.Include(include));

        query = spec.IncludeStrings
            .Aggregate(query, (q, include) => q.Include(include));

        if (spec.OrderBy is not null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDesc is not null)
            query = query.OrderByDescending(spec.OrderByDesc);

        if (spec.IsPagingEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        return query;
    }
}