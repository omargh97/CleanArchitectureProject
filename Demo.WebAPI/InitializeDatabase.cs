using Demo.DAL.Context;
using Demo.Entities;
using System;

namespace Demo.WebAPI
{
    public class InitializeDatabase
    {
        public static void Initialize(DemoDbContext context)
        {
            Users users = new Users
            {
                CreatedDate = DateTime.Now,
                FirstName = "FirstName",
                LastName = "LastName",
                UserName = "username",
                Role = Enums.Roles.User,
                //Password = BCrypt.Net.BCrypt.HashPassword("Username@2021")
            };

        }
    }
}
