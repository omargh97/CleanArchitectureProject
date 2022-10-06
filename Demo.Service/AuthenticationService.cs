using Demo.BLL.Contracts;
using Demo.Entities;
using Demo.Helpers;
using Demo.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Demo.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private IGenericBLL<Users> _UserBL;

        public AuthenticationService(IGenericBLL<Users> UserBL)
        {
            _UserBL = UserBL;
        }

        public async Task<Users> Login(string Username, string Password)
        {
            Users user = await _UserBL.Get(x => x.UserName == Username);

            return user;
        }
    }
}

