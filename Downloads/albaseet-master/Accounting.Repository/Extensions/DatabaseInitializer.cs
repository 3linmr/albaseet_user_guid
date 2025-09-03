using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Helper.Data;
using Shared.Helper.Database;
using Shared.Helper.Models.UserDetail;

namespace Accounting.Repository.Extensions
{
    public static class DatabaseInitializer
    {
        public static async Task<bool> ChangeDbContext(ApplicationSettingDto application, AccountingDbContext currentContext)
        {
            if (application.AdminAPI != null)
            {
                var newConnection = await DatabaseHelper.GetNewConnectionString(application);
                var currentConnection = currentContext?.Database?.GetDbConnection().ConnectionString;
                if (currentConnection != null && currentContext != null)
                {
                    var hasNumber = DatabaseHelper.IsCompanyHasANumber(currentConnection);
                    if (!hasNumber)
                    {
                        var isCompanyTheSames = DatabaseHelper.IsCompanyStillTheSame(newConnection, currentConnection);
                        if (!isCompanyTheSames)
                        {
                            SetConnectionString(currentContext, newConnection);
                        }
                    }
                    else
                    {
                        var isCompanyStillTheSames = DatabaseHelper.IsCompanyStillTheSame(newConnection, currentConnection);
                        if (!isCompanyStillTheSames)
                        {
                            SetConnectionString(currentContext, newConnection);
                        }
                    }
                }
            }
            return true;
        }

        public static void SetConnectionString(AccountingDbContext context, string newConnection)
        {
            context.Database.SetConnectionString(newConnection);
        }


        public static async Task<AccountingDbContext> InitializeNew(ApplicationSettingDto application)
        {
            var newConnection = await DatabaseHelper.GetNewConnectionString(application);

            var optionsBuilder = new DbContextOptionsBuilder<AccountingDbContext>();
            optionsBuilder.UseMySql(newConnection, ServerVersion.AutoDetect(newConnection),
                b => b.MigrationsAssembly("Accounting.Repository"));
            var context = new AccountingDbContext(optionsBuilder.Options);
            return context;
        }

    }
}
