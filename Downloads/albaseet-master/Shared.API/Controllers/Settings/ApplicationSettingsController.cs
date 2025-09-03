using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;

namespace Shared.API.Controllers.Settings
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ApplicationSettingsController : ControllerBase
	{
		private readonly IApplicationSettingService _applicationSettingService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ApplicationSettingsController(IApplicationSettingService applicationSettingService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_applicationSettingService = applicationSettingService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("GetSettingTabs")]
		public async Task<ActionResult<IEnumerable<ApplicationFlagTabDto>>> GetSettingTabs()
		{
			var tabs = await _applicationSettingService.GetSettingTabs();
			return Ok(tabs);
		}

		[HttpGet]
		[Route("GetSettingTabCards")]
		public async Task<ActionResult<IEnumerable<SettingTabCardDto>>> GetSettingTabCards(int? companyId, int applicationFlagTabId)
		{
			var tabs = await _applicationSettingService.GetSettingTabCards(companyId, applicationFlagTabId);
			return Ok(tabs);
		}

		[HttpGet]
		[Route("ShowItemsGrouped")]
		public async Task<ActionResult<bool>> ShowItemsGrouped(int storeId)
		{
			var result = await _applicationSettingService.ShowItemsGrouped(storeId);
			return Ok(result);
		}

		[HttpGet]
		[Route("GetPrintSetting")]
		public async Task<ActionResult<PrintSettingVm>> GetPrintSetting()
		{
			var result = await _applicationSettingService.GetPrintSetting();
			return Ok(result);
		}


		[HttpPost]
		public async Task<IActionResult> Save([FromBody] SaveApplicationSettingDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _applicationSettingService.SaveApplicationSetting(model);
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

		[HttpPost("UploadImage")]
		public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] int applicationFlagDetailId, [FromForm] int companyId, [FromForm] byte applicationFlagTypeId)
		{
			ResponseDto response;
			try
			{
				//var file = _httpContextAccessor?.HttpContext?.Request.Form.Files[0];
				var model = new SaveApplicationSettingDto() { ApplicationFlagDetailId = applicationFlagDetailId, ApplicationFlagTypeId = applicationFlagTypeId, CompanyId = companyId, File = file };

				await _databaseTransaction.BeginTransaction();
				response = await _applicationSettingService.SaveApplicationSetting(model);
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

		[HttpGet("DownloadImage")]
		public async Task<IActionResult> DownloadImage(int companyId,int applicationFlagDetailId)
		{
			var photo = await _applicationSettingService.GetApplicationPhoto(companyId, applicationFlagDetailId);
			if (photo != null && photo.ImageBinary != null && photo.FileType != null)
			{
				var cd = new System.Net.Mime.ContentDisposition
				{
					FileName = photo.FileName,  // original file name
					Inline = false
				};

				// Add the Content-Disposition header
				Response.Headers.Append("Content-Disposition", cd.ToString());

				// Expose the Content-Disposition header to the client
				Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

				return File(photo.ImageBinary, photo.FileType);
			}

			return NotFound();
		}

		[HttpGet("ViewImage")]
		public async Task<IActionResult> ViewImage(int companyId, int applicationFlagDetailId)
		{
			var photo = await _applicationSettingService.ViewApplicationPhoto(companyId, applicationFlagDetailId);
			return Ok(photo);
		}

	}
}
