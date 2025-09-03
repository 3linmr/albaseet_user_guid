using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Shared.API.Helpers
{
    public static class DatabaseInitializer
    {
        public static void Initialize(WebApplicationBuilder builder)
        {
            //var httpContextAccessor = new HttpContextAccessor();
            //var connectionString = httpContextAccessor.GetUserConnectionString();

            //builder.Services.AddDbContext<ApplicationDbContext>(options => options
            //    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly("Shared.Repository")));

        }


        //public static ApplicationDbContext CreateDbContext()
        //{
        //    var connection = "server=localhost;user id=root;password=ayman;database=albaseet_copy;";
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    optionsBuilder.UseMySql(connection, ServerVersion.AutoDetect(connection), b => b.MigrationsAssembly("Shared.Repository"));
        //    var context = new ApplicationDbContext(optionsBuilder.Options);
        //    return context;
        //}
    }
}
