using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Entities;

namespace Demo.IService
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetAll();
        Task<Users> GetById(int id);
        Task<bool> Delete(Users U);
        Task<bool> Update(Users U);
        Task<bool> Add(Users U);
    }
}

