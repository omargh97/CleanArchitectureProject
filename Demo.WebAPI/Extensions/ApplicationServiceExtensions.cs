using Demo.DAL.Context;
using Demo.WebAPI.Infrastructure.CustomErrorResponse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

namespace Demo.WebAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationsServices(this IServiceCollection services, IConfiguration configuration)
        {
           
            // versioning of Api
            services.AddApiVersioning(options => {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                // The format of the version added to the route URL  
                options.GroupNameFormat = "'v'VVV";
                // Tells swagger to replace the version in the controller route  
                options.SubstituteApiVersionInUrl = true;
            }
            );

            // Database
            //services.AddDbContext<DemoDbContext>(options => options.UseSqlServer(_configuration["Data:DemoConnection:ConnectionString"], b => b.MigrationsAssembly("Demo.WebAPI")));
            services.AddDbContext<DemoDbContext>(options => options.UseInMemoryDatabase("MemoryDB").UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddControllers().AddNewtonsoftJson(
                x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter
                    {
                        DateTimeFormat = "dd'/'MM'/'yyyy'T'HH':'mm"
                    });
                    x.SerializerSettings.Culture = new CultureInfo("fr-FR");
                    x.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                }
            ).ConfigureApiBehaviorOptions(options => {
                options.InvalidModelStateResponseFactory = actionContext => { return CustomErrorResponse(actionContext); };
            });

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //// In production, the Angular files will be served from this directory
            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "ClientApp/dist";
            //});
            return services;
        }

        private static BadRequestObjectResult CustomErrorResponse(ActionContext actionContext)
        {
            //BadRequestObjectResult is class found Microsoft.AspNetCore.Mvc and is inherited from ObjectResult.    
            //Rest code is linq.    
            return new BadRequestObjectResult(actionContext.ModelState
             .Where(modelError => modelError.Value.Errors.Count > 0)
             .Select(modelError => new Error
             {
                 ErrorField = modelError.Key,
                 ErrorDescription = modelError.Value.Errors.FirstOrDefault().ErrorMessage
             }).ToList());
        }
    }
}
