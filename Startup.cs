using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using AvibaWeb.Data;
using AvibaWeb.DomainModels;
using AvibaWeb.Hubs;
using AvibaWeb.Infrastructure;

namespace AvibaWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => 
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add EF services to the services container.
            services.AddEntityFrameworkSqlServer().AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration["Data:CollectionDB:ConnectionString"], sqlServerOptions => sqlServerOptions.CommandTimeout(120)));

            services.AddIdentity<AppUser, AppRole>(options => {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
            });

            services.AddMvc();

            services.AddSignalR();
            services.AddSingleton<IDatabaseSubscription, CollectionOperationDatabaseSubscription>();

            services.AddHttpClient();
            services.AddSingleton(Configuration);

            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddSingleton<ICyrillerService, CyrillerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();

            app.UseSignalR(routes =>
            {
                routes.MapHub<MessageHub>("/MessageHub");
            });

            app.UseMvcWithDefaultRoute();

            app.UseSqlTableDependency(Configuration["Data:CollectionDB:ConnectionString"]);

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
