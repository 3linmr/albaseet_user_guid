using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Settings
{
	public class ReportPrintFormService : BaseService<ReportPrintForm>, IReportPrintFormService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ReportPrintFormService(IRepository<ReportPrintForm> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<List<ReportPrintFormDropDownDto>> GetReportPrintForms()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await _repository.GetAll().Select(x => new ReportPrintFormDropDownDto
			{
				ReportPrintFormId = x.ReportPrintFormId,
				ReportPrintFormName = language == LanguageCode.Arabic ? x.ReportPrintFormNameAr: x.ReportPrintFormNameEn
			}).ToListAsync();
		}
	}
}
