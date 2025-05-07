using API.Interfaces;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Specifications
{
    public class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(
            IQueryable<TEntity> inputQuery,
            ISpecification<TEntity> specification
            )
        {
            var query = inputQuery;

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            query = specification.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            query = specification.IncludeStrings.Aggregate(query,
                (current, include) => current.Include(include));

            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.GroupBy != null)
            {
                query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
            }

            if (specification.isPagingEnable)
            {
                query = query.Skip(specification.Skip.Value)
                    .Take(specification.Take.Value);
            }

            return query;
        }

        public static IQueryable<TResult> GetQuery<TResult>(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec, AutoMapper.IConfigurationProvider config)
        {
            var query = GetQuery(inputQuery, spec);
            return query.ProjectTo<TResult>(config);
        }
    }
}
