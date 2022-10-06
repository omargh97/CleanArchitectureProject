using Demo.BLL;
using Demo.BLL.Contracts;
using Demo.DAL.Context;
using Demo.DAL.Contracts;
using Demo.DAL.Repository;
using Demo.DAL.UnitOfWork;
using Demo.Entities;
using Demo.IService;
using Demo.Service;
using Demo.WebAPI.Infrastructure.JwtAuth;
using Demo.WebAPI.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.WebAPI.Extensions
{
    public class ScopedConfiguration
    {
        public static void ScopedConfig(IServiceCollection services)
        {
            // services.AddSingleton<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DemoDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Service
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();


            // BLL
            services.AddScoped<IGenericBLL<Users>, GenericBLL<Users>>();

            // Repositories
            services.AddScoped<IRepository<Users>, UserRepository>();
        }
    }
}
