using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using Newtonsoft.Json;
using Shared.Helper.Data;
using Shared.Helper.Identity;
using Shared.Helper.Models.UserDetail;
using static System.Net.Mime.MediaTypeNames;


namespace Shared.Helper.Database
{
    public static class DatabaseHelper
    {
		//public static async Task<string> GetUserConnectionString(this IHttpContextAccessor httpContextAccessor, ApplicationSettingDto application)
  //      {
  //          var isAuthenticated = httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated;
  //          var adminUrl = application.AdminAPI;
  //          if (isAuthenticated == true)
  //          {
  //              var userId = httpContextAccessor?.GetUserId();
  //              var link = $"{adminUrl}/api/AdminAPI/GetUserConnectionString?userId={userId}";
  //              var apiResponse = await ApiHelper.GetAPI(link);
  //              if (!string.IsNullOrWhiteSpace(apiResponse))
  //              {
  //                  return apiResponse ?? "";
  //              }
  //              else
  //              {
  //                  return "";
  //              }
  //          }
  //          else
  //          {
  //              return "";
  //          }

  //      }

		public static async Task<string> GetConnectionString(this IHttpContextAccessor httpContextAccessor, ApplicationSettingDto application)
		{
			var isAuthenticated = httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated;
			var adminUrl = application.AdminAPI;
			if (isAuthenticated == true)
			{
				var userId = httpContextAccessor?.GetUserId();
				var userCompany = await IdentityHelper.GetUserCompany(userId);
				var connectionString = await httpContextAccessor?.GetConnectionString(application)!;
				return connectionString;
			}
			else
			{
				return "";
			}
		}

		//public static async Task<string> GetNewConnectionString(this IMemoryCache memoryCache, ApplicationSettingDto application)
  //      {
  //          var httpContextAccessor = new HttpContextAccessor();
  //          var requestConnection = GetNewConnectionStringFromRequest(memoryCache);
  //          if (string.IsNullOrEmpty(requestConnection) || string.IsNullOrWhiteSpace(requestConnection))
  //          {
  //              var newConnection = await httpContextAccessor.GetUserConnectionString(application);
		//		//httpContextAccessor?.HttpContext?.Items.Add("MyConnection", newConnection);
		//		memoryCache.Set("MyConnection", newConnection, new TimeSpan(0, 0, 1, 0));
  //              return newConnection;
  //          }
  //          else
  //          {
  //              return requestConnection;
  //          }
  //      }

        //public static string GetNewConnectionStringFromRequest(this IMemoryCache memoryCache)
        //{
	       // return Convert.ToString(memoryCache.Get("MyConnection")) ?? "";
	       // //return (httpContextAccessor?.HttpContext?.Items["MyConnection"])?.ToString() ?? "";
        //}

        public static string GetDataBaseNameFromConnectionString(string connectionString)
        {
            var conList = connectionString.Split(";");
            var database = conList.FirstOrDefault(x => x.Contains("database") || x.Contains("Database"));
            var databaseName = database?.Split("=")[1];
            return databaseName ?? "";
        }

   //     public static bool IsCompanyHasANumber(string connectionString)
   //     {
   //         var databaseName = GetDataBaseNameFromConnectionString(connectionString);
   //         return Regex.IsMatch(databaseName, @"\d+$");
			////return char.IsDigit(databaseName.Last());
   //     }


        public static string GetCompanyIdFromConnectionString(string connectionString)
        {
            var databaseName = GetDataBaseNameFromConnectionString(connectionString);
            return databaseName.Replace("albaseet", "");
        }

        public static bool IsCompanyStillTheSame(string newConnection, string currentConnection)
        {
            var newCompanyId = GetCompanyIdFromConnectionString(newConnection);
            var currentCompanyId = GetCompanyIdFromConnectionString(currentConnection);
            return newCompanyId == currentCompanyId;
        }

        public static async Task<DataTable> SqlQuery(string query, string connectionString, params MySqlCommand[] sqlParameters)
        {
            DataTable dataTable = new DataTable();
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, connection);
            foreach (var param in sqlParameters)
            {
                cmd.Parameters.Add(param);
            }
            await connection.OpenAsync();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dataTable);
            await connection.CloseAsync();
            da.Dispose();
            return dataTable;
        }

		public static async Task BackupDatabaseAsync(string oldDbName, string backupFilePath, string configFilePath)
		{
			// Full path to mysqldump.exe (adjust as necessary)
			string mysqldumpPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe";

			// Build the full command string:
			//  - The executable path, options, database name, and redirection to the backup file are all in one string.
			string fullCommand = $"\"{mysqldumpPath}\" --defaults-extra-file=\"{configFilePath}\" --skip-lock-tables --single-transaction {oldDbName} > \"{backupFilePath}\"";

			// Wrap the entire command string in an extra set of quotes and prefix with /c so that CMD executes it.
			var dumpCommand = $"/c \"{fullCommand}\"";

			await ShellHelper.ExecuteShellCommandAsync(dumpCommand);
		}


		public static async Task RestoreDatabaseToNewAsync(string newDbName, string backupFilePath, string configFilePath)
		{
			// Full path to mysql.exe (adjust as necessary)
			string mysqlPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe";

			// Command to create the new database:
			string createDbFullCommand = $"\"{mysqlPath}\" --defaults-extra-file=\"{configFilePath}\" -e \"CREATE DATABASE {newDbName};\"";
			var createDbCommand = $"/c \"{createDbFullCommand}\"";
			await ShellHelper.ExecuteShellCommandAsync(createDbCommand);

			// Command to import the backup into the new database:
			string importFullCommand = $"\"{mysqlPath}\" --defaults-extra-file=\"{configFilePath}\" {newDbName} < \"{backupFilePath}\"";
			var importCommand = $"/c \"{importFullCommand}\"";
			await ShellHelper.ExecuteShellCommandAsync(importCommand);
		}


		/// <summary>
		/// Remotely dumps `oldDbName` into `backupFilePath` on the **remote** server.
		/// </summary>
		public static async Task BackupDatabaseAsync(string sshHost, int sshPort, string sshUser, string sshPwd, string oldDbName, string backupFilePath, string configFilePath)
		{
			// Path to mysqldump on the remote server
			var mysqldump = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe";

			// Build the shell command (no PowerShell, just plain CMD redirection)
			// e.g.:
			//   "C:\...\mysqldump.exe" --defaults-extra-file="C:\path\mysecret.cnf"
			//     --skip-lock-tables --single-transaction oldDbName > "C:\backups\backup.sql"
			string cmd = $@"""{mysqldump}"" --defaults-extra-file=""{configFilePath}"" " +
						 $"--skip-lock-tables --single-transaction {oldDbName} > \"{backupFilePath}\"";

			await ShellHelper.ExecuteOverSshAsync(sshHost, sshPort, sshUser, sshPwd, cmd);
		}

		/// <summary>
		/// Remotely creates a new database and imports the dump from `backupFilePath`.
		/// </summary>
		public static async Task RestoreDatabaseToNewAsync(string sshHost, int sshPort, string sshUser, string sshPwd, string newDbName, string backupFilePath, string configFilePath)
		{
			var mysql = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe";

			// 1) CREATE DATABASE
			string createCmd = $@"""{mysql}"" --defaults-extra-file=""{configFilePath}"" " +
							   $@"-e ""CREATE DATABASE {newDbName};""";
			await ShellHelper.ExecuteOverSshAsync(sshHost, sshPort, sshUser, sshPwd, createCmd);

			// 2) IMPORT the dump
			string importCmd = $@"""{mysql}"" --defaults-extra-file=""{configFilePath}"" " +
							   $"{newDbName} < \"{backupFilePath}\"";
			await ShellHelper.ExecuteOverSshAsync(sshHost, sshPort, sshUser, sshPwd, importCmd);
		}


	}
}
