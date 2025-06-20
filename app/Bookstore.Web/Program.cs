
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

    
    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add Entity Framework 6 DbContext
                builder.Services.AddScoped<System.Data.Entity.DbContext>(provider => {
                    // Create and configure your EF6 DbContext here
                    var connectionString = builder.Configuration.GetConnectionString("BookstoreDatabaseConnection");
                    // You may need to replace this with your specific DbContext implementation
                    return new System.Data.Entity.DbContext(connectionString);
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation();

                // Configure Areas
                builder.Services.AddRazorPages(); // Support for areas

                // Register filters
                builder.Services.AddMvc(options => {
                    // In ASP.NET Core, filters are registered directly here
                    // For example: options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                    // Add MVC options from config
                    options.EnableEndpointRouting = true;
                    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                });

                // Configure logging
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();

                // Add application settings
                builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

                // Configure client validation
                builder.Services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options => {
                    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
                        (x) => string.Format("The value '{0}' is invalid", x));
                });

                // EntityFramework 6 configuration is set automatically via the connection string
// No explicit configuration needed here as we're using EF6


                
                var app = builder.Build();

                // Configure the HTTP request pipeline (formerly Configure method)
                if (app.Environment.IsDevelopment() ||
                    app.Configuration["Environment"] == "Development")
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

                // Register bundles (handled differently in .NET Core)
                // BundleConfig.RegisterBundles functionality now handled by bundling middleware

                app.UseRouting();

                // Set authentication service based on config
                var authService = app.Configuration["Services/Authentication"] ?? "local";
                if (authService == "aws")
                {
                    // AWS Cognito authentication would be configured here
                }

                app.UseAuthentication();
                app.UseAuthorization();
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

// Routes are now registered directly using endpoint routing

                // Configure error handling for unhandled exceptions
                app.UseExceptionHandler(errorApp => {
                    errorApp.Run(async context => {
                        context.Response.StatusCode = 500;
                        var logger = context.RequestServices.GetService<ILogger<Program>>();
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        if (exception != null)
                        {
                            logger.LogError(exception, "Unhandled exception");
                        }

                        await context.Response.WriteAsync("An error occurred while processing your request.");
                    });
                });

                
                app.Run();
            }
        }
        
    public class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }

        // Helper methods to access configuration values
        public static string GetAppSetting(string key)
        {
            return Configuration[key];
        }

        public static string GetConnectionString(string name)
        {
            return Configuration.GetConnectionString(name);
        }
    }
    }