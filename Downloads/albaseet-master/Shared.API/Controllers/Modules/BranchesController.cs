using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class BranchesController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IBranchService _branchService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public BranchesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IBranchService branchService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_branchService = branchService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadBranches")]
		public IActionResult ReadBranches(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_branchService.GetAllBranches(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllBranches")]
		public async Task<ActionResult<IEnumerable<BranchDto>>> GetAllBranches()
		{
			var allBranches = await _branchService.GetAllBranches().ToListAsync();
			return Ok(allBranches);
		}

		[HttpGet]
		[Route("GetAllBranchesDropDown")]
		public async Task<ActionResult<IEnumerable<BranchDropDownDto>>> GetAllBranchesDropDown()
		{
			var allBranches = await _branchService.GetAllBranchesDropDown().ToListAsync();
			return Ok(allBranches);
		}
		
		[HttpGet]
		[Route("GetBranchesByCompanyIdDropDown")]
		public async Task<ActionResult<IEnumerable<BranchDropDownDto>>> GetBranchesByCompanyIdDropDown(int companyId)
		{
			var allBranches = await _branchService.GetBranchesByCompanyIdDropDown(companyId).ToListAsync();
			return Ok(allBranches);
		}
		
		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetBranchById(int id)
		{
			var branchDb = await _branchService.GetBranchById(id) ?? new BranchDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(branchDb.CompanyId, 0);
			return Ok(userCanLook ? branchDb : new BranchDto());
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] BranchDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _branchService.SaveBranch(model);
				if (response.Success)
				{
					await _databaseTransaction.Commit();
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
		public async Task<IActionResult> DeleteBranch(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _branchService.DeleteBranch(id);
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
