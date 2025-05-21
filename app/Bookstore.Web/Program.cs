
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
    
    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add configuration settings from web.config
                builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

                // Add connection string
                var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings/BookstoreDatabaseConnection")
                    ?? "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";

// Register DbContext directly using EF6 (not EF Core)
                builder.Services.AddScoped<System.Data.Entity.DbContext>(serviceProvider =>
                {
                    // Using Entity Framework 6, not EF Core
                    // Create and return your DbContext instance with connection string
                    return new System.Data.Entity.DbContext(connectionString);
                });

                // Add application settings
                builder.Services.Configure<AppSettings>(options => {
                    options.ClientValidationEnabled = builder.Configuration.GetValue<bool>("ClientValidationEnabled", true);
                    options.Environment = builder.Configuration.GetValue<string>("Environment", "Development");
                    options.ServicesAuthentication = builder.Configuration.GetValue<string>("Services/Authentication", "local");
                    options.ServicesDatabase = builder.Configuration.GetValue<string>("Services/Database", "local");
                    options.ServicesFileService = builder.Configuration.GetValue<string>("Services/FileService", "local");
                    options.ServicesImageValidationService = builder.Configuration.GetValue<string>("Services/ImageValidationService", "local");
                    options.ServicesLoggingService = builder.Configuration.GetValue<string>("Services/LoggingService", "local");
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Register area configurations
                builder.Services.AddMvc()
                    .AddMvcOptions(options => {
                        // Global filters can be added here
                    });

                // Add bundling services (replaces BundleConfig)
                // For modern bundling in .NET Core, use a tool like WebOptimizer:
                // builder.Services.AddWebOptimizer();

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

                app.UseAuthorization();

                // Configure error handling middleware to log exceptions
                app.UseExceptionHandler(errorApp => {
                    errorApp.Run(async context => {
                        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        logger.LogError(exception, "Unhandled exception");
                        await Task.CompletedTask;
                    });
                });
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Map area routes (replaces AreaRegistration.RegisterAllAreas())
                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                
                app.Run();
            }
    }

        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }

        public class AppSettings
        {
            public bool ClientValidationEnabled { get; set; }
            public string Environment { get; set; }
            public string ServicesAuthentication { get; set; }
            public string ServicesDatabase { get; set; }
            public string ServicesFileService { get; set; }
            public string ServicesImageValidationService { get; set; }
            public string ServicesLoggingService { get; set; }

            // Authentication settings
            public string AuthCognitoLocalClientId { get; set; }
            public string AuthCognitoAppRunnerClientId { get; set; }
            public string AuthCognitoMetadataAddress { get; set; }
            public string AuthCognitoDomain { get; set; }

            // File service settings
            public string FilesBucketName { get; set; }
            public string FilesCloudFrontDomain { get; set; }
        }
    }