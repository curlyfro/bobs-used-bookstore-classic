using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bookstore.Web.Setup; // Add this line to import the Setup namespace

namespace Bookstore.Web
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(ILogger<Startup> logger)
        {
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure services here
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            _logger.LogInformation("Configuring application...");

            _logger.LogInformation("Configuring logging...");
            LoggingSetup.ConfigureLogging();

            _logger.LogInformation("Configuring configuration...");
            ConfigurationSetup.ConfigureConfiguration();

            _logger.LogInformation("Configuring dependency injection...");
            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            _logger.LogInformation("Configuring authentication...");
            AuthenticationConfig.ConfigureAuthentication(app);

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

            _logger.LogInformation("Application configured successfully.");
        }
    }
}