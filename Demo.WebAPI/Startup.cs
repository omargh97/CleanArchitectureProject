using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Demo.DAL.Context;
using Demo.WebAPI.Middleware;
using Microsoft.Extensions.Hosting;
using Demo.WebAPI.Extensions;

namespace Demo.WebAPI
{
    public class Startup
    {

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(Startup).Assembly);

            // Application Insights
            services.AddApplicationInsightsTelemetry();

            services.AddApplicationsServices(_configuration);
            services.AddIdentityServices(_configuration);

            // Injections
            ScopedConfiguration.ScopedConfig(services);

            // Enable Cors
            services.AddCors();

            // Enable Swagger
            services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptionsConfigure>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, DemoDbContext dataContext, ILoggerFactory loggerFactory, IHostApplicationLifetime lifetime)
        {
            // Dont change the order off Middleware 

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHsts();

            app.UseHttpsRedirection();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthentication();

            app.UseAuthorization();

            // Custom Middleware must be added after this line

            app.UseMiddleware<SecureAPIMiddleware>();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new JsonExceptionMiddleware(loggerFactory).Invoke
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            lifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine("*** Application is started...");

            }, true);

            lifetime.ApplicationStopping.Register(() =>
            {
                Console.WriteLine("*** Application is shutting down...");
            }, true);

            lifetime.ApplicationStopped.Register(() =>
            {
                Console.WriteLine("*** Application is shut down...");
            }, true);
        }       
    }
}
