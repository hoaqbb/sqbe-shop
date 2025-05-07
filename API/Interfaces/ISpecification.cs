using System.Linq.Expressions;

namespace API.Interfaces
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>>? Criteria { get; }
        List<Expression<Func<T, object>>>? Includes { get; }
        List<string>? IncludeStrings { get; }
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
        Expression<Func<T, object>>? GroupBy { get; }
        IQueryable<T> ApplyCriteria(IQueryable<T> query);
        int? Take { get; }
        int? Skip { get; }
        bool isPagingEnable { get; }
    }
}
