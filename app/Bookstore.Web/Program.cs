
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
using System.Data.Entity;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add configuration from appsettings.json and environment variables
                builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add database connection strings for EF6
                builder.Services.AddScoped<DbContext>(serviceProvider =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("BookstoreDatabaseConnection");
                    // Using EntityFramework 6, not EF Core
                    // Note: This typically requires additional configuration for EF6 in .NET Core
                    return new System.Data.Entity.DbContext(connectionString);
                });

                // Add app settings configuration
                builder.Services.Configure<AppSettings>(options =>
                {
                    options.ClientValidationEnabled = builder.Configuration.GetValue<bool>("AppSettings:ClientValidationEnabled");
                    options.Environment = builder.Configuration.GetValue<string>("AppSettings:Environment");
                    // Services settings
                    options.Services = new ServicesSettings
                    {
                        Authentication = builder.Configuration.GetValue<string>("AppSettings:Services:Authentication"),
                        Database = builder.Configuration.GetValue<string>("AppSettings:Services:Database"),
                        FileService = builder.Configuration.GetValue<string>("AppSettings:Services:FileService"),
                        ImageValidationService = builder.Configuration.GetValue<string>("AppSettings:Services:ImageValidationService"),
                        LoggingService = builder.Configuration.GetValue<string>("AppSettings:Services:LoggingService")
                    };
                    // Authentication settings
                    options.Authentication = new AuthenticationSettings
                    {
                        Cognito = new CognitoSettings
                        {
                            LocalClientId = builder.Configuration.GetValue<string>("AppSettings:Authentication:Cognito:LocalClientId"),
                            AppRunnerClientId = builder.Configuration.GetValue<string>("AppSettings:Authentication:Cognito:AppRunnerClientId"),
                            MetadataAddress = builder.Configuration.GetValue<string>("AppSettings:Authentication:Cognito:MetadataAddress"),
                            CognitoDomain = builder.Configuration.GetValue<string>("AppSettings:Authentication:Cognito:CognitoDomain")
                        }
                    };
                    // Files settings
                    options.Files = new FilesSettings
                    {
                        BucketName = builder.Configuration.GetValue<string>("AppSettings:Files:BucketName"),
                        CloudFrontDomain = builder.Configuration.GetValue<string>("AppSettings:Files:CloudFrontDomain")
                    };
                });

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Register areas
                builder.Services.AddMvc()
                    .AddMvcOptions(options => {
                        // Add any filter configurations here if needed
                    });

                // Added bundle support (similar to BundleConfig)
                // Note: Use modern alternatives like Webpack or NPM for bundling in new projects

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

                // Configure logging for application errors
                app.UseExceptionHandler(errorApp => {
                    errorApp.Run(async context => {
                        var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        if (exception != null)
                        {
                            logger.LogError(exception, "Unhandled exception");
                        }

                        await Task.CompletedTask;
                    });
                });

                app.UseRouting();

                app.UseAuthorization();

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

        public class AppSettings
        {
            public bool ClientValidationEnabled { get; set; }
            public string Environment { get; set; }
            public ServicesSettings Services { get; set; }
            public AuthenticationSettings Authentication { get; set; }
            public FilesSettings Files { get; set; }
        }

        public class ServicesSettings
        {
            public string Authentication { get; set; }
            public string Database { get; set; }
            public string FileService { get; set; }
            public string ImageValidationService { get; set; }
            public string LoggingService { get; set; }
        }

        public class AuthenticationSettings
        {
            public CognitoSettings Cognito { get; set; }
        }

        public class CognitoSettings
        {
            public string LocalClientId { get; set; }
            public string AppRunnerClientId { get; set; }
            public string MetadataAddress { get; set; }
            public string CognitoDomain { get; set; }
        }

        public class FilesSettings
        {
            public string BucketName { get; set; }
            public string CloudFrontDomain { get; set; }
        }
    }