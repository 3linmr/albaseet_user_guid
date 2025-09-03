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
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IApprovalSystemService _approvalSystemService;
        private readonly ApplicationSettingDto _application;
        private readonly ApplicationDbContext _context;
        private readonly IMenuService _menuService;
        private readonly IStringLocalizer<AdminController> _localizer;
        private readonly IApplicationDataService _applicationDataService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public AdminController(IHttpContextAccessor httpContextAccessor, IApprovalSystemService approvalSystemService,ApplicationSettingDto application,ApplicationDbContext context,IMenuService menuService,IStringLocalizer<AdminController> _localizer,IApplicationDataService applicationDataService, IWebHostEnvironment webHostEnvironment,IConfiguration configuration)
		{
			_httpContextAccessor = httpContextAccessor;
			_approvalSystemService = approvalSystemService;
            _application = application;
            _context = context;
            _menuService = menuService;
            this._localizer = _localizer;
            _applicationDataService = applicationDataService;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
		}

		[HttpGet]
		[Route("GetUserData")]
		public async Task<IActionResult> GetUserData()
		{
			var userInfo = await _httpContextAccessor.GetUserData();
			var ss = _httpContextAccessor?.HttpContext?.User.FindFirst("company");

			if (userInfo.Menus != null)
			{
				userInfo.PlainMenus = BuildPlainMenus(userInfo.Menus);
				var finalMenus = BuildMenus(userInfo.Menus);
				userInfo.Menus = finalMenus;
				userInfo.Documents = await _menuService.GetDocuments();
			}
			return Ok(userInfo);
		}

		[HttpGet]
		[Route("GetMenuData")]
		public async Task<IActionResult> GetMenuData()
		{
			var userInfo = await _httpContextAccessor.GetUserData();
			if (userInfo.Menus != null)
			{
				userInfo.PlainMenus = BuildPlainMenus(userInfo.Menus);
			}
			return Ok(userInfo);
		}

		[HttpGet]
		[Route("GetMenus")]
		public async Task<IActionResult> GetMenus()
		{
			var menus = await _httpContextAccessor.GetUserMenus();
			var finalMenus = BuildMenus(menus);
			return Ok(finalMenus);
		}
		
		[HttpGet]
		[Route("GetMenuSearch")]
		public async Task<IActionResult> GetMenuSearch(string? menuName)
		{
			var menus = await _httpContextAccessor.GetUserMenus();
			var finalMenus = BuildMenuLabels(menus);
			if (!string.IsNullOrEmpty(menuName))
				finalMenus = finalMenus.Where(x => x.MenuName!.Trim().ToLower().Contains(menuName!.Trim().ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
			return Ok(finalMenus);
		}

		[HttpGet]
		[Route("IsMenuHasApprove")]
		public async Task<ActionResult<MenuApproveTypeDto>> IsMenuHasApprove(int menuCode)
		{
			var allCompanies = await _approvalSystemService.IsMenuHasApprove(menuCode);
			return Ok(allCompanies);
		}

		[HttpGet]
		[Route("GetUserBasicData")]
		public async Task<IActionResult> GetUserBasicData()
		{
			var userInfo = await _httpContextAccessor.GetUserBasicData();
			return Ok(userInfo);
		}

		[HttpPost]
		[Route("ChangeUserLanguage")]
		public async Task<IActionResult> ChangeUserLanguage(string languageCode)
		{
			var userInfo = await _httpContextAccessor.ChangeUserLanguage(languageCode);
			return Ok(userInfo);
		}

		[HttpGet]
		[Route("UserCanDo")]
		public async Task<IActionResult> UserCanDo(int flagId)
		{
			var userInfo = await _httpContextAccessor.UserCanDo(flagId);
			return Ok(userInfo);
		}
		
		[HttpGet]
		[Route("GetSubscriptionBusinessCount")]
		public async Task<IActionResult> GetSubscriptionBusinessCount()
		{
			var businessCount = await IdentityHelper.GetSubscriptionBusinessCount();
			return Ok(businessCount);
		}

		[HttpGet]
		[Route("GetUserStores")]
		public async Task<IActionResult> GetUserStores()
		{
			var userFlags = await _httpContextAccessor.GetUserStores();
			return Ok(userFlags);
		}

		[HttpGet]
		[Route("GetUserSteps")]
		public async Task<IActionResult> GetUserSteps()
		{
			var userSteps = await _httpContextAccessor.GetUserSteps();
			return Ok(userSteps);
		}

		[HttpGet]
		[Route("GetMenuUserFlags")]
		public async Task<IActionResult> GetMenuUserFlags(int menuCode)
		{
			var userFlags = await _httpContextAccessor.GetMenuUserFlags(menuCode);
			return Ok(userFlags);
		}
		
		
		[HttpGet]
		[Route("GetUserFlagsValues")]
		public async Task<IActionResult> GetUserFlagsValues(int menuCode)
		{
			var userFlags = await _httpContextAccessor.GetUserFlagsValues(menuCode);
			return Ok(userFlags);
		}

		[HttpGet]
		[Route("UserCanRoute")]
		public async Task<IActionResult> UserCanRoute(string routingUrl)
		{
			try
			{
				var canRoute = await _httpContextAccessor.UserCanRoute(routingUrl);
				return Ok(canRoute);
			}
			catch (Exception e)
			{

				Console.WriteLine(e);
				return Ok(false);
			}
		}
		
		[HttpGet]
		[Route("IsUserValid")]
		public async Task<IActionResult> IsUserValid(string userId)
		{
			try
			{
				var isUserValid = await IdentityHelper.IsUserValid(userId);
				return Ok(isUserValid);
			}
			catch (Exception e)
			{

				Console.WriteLine(e);
				return Ok(false);
			}
		}
		
		[HttpGet]
		[Route("IsUserOk")]
		public async Task<IActionResult> IsUserOk()
		{
			try
			{
				var canRoute = await _httpContextAccessor.IsUserOk();
				return Ok(canRoute);
			}
			catch (Exception e)
			{

				Console.WriteLine(e);
				return Ok(false);
			}
		}
		
		[HttpGet]
		[Route("UserCanProceed")]
		public async Task<UserValidDto> UserCanProceed()
		{
			try
			{
				var userCanProceed = await _httpContextAccessor.UserCanProceed();
				return userCanProceed;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return new UserValidDto();
			}
		}

		//[HttpPost]
		//[Route("CreateDatabase")]
		//public async Task<IActionResult> CreateDatabase(CompanyIdentityDto model)
		//{
		//	var language = _httpContextAccessor.GetProgramCurrentLanguage();
		//	var companyName = "";
		//	try
		//	{
		//		//await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, model.CompanyId);
		//		//await _context.Database.MigrateAsync();
		//		var sourceDb = _applicationDataService.GetStructureDatabase();
		//		var targetDb = $"albaseet{model.CompanyId.ToString()}";
		//		var result = await CloneDatabaseLogic(sourceDb, targetDb);
		//		if (result.Success)
		//		{
		//			await IdentityHelper.ImportAllTasks(model.CompanyId);
		//			await DatabaseInitializer.ChangeDbContext(_application, _context, model.CompanyId);
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


		[HttpPost]
		[Route("UpdateDatabase")]
		public async Task<IActionResult> UpdateDatabase(int companyId)
		{
			try
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();
				await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, companyId);
				await _context.Database.MigrateAsync();
				var applicationName = language == LanguageCode.Arabic ? ApplicationData.ApplicationNameAr : ApplicationData.ApplicationNameEn;
				return Ok(new ResponseDto() { Success = true, Message = _localizer["DatabaseUpdatedSuccessfully", applicationName] });
			}
            catch (Exception ex)
            {
                return Ok(new ResponseDto() { Success = false, Message = $"Error applying migrations for company {companyId}: {ex.Message} {ex.InnerException?.Message} {ex?.InnerException?.InnerException?.Message}" });
            }
        }

		private List<MenuDto> BuildPlainMenus(List<MenuDto> menus)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var menusInOrder = menus.Where(x => x.Show == true)
				.Select(x => new MenuDto
				{
					MainMenuId = x.MainMenuId,
					MenuId = x.MenuId,
					Icon = x.Icon,
					Image = x.Image,
					MenuUrl = x.MenuUrl,
					ApplicationId = x.ApplicationId,
					MenuName = language == LanguageCode.Arabic ? x.MenuNameAr : x.MenuNameEn,
					MenuNameAr = x.MenuNameAr,
					MenuNameEn = x.MenuNameEn,
					IsFavorite = x.IsFavorite,
					MenuOrder = x.MenuOrder,
					Show = x.Show,
					Section = x.Section,
					IsMain = x.IsMain,
					HasApprove = x.HasApprove,
					MenuCode = x.MenuCode
				}).OrderBy(x => x.MenuId).ToList();
			return menusInOrder;
		}


		private List<MenuDto> BuildMenus(List<MenuDto> menus)
        {
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var allMenus = menus;
			if (allMenus.Any())
			{
				var menusInOrder = allMenus.Where(x => x.MainMenuId == null && x.Show == true)
					.Select(x => new MenuDto
					{
						MainMenuId = x.MainMenuId,
						MenuId = x.MenuId,
						//Icon = x.Icon,
						Image = x.Image,
						MenuUrl = x.MenuUrl,
						ApplicationId = x.ApplicationId,
						MenuName = language == LanguageCode.Arabic ? x.MenuNameAr : x.MenuNameEn,
						MenuNameAr = x.MenuNameAr,
						MenuNameEn = x.MenuNameEn,
						IsFavorite = x.IsFavorite,
						MenuOrder = x.MenuOrder,
						Show = x.Show,
						Section = x.Section,
						IsMain = x.IsMain,
						HasApprove = x.HasApprove,
						MenuCode = x.MenuCode,
						items = GetChildren(menus, x.MenuId)
					}).OrderBy(x => x.MenuOrder).ToList();
				return menusInOrder;
			}
			return new List<MenuDto>();

		}

		private List<MenuDto> GetChildren(List<MenuDto> menus, int parentId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return menus.Where(x => x.MainMenuId == parentId && x.Show == true)
				.Select(x => new MenuDto
				{
					MainMenuId = x.MainMenuId,
					MenuId = x.MenuId,
					//Icon = x.Icon,
					Image = x.Image,
					MenuUrl = x.MenuUrl,
					ApplicationId = x.ApplicationId,
					MenuName = language == LanguageCode.Arabic ? x.MenuNameAr : x.MenuNameEn,
					MenuNameAr = x.MenuNameAr,
					MenuNameEn = x.MenuNameEn,
					IsFavorite = x.IsFavorite,
					MenuOrder = x.MenuOrder,
					Show = x.Show,
					Section = x.Section,
					IsMain = x.IsMain,
					HasApprove = x.HasApprove,
					MenuCode = x.MenuCode,
					items = GetChildren(menus, x.MenuId)
				}).OrderBy(x => x.MenuOrder).ToList();
		}

		private List<MenuSearchDto> BuildMenuLabels(List<MenuDto> menus)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var menuDict = menus.ToDictionary(m => m.MenuId);

			string BuildLabel(int menuId)
			{
				// If the menu has a parent, recursively build the label
				if (menuDict[menuId].MainMenuId.HasValue)
				{
					var parentLabel = BuildLabel(menuDict[menuId].MainMenuId!.Value);
					return language == LanguageCode.Arabic ? $"{parentLabel} > {menuDict[menuId].MenuNameAr}" : $"{parentLabel} > {menuDict[menuId].MenuNameEn}";
				}
				// Base case: root menu
				return language == LanguageCode.Arabic ? menuDict[menuId].MenuNameAr ?? "" :  menuDict[menuId].MenuNameEn ?? "";
			}

			var data = menus.Select(x => new 
			{
				MenuNameAr = x.MenuNameAr ?? "",
				MenuNameEn = x.MenuNameEn ?? "",
				Label = BuildLabel(x.MenuId),
				Route = x.MenuUrl,
				LastMenu = !x.IsMain
			}).ToList();

			return data.Where(x=>x.LastMenu).Select(x=> new MenuSearchDto()
			{
				MenuName = language == LanguageCode.Arabic ? x.MenuNameAr : x.MenuNameEn,
				Label = x.Label,
				Route = x.Route
			}).ToList();
		}


		[HttpPost("clone")]
		public async Task<IActionResult> CloneDatabase([FromQuery] string sourceDb, [FromQuery] string targetDb)
		{
			try
			{
				var backupFilePath = await CloneDatabaseLogic(sourceDb, targetDb);

				return Ok($"Database '{sourceDb}' has been cloned to '{targetDb}' using backup file at '{backupFilePath.Message}'.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

		private async Task<ResponseDto> CloneDatabaseLogic(string sourceDb, string targetDb)
		{
			// Build the backup file path inside wwwroot.
			string backupFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "backup.sql");

			// Build the config file path inside wwwroot (contains mysecret.cnf).
			string configFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "mysecret.cnf");

			// Backup the source database.
			await DatabaseHelper.BackupDatabaseAsync(sourceDb, backupFilePath, configFilePath);

			// Create the new database and import the backup.
			await DatabaseHelper.RestoreDatabaseToNewAsync(targetDb, backupFilePath, configFilePath);

			return new ResponseDto(){Success = true,Message = backupFilePath };
		}
	}
}
