using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Custom.Provider;
using EmployeeManagement.Middlewares;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IConfiguration _config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContextPool<AppDBContext>(
                option => option.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            #region Settings for password
            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequiredLength = 3;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                }
            ).AddEntityFrameworkStores<AppDBContext>(); // For database connection
            //------------OR-----------------
            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequiredLength = 3;
            //    options.Password.RequiredUniqueChars = 0;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireLowercase = false;
            //});
            #endregion

            services.AddSession();

            services.AddMvc(config =>
            {
                config.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
                var policy = new AuthorizationPolicyBuilder()
                               .RequireAuthenticatedUser()
                               .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
        }

        private static void ExecuteMiddleware1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Execute Middleware 1");
            });
        }
        private static void ExecuteMiddleware2(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Execute Middleware 2");
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions();
                developerExceptionPageOptions.SourceCodeLineCount = 3;
                app.UseDeveloperExceptionPage(developerExceptionPageOptions);
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}"); //Url will not change and show the error on same url
                //app.UseStatusCodePages(); // Show default error page.
                //app.UseStatusCodePagesWithRedirects("/Error/{0}"); //Url will change and show the error
                
            }
            
            //app.UseDefaultFiles();
            //app.UseHttpsRedirection(); //To redirect all HTTP requests to HTTPS.
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    //template: "jeet/{controller=Employee}/{action=Index}/{id?}");
                    template: "{controller=Employee}/{action=Index}/{id?}");
            });

            app.UseMyMiddleware();
            //Map method is used for branching the application
            app.Map("/mapPath1", ExecuteMiddleware1);
            app.Map("/mapPath2", ExecuteMiddleware2);

            app.Use(async (context, next) =>
            {
                logger.LogInformation("MW1: IR");
                await next();
                logger.LogInformation("MW1: OR");
            });

            // Middleware define by Run extension method is Terminal Middleware
            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Hello World");
            //    logger.LogInformation("MW2");
            //});
        }
    }
}
