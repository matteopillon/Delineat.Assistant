using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Stores;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Core.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;

namespace Delineat.Assistant.API
{
    public class Startup
    {
        private const string kCORSAllowAnyOriginPolicy = "CORSAllowAny";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<DAStoresConfiguration>(Configuration);
            services.Configure<DALogConfiguration>(Configuration);
            services.Configure<DAClientConfiguration>(Configuration);
            services.Configure<DAEmailConfiguration>(Configuration);
            services.Configure<DAJobsConfiguration>(Configuration);
            services.Configure<DAServerConfiguration>(Configuration);

            services.AddScoped<IDAUsersStore, DADBUsersStore>();

            services.AddDbContext<DAAssistantDBContext>(options =>
            {
                options.UseSqlServer("name = ConnectionStrings:DefaultConnection");
            });

            services.AddMemoryCache();

            services.AddControllers();

            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.MapType(typeof(IFormFile), () => new OpenApiSchema() { Type = "file", Format = "binary" });
            });

            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: kCORSAllowAnyOriginPolicy,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin()
                                     .AllowAnyMethod()
                                     .AllowAnyHeader();
                                  });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(kCORSAllowAnyOriginPolicy);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Assistant Api v1");
            });
            
        }
    }
}
