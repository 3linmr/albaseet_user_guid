using Microsoft.EntityFrameworkCore;
using Shared.Repository;
using System.Data.SqlClient;
using System.Data;
using MySqlConnector;
using Shared.Helper.Database;
using Shared.Helper.Models.UserDetail;

namespace Accounting.API.Extensions
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
