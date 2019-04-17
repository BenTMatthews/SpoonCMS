using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpoonCMSCore.Interfaces;
using SpoonCMSCore.LiteDBDatalayer;
using SpoonCMSCore.PostGresData;
using SpoonCMSCore.Workers;
using System.Collections.Generic;
using System.Security.Claims;
using static SpoonCMSCore.DataClasses.Enums;

namespace ExampleCore
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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }           
                        
            app.Map(SpoonWebWorker.AdminPath, SpoonWebWorker.BuildAdminPageDelegate);                 

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "SpoonAdmin",
                //    template: SpoonWebWorker.AdminPath + "/{*AllValues}",
                //    defaults: new { controller = "Spoon", action = "Admin" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "Custom",
                    template:"{*AllValues}",
                    defaults: new { controller = "Custom", action = "Custom" });
            });

        }
    }
}
