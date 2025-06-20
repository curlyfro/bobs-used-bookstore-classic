using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }

    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            // Configure logging
            Console.WriteLine("Logging configured");
        }
    }

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configure configuration
            Console.WriteLine("Configuration configured");
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Configure dependency injection
            Console.WriteLine("Dependency injection configured");
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Configure authentication
            Console.WriteLine("Authentication configured");
        }
    }
}