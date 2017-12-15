using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ASPTest
{
    public class Startup
    {
        // string connString = "server=10.10.0.89;port=3306;database=test_db;user=approval_user;password=password";
        //public Startup(IHostingEnvironment env)
        //{
        //    var builder = new ConfigurationBuilder()
        //       .SetBasePath(env.ContentRootPath)
        //       .AddJsonFile("Properties\\launchSettings.json", optional: false, reloadOnChange: true)
        //       .AddJsonFile($"launchSettings.{env.EnvironmentName}.json", optional: true)
        //       .AddEnvironmentVariables();
        //    Configuration = builder.Build();
        //}

        //public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //services.Add(new ServiceDescriptor(typeof(MySQLComms), new MySQLComms(Configuration.GetConnectionString("DefaultConnection"))));
            // services.Add(new ServiceDescriptor(typeof(MySQLCommsOLD), new MySQLCommsOLD(connString)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Appoval}/{id?}");
            });

            //    app.Run(async (context) =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
        }
    }
}
