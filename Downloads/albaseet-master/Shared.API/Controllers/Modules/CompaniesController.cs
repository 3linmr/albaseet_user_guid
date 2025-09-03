using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CompaniesController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly ICompanyService _companyService;
		private readonly IAccountService _accountService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CompaniesController(IDatabaseTransaction databaseTransaction,IStringLocalizer<HandelException>exLocalizer,ICompanyService companyService,IAccountService accountService,IBranchService branchService,IStoreService storeService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_companyService = companyService;
			_accountService = accountService;
			_branchService = branchService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadCompanies")]
		public IActionResult ReadCompanies(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_companyService.GetAllCompanies(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllCompanies")]
		public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies()
		{
			var allCompanies = await _companyService.GetAllCompanies().ToListAsync();
			return Ok(allCompanies);
		}

		[HttpGet]
		[Route("GetAllCompaniesDropDown")]
		public async Task<ActionResult<IEnumerable<CompanyDropDownDto>>> GetAllCompaniesDropDown()
		{
			var allCompanies = await _companyService.GetAllCompaniesDropDown().ToListAsync();
			return Ok(allCompanies);
		}
		
		[HttpGet]
		[Route("GetAllUserCompaniesDropDown")]
		public async Task<ActionResult<IEnumerable<CompanyDropDownDto>>> GetAllUserCompaniesDropDown()
		{
			var allCompanies = await _companyService.GetAllUserCompaniesDropDown();
			return Ok(allCompanies);
		}
		
		[HttpGet]
		[Route("IsCompanyLimitReached")]
		public async Task<ActionResult<ResponseDto>> IsCompanyLimitReached()
		{
			var response = await _companyService.IsCompanyLimitReached();
			return Ok(response);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCompanyById(int id)
		{
			var companyDb = await _companyService.GetCompanyById(id) ?? new CompanyDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(companyDb.CompanyId, 0);
			return Ok(userCanLook ? companyDb : new CompanyDto());
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] CompanyDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _companyService.SaveCompany(model,false);
				if (response.Success)
				{
					if (model.CompanyId == 0)
					{
						await _accountService.CreateMainAccounts(response.Id, model.CurrencyId);
						var branch = await _branchService.CreateBranchFromCompany(model,response.Id);
						await _storeService.CreateStoreFromBranch(model, branch!.Id,false);
					}
					if (response.Success)
					{
						await _databaseTransaction.Commit();
					}
					else
					{
						await _databaseTransaction.Rollback();
					}
				}
				else
				{
					response.Success = false;
					response.Message = response.Message;
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteCompany(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _companyService.DeleteCompany(id);
				if (response.Success)
				{
					await _databaseTransaction.Commit();
				}
				else
				{
					await _databaseTransaction.Rollback();
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}
	}
}
