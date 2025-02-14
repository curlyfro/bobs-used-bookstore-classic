using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bookstore.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(builder.Services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>().GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });

            // Configure other services here
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Startup>();

            logger.LogInformation("Configuring application...");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // Keeping the original method calls, but logging their execution
            logger.LogInformation("Configuring logging...");
            LoggingSetup.ConfigureLogging();

            logger.LogInformation("Configuring configuration...");
            ConfigurationSetup.ConfigureConfiguration();

            logger.LogInformation("Configuring dependency injection...");
            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            logger.LogInformation("Configuring authentication...");
            AuthenticationConfig.ConfigureAuthentication(app);

            logger.LogInformation("Application configured successfully.");
        }
    }
}