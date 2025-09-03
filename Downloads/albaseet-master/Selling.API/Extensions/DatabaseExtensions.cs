using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Sales.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static void UpdateDatabase(WebApplication app)
        {
            using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context?.Database.Migrate();
        }
    }
}
