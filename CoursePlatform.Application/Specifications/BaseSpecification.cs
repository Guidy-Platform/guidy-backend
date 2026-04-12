using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Common;
using System.Linq.Expressions;

namespace CoursePlatform.Application.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T> where T : BaseEntity
{
    protected BaseSpecification() { }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
        => Criteria = criteria;

    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public List<string> IncludeStrings { get; } = [];
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDesc { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }
    public bool IsNoTracking { get; private set; }
    public bool IgnoreQueryFilters { get; private set; }



    protected void AddCriteria(Expression<Func<T, bool>> criteria)
        => Criteria = criteria;

    protected void AddInclude(Expression<Func<T, object>> include)
        => Includes.Add(include);

    protected void AddInclude(string includeString)
        => IncludeStrings.Add(includeString);    // for "Sections.Lessons"

    protected void AddOrderBy(Expression<Func<T, object>> orderBy)
        => OrderBy = orderBy;

    protected void AddOrderByDesc(Expression<Func<T, object>> orderByDesc)
        => OrderByDesc = orderByDesc;

    protected void ApplyPaging(int pageIndex, int pageSize)
    {
        Skip = (pageIndex - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }

    protected void ApplyNoTracking()
        => IsNoTracking = true;
    protected void ApplyIgnoreQueryFilters()
    => IgnoreQueryFilters = true;
}