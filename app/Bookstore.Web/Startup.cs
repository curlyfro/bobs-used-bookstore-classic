using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;
using Owin;
using NLog;
using System;

[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            try
            {
                // Simple NLog configuration
                var config = new NLog.Config.LoggingConfiguration();
                var consoleTarget = new NLog.Targets.ConsoleTarget("console");
                config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
                LogManager.Configuration = config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring logging: {ex}");
            }
        }
    }

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configuration setup logic here
            Console.WriteLine("Configuring application settings");
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            // Temporarily comment out these lines until their implementation is added
            // DependencyInjectionSetup.ConfigureDependencyInjection(app);
            // AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}