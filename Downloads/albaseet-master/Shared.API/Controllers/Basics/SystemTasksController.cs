using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Shared.API.Extensions;
using Shared.API.Helpers;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.UserDetail;
using static System.Net.Mime.MediaTypeNames;

namespace Shared.API.Controllers.Basics
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SystemTasksController : ControllerBase
	{
		private readonly ApplicationSettingDto _application;
		private readonly IStringLocalizer<SystemTasksController> _localizer;
		private readonly ISystemTaskService _systemTaskService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
		private readonly IConfiguration _configuration;

		public SystemTasksController(ApplicationSettingDto application, IStringLocalizer<SystemTasksController> localizer, ISystemTaskService systemTaskService, IDatabaseTransaction databaseTransaction, IHttpContextAccessor httpContextAccessor, IStringLocalizer<HandelException> exceptionLocalizer, IConfiguration configuration)
		{
			_application = application;
			_localizer = localizer;
			_systemTaskService = systemTaskService;
			_databaseTransaction = databaseTransaction;
			_httpContextAccessor = httpContextAccessor;
			_exceptionLocalizer = exceptionLocalizer;
			_configuration = configuration;
		}


		[HttpGet]
		[Route("GetAllSystemTasks")]
		public async Task<ActionResult<IEnumerable<SystemTaskDto>>> GetAllSystemTasks()
		{
			var tasks = await _systemTaskService.GetAllSystemTasks().ToListAsync();
			return Ok(tasks);
		}
		
		[HttpGet]
		[Route("CanImportAll")]
		public async Task<IActionResult> CanImportAll()
		{
			var data = await _systemTaskService.CanImportAll();
			return Ok(data);
		}

		[HttpPost]
		[Route("ImportAll")]
		public async Task<IActionResult> ImportAll(int companyId)
		{
			//var connectionString = _configuration.GetConnectionString("albaseet") ?? "";
			var connectionString = GetConnectionString(companyId);
			try
			{
				await ImportCurrenciesLogic(companyId);
				await ImportCountriesLogic(companyId);
				await ImportStatesLogic(companyId);
				await ImportCitiesLogic(companyId);
				await ImportDistrictsLogic(companyId);
				return Ok(new ResponseDto() { Success = true, Message = _localizer["AllDataImported"] });

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				await DeleteCurrencies(connectionString);
				await DeleteDistricts(connectionString);
				await DeleteCities(connectionString);
				await DeleteStates(connectionString);
				await DeleteCountries(connectionString);
				await ResetTasks();
				return Ok(new ResponseDto() { Success = false, Message = _localizer["AllDataFailed"] });
			}
		}

		[HttpPost]
		[Route("ImportCountriesFromTxt")]
		public async Task<IActionResult> ImportCountriesFromTxt(int companyId)
		{
			return Ok(await ImportCountriesLogic(companyId));
		}

		[HttpPost]
		[Route("ImportStatesFromTxt")]
		public async Task<IActionResult> ImportStatesFromTxt(int companyId)
		{
			return Ok(await ImportStatesLogic(companyId));
		}

		[HttpPost]
		[Route("ImportCitiesFromTxt")]
		public async Task<IActionResult> ImportCitiesFromTxt(int companyId)	
		{
			return Ok(await ImportCitiesLogic(companyId));
		}

		[HttpPost]
		[Route("ImportDistrictsFromTxt")]
		public async Task<IActionResult> ImportDistrictsFromTxt(int companyId)
		{
			return Ok(await ImportDistrictsLogic(companyId));
		}

		[HttpPost]
		[Route("ImportCurrenciesFromTxt")]
		public async Task<IActionResult> ImportCurrenciesFromTxt(int companyId)
		{
			return Ok(await ImportCurrenciesLogic(companyId));
		}

		[NonAction]
		public async Task<ResponseDto> ImportCountriesLogic(int companyId)
		{
			var connectionString = GetConnectionString(companyId);
			try
			{
				var txt = ReadFile.ReadScripts("countries.txt");
				await DatabaseHelper.SqlQuery(txt, connectionString);
				await _databaseTransaction.BeginTransaction();
				await _systemTaskService.UpdateSystemTaskToBeCompleted((int)BasicData.SystemTask.ImportCountries);
				await _databaseTransaction.Commit();
				return new ResponseDto() { Success = true, Message = _localizer["CountriesMessageSuccess"] };
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				await DeleteCountries(connectionString);
				var handleException = new HandelException(_exceptionLocalizer);
				return handleException.Handle(e);
			}
		}

		[NonAction]
		public async Task<ResponseDto> ImportStatesLogic(int companyId)
		{
			var connectionString = GetConnectionString(companyId);
			try
			{
				var hasCountries = _systemTaskService.GetSystemTask((int)BasicData.SystemTask.ImportCountries).IsCompleted;
				if (hasCountries)
				{
					var txt = ReadFile.ReadScripts("states.txt");
					await DatabaseHelper.SqlQuery(txt, connectionString);
					await _databaseTransaction.BeginTransaction();
					await _systemTaskService.UpdateSystemTaskToBeCompleted((int)BasicData.SystemTask.ImportStates);
					await _databaseTransaction.Commit();
					return new ResponseDto() { Success = true, Message = _localizer["StatesMessageSuccess"] };
				}
				else
				{
					return new ResponseDto() { Success = false, Message = _localizer["StatesMessageBeforeCountries"] };
				}

			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				await DeleteStates(connectionString);
				var handleException = new HandelException(_exceptionLocalizer);
				return handleException.Handle(e);
			}
		}
		[NonAction]
		public async Task<ResponseDto> ImportCitiesLogic(int companyId)
		{
			var connectionString = GetConnectionString(companyId);

			try
			{
				var hasStates = _systemTaskService.GetSystemTask((int)BasicData.SystemTask.ImportStates).IsCompleted;
				if (hasStates)
				{
					for (int i = 1; i <= 16; i++)
					{
						var txt = ReadFile.ReadScripts($"Cities{i}.txt");
						await DatabaseHelper.SqlQuery(txt, connectionString);
					}
					await _databaseTransaction.BeginTransaction();
					await _systemTaskService.UpdateSystemTaskToBeCompleted((int)BasicData.SystemTask.ImportCities);
					await _databaseTransaction.Commit();
					return (new ResponseDto() { Success = true, Message = _localizer["CitiesMessageSuccess"] });
				}
				else
				{
					return (new ResponseDto() { Success = false, Message = _localizer["CitiesMessageBeforeStates"] });
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				await DeleteCities(connectionString);
				var handleException = new HandelException(_exceptionLocalizer);
				return (handleException.Handle(e));
			}
		}

		[NonAction]
		public async Task<ResponseDto> ImportDistrictsLogic(int companyId)
		{
			var connectionString = GetConnectionString(companyId);

			try
			{
				var hasDistricts = _systemTaskService.GetSystemTask((int)BasicData.SystemTask.ImportCities).IsCompleted;
				if (hasDistricts)
				{
					for (int i = 1; i <= 1; i++)
					{
						var txt = ReadFile.ReadScripts($"Districts{i}.txt");
						await DatabaseHelper.SqlQuery(txt, connectionString);
					}
					await _databaseTransaction.BeginTransaction();
					await _systemTaskService.UpdateSystemTaskToBeCompleted((int)BasicData.SystemTask.ImportDistricts);
					await _databaseTransaction.Commit();
					return (new ResponseDto() { Success = true, Message = _localizer["DistrictsMessageSuccess"] });
				}
				else
				{
					return (new ResponseDto() { Success = false, Message = _localizer["DistrictsMessageBeforeCities"] });
				}

			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				await DeleteDistricts(connectionString);
				var handleException = new HandelException(_exceptionLocalizer);
				return (handleException.Handle(e));
			}
		}

		[NonAction]
		public async Task<ResponseDto> ImportCurrenciesLogic(int companyId)
		{
			var connectionString = GetConnectionString(companyId);
			try
			{
				var txt = ReadFile.ReadScripts($"currencies.txt");
				await DatabaseHelper.SqlQuery(txt, connectionString);
				await _databaseTransaction.BeginTransaction();
				await _systemTaskService.UpdateSystemTaskToBeCompleted((int)BasicData.SystemTask.ImportCurrencies);
				await _databaseTransaction.Commit();
				return (new ResponseDto() { Success = true, Message = _localizer["CurrenciesMessageSuccess"] });

			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				await DeleteCurrencies(connectionString);
				var handleException = new HandelException(_exceptionLocalizer);
				return (handleException.Handle(e));
			}
		}

		[NonAction]
		public async Task<bool> DeleteCountries(string connectionString)
		{
			await DatabaseHelper.SqlQuery("'delete from countries'", connectionString);
			return true;
		}

		[NonAction]
		public async Task<bool> DeleteStates(string connectionString)
		{
			await DatabaseHelper.SqlQuery("'delete from states'", connectionString);
			return true;
		}

		[NonAction]
		public async Task<bool> DeleteCities(string connectionString)
		{
			await DatabaseHelper.SqlQuery("'delete from cities'", connectionString);
			return true;
		}

		[NonAction]
		public async Task<bool> DeleteDistricts(string connectionString)
		{
			await DatabaseHelper.SqlQuery("'delete from districts'", connectionString);
			return true;
		}

		[NonAction]
		public async Task<bool> DeleteCurrencies(string connectionString)
		{
			await DatabaseHelper.SqlQuery("'delete from currencies'", connectionString);
			return true;
		}

		[NonAction]
		public async Task<bool> ResetTasks()
		{
			try
			{
				await _databaseTransaction.BeginTransaction();
				await _systemTaskService.ResetSystemTasks();
				await _databaseTransaction.Commit();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				await _databaseTransaction.Rollback();
				return false;
			}
		}

		[NonAction]
		public string GetConnectionString(int companyId)
		{
			var baseConnectionString = _configuration.GetConnectionString("albaseet");
			return string.Format(baseConnectionString!, $"albaseet{companyId}");
		}
	}
}
