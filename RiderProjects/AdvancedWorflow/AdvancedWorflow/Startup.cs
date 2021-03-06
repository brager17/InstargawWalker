using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedWorflow.Workflow;
using AuthProject.Context;
using AuthProject.EmailSender;
using AuthProject.Identities;
using AuthProject.ServiceCollectionExtensions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdvancedWorflow
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
            services.AutoRegistration();
            services.AddScoped<SmtpClient>();
            services.AddScoped<EmailSenderService>();
            services.AddScoped(typeof(WorkflowManager<,>));
            services.AddDbContext<AuthDbContext>(x =>
                x.UseSqlServer(Configuration.GetConnectionString("ef")));
            
            services.AddIdentity<CustomIdentityUser, CustomIdentityRole>(
                    x =>
                    {
                        x.SignIn.RequireConfirmedEmail = true;
                        x.Password.RequireDigit = false;
                        x.Password.RequireLowercase = false;
                        x.Password.RequireUppercase = false;
                        x.Password.RequiredLength = 5;
                    })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
            
            services.AddControllers()
                .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}