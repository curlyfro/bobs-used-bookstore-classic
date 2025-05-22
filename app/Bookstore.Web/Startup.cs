using Microsoft.Owin;
using Owin;



[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Comment out or remove until LoggingSetup class is implemented
            // LoggingSetup.ConfigureLogging();

            // Comment out or remove until ConfigurationSetup class is implemented
            // ConfigurationSetup.ConfigureConfiguration();

            // Comment out or remove until DependencyInjectionSetup class is implemented
            // DependencyInjectionSetup.ConfigureDependencyInjection(app);

            // Comment out or remove until AuthenticationConfig class is implemented
            // AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}