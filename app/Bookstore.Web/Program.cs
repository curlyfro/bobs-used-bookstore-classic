
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using System.Data.Entity;
using Bookstore.Data;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using WebOptimizer;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configure connection string for EntityFramework 6
                var connectionString = builder.Configuration.GetConnectionString("BookstoreDatabaseConnection");
                System.Data.Entity.Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
                System.Data.Entity.Database.DefaultConnectionFactory = new System.Data.Entity.Infrastructure.SqlConnectionFactory(connectionString);

                // Configure Entity Framework
// Note: This is using EntityFramework 6, not EF Core
                // You might need to add a reference to EntityFramework package

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation();

                // Configure client validation (from Web.config)
                builder.Services.AddMvc()
                    .AddMvcOptions(options => { })
                    .SetCompatibilityVersion(CompatibilityVersion.Latest);

                // Add bundling capability (replaces BundleConfig)
                builder.Services.AddWebOptimizer();

                // Add logging
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();

                // Configure services based on environment settings
                var environmentSetting = builder.Configuration["Environment"] ?? "Development";
                var authService = builder.Configuration["Services:Authentication"] ?? "local";
                var databaseService = builder.Configuration["Services:Database"] ?? "local";
                var fileService = builder.Configuration["Services:FileService"] ?? "local";
                var imageValidationService = builder.Configuration["Services:ImageValidationService"] ?? "local";
                var loggingService = builder.Configuration["Services:LoggingService"] ?? "local";

                // Add AWS Cognito configuration if needed
                if (authService == "aws")
                {
                    // Configure Cognito authentication here
                }

                //Added Services


                
                var app = builder.Build();
                
                // Configure the HTTP request pipeline (formerly Configure method)
                if (app.Environment.IsDevelopment())
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
                app.UseStaticFiles();
                
                //Added Middleware
                
                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                // Configure bundling (replaces BundleConfig)
                app.UseWebOptimizer();


                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }