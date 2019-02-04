﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Organiser.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Organiser.Actions;
using Organiser.Services;

namespace Organiser
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
           var connection = Configuration.GetSection("ConnectionStrings").GetSection("OrganiserData").Value;

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connection));
            services.AddAuthentication(o =>
            {
                o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Account/Login/");
                options.LoginPath = new PathString("/Account/Login/");
            });
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IDepartmentStateRepository, DepartmentStateRepository>();
            services.AddTransient<INoteRepository, NoteRepository>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<IAccountActions, AccountActions>();
            services.AddTransient<IAccountService, AccountService>();

            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
           
            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "specifiedSearch",
                    template: "order/{action}/{id?}",
                    defaults: new { Controller = "Order", action = "Index"});

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Order}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "NonDefault",
                    template: "{controller=Order}/{action=Index}/{DepartmentStateId?}");
                

                routes.MapRoute(
                    name: "markAsFinished",
                    template: "{controller}/{action}/{DepartmentStateId?}/{errorType?}/{message?}/{showMessages?}");
            });
        }
    }
}
