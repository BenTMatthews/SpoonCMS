using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpoonCMS.DataClasses;
using SpoonCMS.Interfaces;
using SpoonCMS.Workers;
using System.Collections.Generic;
using System.Security.Claims;

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
            //string connString = Configuration["LiteDBSettings:ConnectionString"];            
            //ISpoonData spoonData = new LiteDBData(connString);

            string connString = Configuration["LiteDBSettings:ConnectionString"];
            ISpoonData spoonData = new PostGresData(connString);

            SpoonWebWorker.AdminPath = "/adminControl";
            SpoonWebWorker.SpoonData = spoonData;

            //Will need to have some sort of user management system for this to work
            SpoonWebWorker.RequireAuth = false;
            SpoonWebWorker.AuthClaims = new List<Claim>() { new Claim(ClaimTypes.Role, "admins"), new Claim(ClaimTypes.Name, "John") };

            services.AddSingleton<ISpoonData>(spoonData);
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
