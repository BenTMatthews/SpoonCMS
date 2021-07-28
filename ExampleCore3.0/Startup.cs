using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpoonCMSCore.LiteDBDatalayer;
using SpoonCMSCore.PostGresData;
using SpoonCMSCore.Workers;

namespace ExampleCore3._0
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
            // LiteDB path to store file, for instance: "Data\\Spoon\\"   
            string connStringLDB = "Data\\Spoon\\";
            LiteDBData spoonDataLDB = new LiteDBData(connStringLDB);

            //Postgres DB connection string, for instance: "database=xxxx; host=xxx.xxx.xxx.xxx.com; username=xxx; password=xxx; SslMode=Prefer; port=1234;"
            string connStringPG = "database=xxxx; host=xxx.xxx.xxx.xxx.com; username=xxx; password=xxx; SslMode=Prefer; port=1234;";
            PostGresData spoonDataPG = new PostGresData(connStringPG);

            SpoonWebWorker.AdminPath = "/adminControl";
            SpoonWebWorker.SpoonData = spoonDataLDB;
            //SpoonWebWorker.SpoonData = spoonDataPG;

            //Will need to have some sort of user management system for this to work
            SpoonWebWorker.RequireAuth = false;
            SpoonWebWorker.AuthClaims = new List<Claim>() { new Claim(ClaimTypes.Role, "admins"), new Claim(ClaimTypes.Name, "John") };

            services.AddSingleton(SpoonWebWorker.SpoonData);

            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.Map(SpoonWebWorker.AdminPath, SpoonWebWorker.BuildAdminPageDelegate);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Custom",
                    pattern: "{*AllValues}",
                    defaults: new { controller = "Custom", action = "CustomAction" });

            });
        }
    }
}
