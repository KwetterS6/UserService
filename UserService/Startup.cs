using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.DatastoreSettings;
using UserService.Helpers;
using UserService.Repositories;
using UserService.Services;

namespace UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            
            services.AddControllers();
            
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            
            services.AddTransient<IHasher, Hasher>();

            services.AddTransient<IUserService, Services.UserService>();

            services.AddTransient<IUserRepository, UserRepository>();
            
            services.AddTransient<IJWTTokenGenerator, JWTTokenGenerator>();
            
            services.Configure<UserstoreDatabaseSettings>(
                Configuration.GetSection(nameof(UserstoreDatabaseSettings)));
            
            services.AddSingleton<IUserstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<UserstoreDatabaseSettings>>().Value);
                
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}