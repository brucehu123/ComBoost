﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class EntityMappedContext<T, M> : IEntityContext<T>
        where M : IEntity, T
        where T : IEntity
    {
        public EntityMappedContext(IEntityContext<M> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            InnerContext = context;
        }

        public IEntityContext<M> InnerContext { get; private set; }

        public IDatabaseContext Database { get { return InnerContext.Database; } }

        public IEntityMetadata Metadata { get { return InnerContext.Metadata; } }

        public void Add(T item)
        {
            InnerContext.Add((M)item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            InnerContext.AddRange((IEnumerable<M>)items);
        }

        protected virtual IQueryable<T> WrapQueryable(IQueryable<M> query)
        {
            
        }

        protected virtual IQueryable<M> UnwrapQueryable(IQueryable<T> query)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(IQueryable<T> query)
        {
            return InnerContext.CountAsync(UnwrapQueryable(query));
        }

        protected virtual Expression<Func<M, TResult>> WrapExpression<TResult>(Expression<Func<T, TResult>> expression)
        {
            var parameters = new ParameterExpression[] { Expression.Parameter(typeof(M)) };
            var newExpression = Expression.Lambda<Func<M, TResult>>(expression.Update(expression, parameters).Body, parameters);
            return newExpression;
        }

        public Task<int> CountAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            var newExpression = WrapExpression(expression);
            return InnerContext.CountAsync(UnwrapQueryable(query), newExpression);
        }

        public T Create()
        {
            return InnerContext.Create();
        }

        public async Task<T> FirstAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            var newExpression = WrapExpression(expression);
            return await InnerContext.FirstAsync(UnwrapQueryable(query), newExpression);
        }

        public async Task<T> FirstOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            var newExpression = WrapExpression(expression);
            return await InnerContext.FirstOrDefaultAsync(UnwrapQueryable(query), newExpression);
        }

        public async Task<T> LastAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            var newExpression = WrapExpression(expression);
            return await InnerContext.LastAsync(UnwrapQueryable(query), newExpression);
        }

        public async Task<T> LastOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            var newExpression = WrapExpression(expression);
            return await InnerContext.LastOrDefaultAsync(UnwrapQueryable(query), newExpression);
        }

        public IQueryable<T> Query()
        {
            return WrapQueryable(InnerContext.Query());
        }

        public void Remove(T item)
        {
            InnerContext.Remove((M)item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            InnerContext.RemoveRange((IEnumerable<M>)items);
        }

        public async Task<T> SingleAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            var newExpression = WrapExpression(expression);
            return await InnerContext.SingleAsync(UnwrapQueryable(query), newExpression);
        }

        public async Task<T> SingleOrDefaultAsync(IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            var newExpression = WrapExpression(expression);
            return await InnerContext.SingleOrDefaultAsync(UnwrapQueryable(query), newExpression);
        }

        public async Task<T[]> ToArrayAsync(IQueryable<T> query)
        {
            return (await InnerContext.ToArrayAsync(UnwrapQueryable(query))).Cast<T>().ToArray();
        }

        public async Task<List<T>> ToListAsync(IQueryable<T> query)
        {
            return (await InnerContext.ToListAsync(UnwrapQueryable(query))).Cast<T>().ToList();
        }

        public void Update(T item)
        {
            InnerContext.Update((M)item);
        }

        public void UpdateRange(IEnumerable<T> items)
        {
            InnerContext.UpdateRange((IEnumerable<M>)items);
        }
    }
}
