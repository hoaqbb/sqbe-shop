﻿using API.Interfaces;
using System.Linq.Expressions;
using System.Linq;

namespace API.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T> where T : class
    {
        public Expression<Func<T, bool>>? Criteria { get; set; }

        protected BaseSpecification(Expression<Func<T, bool>>? criteria)
        {
            Criteria = criteria;
        }

        public BaseSpecification()
        {

        }
        public List<Expression<Func<T, object>>>? Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public Expression<Func<T, object>>? GroupBy { get; private set; }

        public int? Take { get; private set; }
        public int? Skip { get; private set; }
        public bool isPagingEnable { get; private set; } = false;

        public IQueryable<T> ApplyCriteria(IQueryable<T> query)
        {
            if (Criteria != null)
            {
                query = query.Where(Criteria);
            }

            return query;
        }

        protected virtual void AddInclude(Expression<Func<T, object>>? includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected virtual void ApplyPaging(int? skip, int? take)
        {
            Skip = skip;
            Take = take;
            isPagingEnable = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>>? orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>>? orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected virtual void ApplyGroupBy(Expression<Func<T, object>>? groupByExpression)
        {
            GroupBy = groupByExpression;
        }
    }
}
