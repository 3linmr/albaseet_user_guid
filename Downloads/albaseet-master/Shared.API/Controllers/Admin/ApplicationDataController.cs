using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Admin;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Admin;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.UserDetail;
using Shared.Repository;
using Shared.Repository.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace Shared.API.Controllers.Admin
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ApplicationDataController : ControllerBase
	{
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;
		private readonly IApproveDefinitionService _approveDefinitionService;
		private readonly IApplicationDataService _applicationDataService;
		private readonly ApplicationSettingDto _application;
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public ApplicationDataController(ICompanyService companyService,IBranchService branchService,IStoreService storeService,IApproveDefinitionService approveDefinitionService,IApplicationDataService applicationDataService, ApplicationSettingDto application, ApplicationDbContext context,IConfiguration configuration)
		{
			_companyService = companyService;
			_branchService = branchService;
			_storeService = storeService;
			_approveDefinitionService = approveDefinitionService;
			_applicationDataService = applicationDataService;
			_application = application;
			_context = context;
			_configuration = configuration;
		}


		[HttpGet]
		[Route("GetBusinessesDropDown")]
		public async Task<ActionResult<IEnumerable<CompanyDropDownDto>>> GetBusinessesDropDown(int tenantId)
		{
			await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, tenantId);

			var allCompanies = await _companyService.GetAllCompaniesForAdmin().ToListAsync();
			return Ok(allCompanies);
		}

		[HttpGet]
		[Route("GetBranchesDropDown")]
		public async Task<ActionResult<IEnumerable<BranchDropDownDto>>> GetBranchesDropDown(int tenantId,int companyId)
		{
			await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, tenantId);
			var allBranches = await _branchService.GetBranchesDropDownForAdmin(companyId).ToListAsync();
			return Ok(allBranches);
		}


		[HttpGet]
		[Route("GetStoresDropDown")]
		public async Task<ActionResult<IEnumerable<StoreDropDownVm>>> GetStoresDropDown(int tenantId, int branchId)
		{
			await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, tenantId);
			var stores = await _storeService.GetStoresDropDownForAdmin(branchId);
			return Ok(stores);
		}

		[HttpGet]
		[Route("GetApproveTree")]
		public async Task<IActionResult> GetApproveTree(int tenantId, int companyId)
		{
			await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, tenantId);
			var data = await _approveDefinitionService.GetApproveTree(companyId);
			return Ok(data);
		}
		
		
		[HttpGet]
		[Route("GetApplicationValidationData")]
		public async Task<ApplicationValidationDataDto> GetApplicationValidationData(int tenantId)
		{
			await DatabaseInitializer.ChangeDatabaseConnectionStringAsync(_configuration, _context, tenantId);
			var data = await _applicationDataService.GetApplicationValidationData();
			return data;
		}

	}
}
