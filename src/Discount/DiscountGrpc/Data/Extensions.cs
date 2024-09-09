
using Microsoft.EntityFrameworkCore;

namespace DiscountGrpc.Data
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<DiscountContext>();
            
            context.Database.MigrateAsync();
            return app;
        }
    }
}