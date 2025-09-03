using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Admin;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Database;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.StaticData;
using Shared.Helper.Models.UserDetail;
using Shared.Repository;
using Shared.Repository.Extensions;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.API.Controllers.Admin
{
	[Route("api/[controller]")]
	[ApiController]
	public class VisitorsController : ControllerBase
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ApplicationSettingDto _application;
		private readonly ApplicationDbContext _context;
		private readonly IStringLocalizer<AdminController> _localizer;
		private readonly IApplicationDataService _applicationDataService;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IConfiguration _configuration;

		public VisitorsController(IHttpContextAccessor httpContextAccessor, ApplicationSettingDto application, ApplicationDbContext context, IStringLocalizer<AdminController> _localizer, IApplicationDataService applicationDataService, IWebHostEnvironment webHostEnvironment,IConfiguration configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_application = application;
			_context = context;
			this._localizer = _localizer;
			_applicationDataService = applicationDataService;
			_webHostEnvironment = webHostEnvironment;
			_configuration = configuration;
		}

		[HttpGet]
		[Route("IsServerUp")]
		public IActionResult IsServerUp()
		{
			return Ok(true);
		}

		//[HttpPost]
		//[Route("CreateDatabase")]
		//public async Task<IActionResult> CreateDatabase(CompanyIdentityDto model)
		//{
		//	var language = _httpContextAccessor.GetProgramCurrentLanguage();
		//	var companyName = "";
		//	try
		//	{
		//		var sourceDb = _applicationDataService.GetStructureDatabase();
		//		var targetDb = $"albaseet{model.CompanyId.ToString()}";
		//		var result = await CloneDatabaseLogic(sourceDb, targetDb);
		//		if (result.Success)
		//		{
		//			await IdentityHelper.ImportAllTasks(model.CompanyId);
		//			await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, model.CompanyId);
		//			await _applicationDataService.CreateFirstCompany(model);
		//			var applicationName = language == LanguageCode.Arabic ? ApplicationData.ApplicationNameAr : ApplicationData.ApplicationNameEn;
		//			companyName = language == LanguageCode.Arabic ? model.CompanyNameAr : model.CompanyNameEn;
		//			return Ok(new ResponseDto() { Success = true, Message = _localizer["DatabaseCreatedSuccessfully", applicationName] });
		//		}
		//		return Ok(new ResponseDto() { Success = false, Message = _localizer["DatabaseCreatedFailed", companyName] });
		//	}
		//	catch (Exception ex)
		//	{
		//		return Ok(new ResponseDto() { Success = false, Message = $"{_localizer["DatabaseCreatedFailed"]} {companyName}: {ex.Message} {ex.InnerException?.Message} {ex?.InnerException?.InnerException?.Message}" });
		//	}
		//}

		//private async Task<ResponseDto> CloneDatabaseLogic(string sourceDb, string targetDb)
		//{
		//	// Build the backup file path inside wwwroot.
		//	string backupFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "backup.sql");

		//	// Build the config file path inside wwwroot (contains mysecret.cnf).
		//	string configFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "mysecret.cnf");

		//	// Backup the source database.
		//	await DatabaseHelper.BackupDatabaseAsync(sourceDb, backupFilePath, configFilePath);

		//	// Create the new database and import the backup.
		//	await DatabaseHelper.RestoreDatabaseToNewAsync(targetDb, backupFilePath, configFilePath);

		//	return new ResponseDto() { Success = true, Message = backupFilePath };
		//}



		[HttpPost]
		[Route("CreateDatabase")]
		public async Task<IActionResult> CreateDatabase(CompanyIdentityDto model)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyName = "";

			try
			{
				// 1) Build names
				string sourceDb = _applicationDataService.GetStructureDatabase();
				string targetDb = $"albaseet{model.CompanyId}";

				// 2) SSH connection settings
				var ssh = _applicationDataService.GetSshOptions();

				// 3) Perform clone
				var result = await CloneDatabaseLogic(ssh.Host!, ssh.Port, ssh.Username!, ssh.Password!, sourceDb, targetDb);

				if (result.Success)
				{
					await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, model.CompanyId);
					await _applicationDataService.CreateFirstCompany(model);
					var applicationName = language == LanguageCode.Arabic ? ApplicationData.ApplicationNameAr : ApplicationData.ApplicationNameEn;
					companyName = language == LanguageCode.Arabic ? model.CompanyNameAr : model.CompanyNameEn;
					return Ok(new ResponseDto() { Success = true, Message = _localizer["DatabaseCreatedSuccessfully", applicationName] });
				}
				return Ok(new ResponseDto() { Success = false, Message = _localizer["DatabaseCreatedFailed", companyName] });
			}
			catch (Exception ex)
			{
				return Ok(new ResponseDto() { Success = false, Message = $"{_localizer["DatabaseCreatedFailed"]} {companyName}: {ex.Message} {ex.InnerException?.Message} {ex?.InnerException?.InnerException?.Message}" });
			}
		}

		private static async Task<ResponseDto> CloneDatabaseLogic(string sshHost, int sshPort, string sshUser, string sshPwd, string sourceDb, string targetDb)
		{
			// These paths live on the **remote** server’s disk!
			string backupFile = $@"C:\backups\{sourceDb}.sql";
			string configFile = @"C:\backups\mysecret.cnf";

			// Check If the backup exist first
			var fileIsThere = ShellHelper.RemoteFileExists(sshHost, sshPort, sshUser, sshPwd, backupFile);

			if (!fileIsThere)
			{
				// Run backup & restore over SSH
				await DatabaseHelper.BackupDatabaseAsync(sshHost, sshPort, sshUser, sshPwd, sourceDb, backupFile, configFile);
			}
			
			await DatabaseHelper.RestoreDatabaseToNewAsync(sshHost, sshPort, sshUser, sshPwd, targetDb, backupFile, configFile);

			return new ResponseDto
			{
				Success = true,
				Message = $"Backup taken at {backupFile} and restored to {targetDb}"
			};
		}
	}
}
