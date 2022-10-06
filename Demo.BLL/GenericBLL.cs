using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Demo.BLL.Contracts;
using Demo.DAL.Contracts;

namespace Demo.BLL
{
    public class GenericBLL<T> : IGenericBLL<T> where T : class, new()
    {
        IUnitOfWork _unitOfWork;
        IRepository<T> _repo;
        public GenericBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repo = (IRepository<T>)_unitOfWork.GetRepository<T>();
        }
        public async Task Add(T entity)
        {
            await _repo.Add(entity);
        }

        public async Task Delete(T entity)
        {
            await _repo.Delete(entity);
        }

        public async Task DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            await _repo.DeleteWhere(predicate);
        }

        public async Task<T> GetById(params object[] id)
        {
            return await _repo.GetById(id);
        }

        public async Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            return await _repo.GetMany(predicate, includeProperties);
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            return await _repo.Get(predicate, includeProperties);
        }

        public async Task Update(T entity)
        {
            await _repo.Update(entity);
        }
    }
}

