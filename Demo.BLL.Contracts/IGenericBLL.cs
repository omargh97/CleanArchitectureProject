using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Demo.BLL.Contracts
{
    public interface IGenericBLL<T> where T : class, new()
    {
        Task Add(T entity);

        Task Delete(T entity);

        Task DeleteWhere(Expression<Func<T, bool>> predicate);

        Task<T> GetById(params object[] id);

        Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        Task<T> Get(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        Task Update(T entity);
    }
}

