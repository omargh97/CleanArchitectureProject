using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Demo.BLL.Contracts;
using Demo.Entities;
using Demo.IService;


namespace Demo.Service
{
    public class UserService : IUserService
    {
        private IGenericBLL<Users> _UserBL;

        public UserService(IGenericBLL<Users> UserBL)
        {
            _UserBL = UserBL;
        }

        public async Task<bool> Add(Users U)
        {
            U.CreatedDate = DateTime.Now;
            await _UserBL.Add(U);
            return true;
        }

        public async Task<bool> Delete(Users U)
        {
            Users Users = await _UserBL.Get(x => x.UserName == U.UserName);

            if (Users != null)
            {
                return false;
            }
            await _UserBL.Delete(U);
            return true;
        }

        public async Task<IEnumerable<Users>> GetAll()
        {
            return await _UserBL.GetMany();
        }
        public async Task<Users> GetById(int id)
        {
            return await _UserBL.GetById(id);
        }

        public async Task<bool> Update(Users U)
        {
            Users Users = await _UserBL.Get(x => x.UserName == U.UserName);

            if (Users != null)
            {
                return false;
            }
            await _UserBL.Update(U);
            return true;
        }
    }
}

