
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
using WebOptimizer;

    namespace Bookstore.Web
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add connection strings from Web.config
                builder.Configuration.AddConnectionString("BookstoreDatabaseConnection",
                    "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;");

                // Configure app settings from Web.config
                builder.Services.Configure<MvcOptions>(options => {
                    options.EnableEndpointRouting = true;
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews(options => {
                    options.EnableEndpointRouting = true;
                })
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
                builder.Services.AddRazorPages();

                // Register all areas
                builder.Services.Configure<RouteOptions>(options => {
                    options.LowercaseUrls = true;
                    options.AppendTrailingSlash = true;
                });

                // Add bundling services
                builder.Services.AddWebOptimizer(pipeline => {
                    // Configure bundles here similar to BundleConfig
                });

                // Configure client-side validation (from Web.config appSettings)
                builder.Services.AddMvc().AddViewOptions(options => {
                    options.HtmlHelperOptions.ClientValidationEnabled = true;
                });

                // Add app settings from Web.config
                builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

                var app = builder.Build();

                // Configure logging
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                
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
                app.Use(async (context, next) => {
                    // Access environment setting from config
                    string environment = app.Configuration["AppSettings:Environment"] ?? "Development";
                    context.Items["Environment"] = environment;
                    await next();
                });

                app.UseRouting();

                app.UseAuthorization();

                // Register routes
                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.MapRazorPages();

                // Global error handler
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next(context);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An unhandled exception occurred");
                        throw;
                    }
                });

                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }

        public static class AddConnectionStringExtension
        {
            public static IConfigurationBuilder AddConnectionString(this IConfigurationBuilder configurationBuilder, string name, string connectionString)
            {
                return configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { $"ConnectionStrings:{name}", connectionString }
                });
            }
        }

        public class AppSettings
        {
            public string Environment { get; set; } = "Development";
            public Services Services { get; set; } = new Services();
            public Authentication Authentication { get; set; } = new Authentication();
            public Files Files { get; set; } = new Files();
        }

        public class Services
        {
            public string Authentication { get; set; } = "local";
            public string Database { get; set; } = "local";
            public string FileService { get; set; } = "local";
            public string ImageValidationService { get; set; } = "local";
            public string LoggingService { get; set; } = "local";
        }

        public class Authentication
        {
            public Cognito Cognito { get; set; } = new Cognito();
        }

        public class Cognito
        {
            public string LocalClientId { get; set; } = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
            public string AppRunnerClientId { get; set; } = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
            public string MetadataAddress { get; set; } = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
            public string CognitoDomain { get; set; } = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
        }

        public class Files
        {
            public string BucketName { get; set; } = "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']";
            public string CloudFrontDomain { get; set; } = "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']";
        }
    }