using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoviesApi.Filters;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi
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
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter)); // Globally applying our custom exception filter
            }).AddXmlDataContractSerializerFormatters(); // Adds in the ability to send data in XML format
            services.AddResponseCaching(); // Adds the ability to cache within the api
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); // Adds in JWT Authentication
            services.AddSingleton<IRepository, InMemoryRepository>();
            services.AddTransient<MyActionFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // app.Use does not short-circuit the pipline, it runs with the pipeline

            // Custom middleware to log every http body response
            app.Use(async (context, next) =>
            {
                using (var swapStream = new MemoryStream())
                {
                    var originalResponseBody = context.Response.Body;
                    context.Response.Body = swapStream;

                    await next.Invoke();

                    swapStream.Seek(0, SeekOrigin.Begin);
                    string responseBody = new StreamReader(swapStream).ReadToEnd();
                    swapStream.Seek(0, SeekOrigin.Begin);

                    await swapStream.CopyToAsync(originalResponseBody);
                    context.Response.Body = originalResponseBody;

                    logger.LogInformation(responseBody);
                }
            });

            /*
             * It is possible to set your own custom middleware/pipelines for an application.
             *
                 app.Map("/map1", (app) =>
                 {
                     app.Run(async conext =>
                     {
                         await conext.Response.WriteAsync("I am short-circuting the pipeline");
                     });
                 });
            */

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching(); // Adds the ability to cache within the api - Must go after Routing

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}