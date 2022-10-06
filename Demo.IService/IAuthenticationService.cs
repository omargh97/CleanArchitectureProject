using Demo.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.IService
{
    public interface IAuthenticationService
    {
        Task<Users> Login(string Username, string Password);
    }
}

