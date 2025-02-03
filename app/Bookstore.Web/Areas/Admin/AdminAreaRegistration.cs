using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bookstore.Web.Areas.Admin
{
    public static class AdminAreaRegistration
    {
        public const string AreaName = "Admin";

        public static void ConfigureAdminArea(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin_default",
                    areaName: AreaName,
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}",
                    defaults: new { area = AreaName }
                );
            });
        }
    }
}