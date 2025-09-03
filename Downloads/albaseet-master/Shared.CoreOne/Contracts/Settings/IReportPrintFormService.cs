using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;

namespace Shared.CoreOne.Contracts.Settings
{
	public interface IReportPrintFormService : IBaseService<ReportPrintForm>
	{
		Task<List<ReportPrintFormDropDownDto>> GetReportPrintForms();
	}
}
