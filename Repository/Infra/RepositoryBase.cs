using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Invitee.Repository.Infra
{
    public abstract class RepositoryBase<T> : IRepository<T>, IDisposable where T : class
    {
        protected RepositoryContext RepositoryContext { get; set; }

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll(bool tracking=false)
        {
            return tracking?this.RepositoryContext.Set<T>(): this.RepositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>().Where(expression).AsNoTracking();
        }

        public T Create(T entity)
        {
            return this.RepositoryContext.Set<T>().Add(entity);
        }

        public T Update(T entity)
        {
            if (this.RepositoryContext.Entry(entity).State == EntityState.Detached)
                this.RepositoryContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public void Delete(T entity)
        {
            if (this.RepositoryContext.Entry(entity).State == EntityState.Detached)
                this.RepositoryContext.Entry(entity).State = EntityState.Deleted;
            this.RepositoryContext.Set<T>().Remove(entity);
        }

        public void Dispose()
        {
            RepositoryContext.Dispose();
        }
    }
}