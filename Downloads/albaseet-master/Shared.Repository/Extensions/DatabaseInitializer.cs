using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Shared.Helper.Data;
using Shared.Helper.Database;
using Shared.Helper.Identity;
using Shared.Helper.Models.UserDetail;
using static System.Net.Mime.MediaTypeNames;


namespace Shared.Repository.Extensions
{
	public static class DatabaseInitializer
	{
		public static async Task<bool> ChangeDatabaseConnectionStringAsync(IConfiguration configuration, ApplicationDbContext currentContext, int databaseNumber)
		{
			var template = configuration.GetConnectionString("albaseetWithNumber");
			if (string.IsNullOrWhiteSpace(template))
				throw new InvalidOperationException("Connection string 'albaseet' not found.");

			var newConnectionString = string.Format(template, databaseNumber);
			await SetConnectionStringAsync(currentContext, newConnectionString);
			return true;
		}

		public static async Task SetConnectionStringAsync(ApplicationDbContext context, string newConnectionString)
		{
			// Grab the raw ADO connection
			DbConnection conn = context.Database.GetDbConnection();

			// If it's already open, close it first
			if (conn.State == ConnectionState.Open)
				await context.Database.CloseConnectionAsync();

			// Swap in the new string
			conn.ConnectionString = newConnectionString;

			// Re‑open asynchronously
			await context.Database.OpenConnectionAsync();
		}

		//public static async Task<bool> ChangeDbContext(ApplicationSettingDto application, ApplicationDbContext currentContext,int? companyId)
  //      {
  //          if (application.AdminAPI != null)
  //          {
  //              var newConnection = await IdentityHelper.GetUserConnectionString(companyId);
  //              var currentConnection = currentContext?.Database?.GetDbConnection().ConnectionString;

  //              if (currentConnection != null && currentContext != null)
  //              {
  //                  var isCompanyTheSames = DatabaseHelper.IsCompanyStillTheSame(newConnection, currentConnection);
  //                  if (!isCompanyTheSames)
  //                  {
  //                      SetConnectionString(currentContext, newConnection);
  //                  }
  //              }
  //          }
  //          return true;
  //      }

        //public static async Task<bool> ChangeDbContext(ApplicationSettingDto application, ApplicationDbContext currentContext)
        //{
        //	if (application.AdminAPI != null)
        //	{
        //		var newConnection = await IdentityHelper.GetUserConnectionString();
        //		var currentConnection = currentContext?.Database?.GetDbConnection().ConnectionString;
        //		if (currentConnection != null && currentContext != null)
        //		{
        //			var hasNumber = DatabaseHelper.IsCompanyHasANumber(currentConnection);
        //			if (!hasNumber)
        //			{
        //				var isCompanyTheSames = DatabaseHelper.IsCompanyStillTheSame(newConnection, currentConnection);
        //				if (!isCompanyTheSames)
        //				{
        //					SetConnectionString(currentContext, newConnection);
        //				}
        //			}
        //			else
        //			{
        //				var isCompanyStillTheSames = DatabaseHelper.IsCompanyStillTheSame(newConnection, currentConnection);
        //				if (!isCompanyStillTheSames)
        //				{
        //					SetConnectionString(currentContext, newConnection);
        //				}
        //			}
        //		}
        //	}
        //	return true;
        //}


  //      public static void SetConnectionString(ApplicationDbContext context, string newConnection)
		//{
		//	context.Database.SetConnectionString(newConnection);


		//	//try
		//	//{
		//	//    context.Database.SetConnectionString(newConnection);
		//	//}
		//	//catch (Exception e)
		//	//{
		//	//    context.Database.CloseConnectionAsync();

		//	//}
		//	//finally
		//	//{
		//	//    context.Database.OpenConnectionAsync();
		//	//}
		//}


		//public static async Task<ApplicationDbContext> InitializeNew(ApplicationSettingDto application)
		//{
		//    var newConnection = await DatabaseHelper.GetNewConnectionString(application);

		//    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		//    optionsBuilder.UseMySql(newConnection, ServerVersion.AutoDetect(newConnection),
		//        b => b.MigrationsAssembly("Shared.Repository"));
		//    var context = new ApplicationDbContext(optionsBuilder.Options);
		//    return context;
		//}

	}
}
