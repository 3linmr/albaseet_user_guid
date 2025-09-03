using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;

namespace Shared.API.Controllers.Settings
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ReportPrintSettingsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IReportPrintFormService _reportPrintFormService;
		private readonly IReportPrintSettingService _reportPrintSettingService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;

		public ReportPrintSettingsController( IDatabaseTransaction databaseTransaction, IReportPrintFormService reportPrintFormService, IReportPrintSettingService reportPrintSettingService, IStringLocalizer<HandelException> exLocalizer)
		{
			_databaseTransaction = databaseTransaction;
			_reportPrintFormService = reportPrintFormService;
			_reportPrintSettingService = reportPrintSettingService;
			_exLocalizer = exLocalizer;
		}

		[HttpGet]
		[Route("GetReportPrintFormsDropDown")]
		public async Task<ActionResult<ReportPrintFormDropDownDto>> GetReportPrintFormsDropDown()
		{
			var result = await _reportPrintFormService.GetReportPrintForms();
			return Ok(result);
		}

		[HttpGet]
		[Route("GetReportPrintSetting")]
		public async Task<ActionResult<PrintSettingVm>> GetReportPrintSetting(short? menuCode)
		{
			var result = await _reportPrintSettingService.GetReportPrintSetting(menuCode);
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] ReportPrintSettingDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _reportPrintSettingService.SaveReportPrintSetting(model);
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
