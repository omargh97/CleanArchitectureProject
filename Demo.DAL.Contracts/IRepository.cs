using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Demo.DAL.Contracts
{
    public interface IRepository
    {
        void Submit();
    }

    public interface IRepository<T> : IRepository where T : class
    {
        Task<T> GetById(params object[] id);

        Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        Task<T> Get(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        Task Add(T entity);

        Task Update(T entity);

        Task Delete(T entity);

        Task DeleteWhere(Expression<Func<T, bool>> predicate);


    }
}

