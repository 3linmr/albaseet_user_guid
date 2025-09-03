using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Settings
{
	public interface IReportPrintSettingService : IBaseService<ReportPrintSetting>
	{
		IQueryable<ReportPrintSettingDto> GetAllReportPrintSettingData();
		Task<PrintSettingVm?> GetReportPrintSettingData(int companyId, short menuCode);
		Task<PrintSettingVm> GetReportPrintSetting(short? menuCode);
		Task<ResponseDto> SaveReportPrintSetting(ReportPrintSettingDto reportPrintSetting);
	}
}
