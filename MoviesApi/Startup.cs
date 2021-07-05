using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesApi.Filters;
using MoviesApi.Helpers;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            sqlServer => sqlServer.UseNetTopologySuite()
            ));

            services.AddCors(
                options =>
                {
                    options.AddPolicy("AllowAPIRequestIO",
                        builder => builder.WithOrigins("https://apirequest.io").WithMethods("GET", "POST").AllowAnyHeader());
                });

            services.AddDataProtection();

            services.AddAutoMapper(typeof(Startup));

            services.AddTransient<HashService>();

            services.AddTransient<IFileStorageService, InAppStorageService>();
            services.AddTransient<IHostedService, MovieInTheaterService>();

            services.AddHttpContextAccessor();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<GenreHATEOASAttribute>();
            services.AddTransient<LinksGenerator>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                    ClockSkew = TimeSpan.Zero
                }
                );

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter)); // Globally applying our custom exception filter
            }).AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters(); // Adds in the ability to send data in XML format

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MoviesAPI",
                    Description = "A simple test API that does movies",
                    TermsOfService = new Uri("https://udemy.com/user/felipegaviln/"),
                    License = new OpenApiLicense() { Name = "MIT" },
                    Contact = new OpenApiContact()
                    {
                        Name = "Elliot Beckett",
                        Email = "ElliotBeckettAlt@gmail.com"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(config =>
           {
               config.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesApi");
           });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors();

            // app.UseCors(builder => builder.WithOrigins("https://apirequest.io").WithMethods("GET", "POST").AllowAnyHeader()); // Allows the Apirequest website to make GET and POST requests.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}