using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Settings
{
	public class ReportPrintSettingService : BaseService<ReportPrintSetting>, IReportPrintSettingService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ReportPrintSettingService> _localizer;
		private readonly ICompanyService _companyService;
		private readonly IMenuService _menuService;
		private readonly IApplicationSettingService _applicationSettingService;
		private readonly IMapper _mapper;

		public ReportPrintSettingService(IRepository<ReportPrintSetting> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ReportPrintSettingService> localizer, ICompanyService companyService, IMenuService menuService, IApplicationSettingService applicationSettingService,IMapper mapper) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_companyService = companyService;
			_menuService = menuService;
			_applicationSettingService = applicationSettingService;
			_mapper = mapper;
		}

		public IQueryable<ReportPrintSettingDto> GetAllReportPrintSettingData()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from reportPrintSetting in _repository.GetAll()
				from company in _companyService.GetAll().Where(x => x.CompanyId == reportPrintSetting.CompanyId)
				from menu in _menuService.GetAll().Where(x => x.MenuCode == reportPrintSetting.MenuCode)
				select new ReportPrintSettingDto
				{
					MenuCode = reportPrintSetting.MenuCode,
					MenuCodeName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
					CompanyId = reportPrintSetting.CompanyId,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					PrintFormId = reportPrintSetting.PrintFormId,
					PrintBusinessName = reportPrintSetting.PrintBusinessName,
					BottomNotesAllPagesFont = reportPrintSetting.BottomNotesAllPagesFont,
					Address1Ar = reportPrintSetting.Address1Ar,
					Address2Ar = reportPrintSetting.Address2Ar,
					Address3Ar = reportPrintSetting.Address3Ar,
					Address3Font = reportPrintSetting.Address3Font,
					TopMargin = reportPrintSetting.TopMargin,
					RightMargin = reportPrintSetting.RightMargin,
					BottomMargin = reportPrintSetting.BottomMargin,
					LeftMargin = reportPrintSetting.LeftMargin,
					InstitutionNameAr = reportPrintSetting.InstitutionNameAr,
					TopNotesFirstPageOnlyAr = reportPrintSetting.TopNotesFirstPageOnlyAr,
					TopNotesAllPagesAr = reportPrintSetting.TopNotesAllPagesAr,
					TopNotesFirstPageOnlyEn = reportPrintSetting.TopNotesFirstPageOnlyEn,
					BottomNotesLastPageOnlyAr = reportPrintSetting.BottomNotesLastPageOnlyAr,
					BottomNotesLastPageOnlyEn = reportPrintSetting.BottomNotesLastPageOnlyEn,
					PrintDateTime = reportPrintSetting.PrintDateTime,
					PrintUserName = reportPrintSetting.PrintUserName,
					InstitutionNameFont = reportPrintSetting.InstitutionNameFont,
					InstitutionOtherNameFont = reportPrintSetting.InstitutionOtherNameFont,
					TopNotesFirstPageOnlyFont = reportPrintSetting.TopNotesFirstPageOnlyFont,
					TopNotesAllPagesFont = reportPrintSetting.TopNotesAllPagesFont,
					ReportNameFont = reportPrintSetting.ReportNameFont,
					GridFont = reportPrintSetting.GridFont,
					BusinessFont = reportPrintSetting.BusinessFont,
					DetailRounding = reportPrintSetting.DetailRounding,
					SumRounding = reportPrintSetting.SumRounding,
					Address1Font = reportPrintSetting.Address1Font,
					Address2Font = reportPrintSetting.Address2Font,
					Address1En = reportPrintSetting.Address1En,
					Address2En = reportPrintSetting.Address2En,
					Address3En = reportPrintSetting.Address3En,
					ReportPeriodFont = reportPrintSetting.ReportPeriodFont,
					BottomNotesAllPagesAr = reportPrintSetting.BottomNotesAllPagesAr,
					BottomNotesAllPagesEn = reportPrintSetting.BottomNotesAllPagesEn,
					BottomNotesLastPageOnlyFont = reportPrintSetting.BottomNotesLastPageOnlyFont,
					InstitutionNameEn = reportPrintSetting.InstitutionNameEn,
					InstitutionOtherNameAr = reportPrintSetting.InstitutionOtherNameAr,
					InstitutionOtherNameEn = reportPrintSetting.InstitutionOtherNameEn,
					ReportPrintSettingId = reportPrintSetting.ReportPrintSettingId,
					TopNotesAllPagesEn = reportPrintSetting.TopNotesAllPagesEn,
					PrintInstitutionLogo = reportPrintSetting.PrintInstitutionLogo
				};
			return data;
		}

		public async Task<PrintSettingVm?> GetReportPrintSettingData(int companyId, short menuCode)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var reportPrintSetting = await GetAllReportPrintSettingData().Where(x => x.CompanyId == companyId && x.MenuCode == menuCode).FirstOrDefaultAsync();
			if (reportPrintSetting != null)
			{
				var returnedData = _mapper.Map<PrintSettingVm>(reportPrintSetting);
				var logo = await _applicationSettingService.ViewApplicationCompanyLogo(ApplicationSettingDetailData.Logo);
				returnedData.Address1 = language == LanguageCode.Arabic ? reportPrintSetting?.Address1Ar : reportPrintSetting?.Address1En;
				returnedData.Address2 = language == LanguageCode.Arabic ? reportPrintSetting?.Address2Ar : reportPrintSetting?.Address2En;
				returnedData.Address3 = language == LanguageCode.Arabic ? reportPrintSetting?.Address3Ar : reportPrintSetting?.Address3En;
				returnedData.TopNotesFirstPageOnly = language == LanguageCode.Arabic ? reportPrintSetting?.TopNotesFirstPageOnlyAr : reportPrintSetting?.TopNotesFirstPageOnlyEn;
				returnedData.BottomNotesLastPageOnly = language == LanguageCode.Arabic ? reportPrintSetting?.BottomNotesLastPageOnlyAr : reportPrintSetting?.BottomNotesLastPageOnlyEn;
				returnedData.TopNotesAllPages = language == LanguageCode.Arabic ? reportPrintSetting?.TopNotesAllPagesAr : reportPrintSetting?.TopNotesAllPagesEn;
				returnedData.BottomNotesAllPages = language == LanguageCode.Arabic ? reportPrintSetting?.BottomNotesAllPagesAr : reportPrintSetting?.BottomNotesAllPagesEn;
				returnedData.InstitutionName = language == LanguageCode.Arabic ? reportPrintSetting?.InstitutionNameAr : reportPrintSetting?.InstitutionNameEn;
				returnedData.InstitutionOtherName = language == LanguageCode.Arabic ? reportPrintSetting?.InstitutionOtherNameAr : reportPrintSetting?.InstitutionOtherNameEn;
				returnedData.ReportPrintSettingId = reportPrintSetting!.ReportPrintSettingId;
				returnedData.CompanyId = reportPrintSetting.CompanyId;
				returnedData.CompanyName = reportPrintSetting.CompanyName;
				returnedData.MenuCode = reportPrintSetting.MenuCode;
				returnedData.Logo = logo != null ? $"data:{logo.ContentType};base64,{logo.ImageData}" : null;

				return returnedData;
			}
			else
			{
				return null;
			}
		}

		public async Task<PrintSettingVm> GetReportPrintSetting(short? menuCode)
		{
			if (menuCode != null)
			{
				var companyId = _httpContextAccessor.GetCurrentUserCompany();
				return (await GetReportPrintSettingData(companyId, menuCode.GetValueOrDefault()) ?? (await _applicationSettingService.GetPrintSetting()));

			}
			return await _applicationSettingService.GetPrintSetting();
		}

		public async Task<ResponseDto> SaveReportPrintSetting(ReportPrintSettingDto reportPrintSetting)
		{
			if (reportPrintSetting.ReportPrintSettingId == 0)
			{
				return await CreateReportPrintSetting(reportPrintSetting);
			}
			else
			{
				return await UpdateReportPrintSetting(reportPrintSetting);
			}
		}

		public async Task<ResponseDto> CreateReportPrintSetting(ReportPrintSettingDto reportPrintSetting)
		{
			var model = new ReportPrintSetting()
			{
				ReportPrintSettingId = await GetNextId(),
				MenuCode = reportPrintSetting.MenuCode,
				CompanyId = reportPrintSetting.CompanyId,
				PrintFormId = reportPrintSetting.PrintFormId,
				PrintBusinessName = reportPrintSetting.PrintBusinessName,
				BottomNotesAllPagesFont = reportPrintSetting.BottomNotesAllPagesFont,
				Address1Ar = reportPrintSetting.Address1Ar,
				Address2Ar = reportPrintSetting.Address2Ar,
				Address3Ar = reportPrintSetting.Address3Ar,
				Address3Font = reportPrintSetting.Address3Font,
				TopMargin = reportPrintSetting.TopMargin,
				RightMargin = reportPrintSetting.RightMargin,
				BottomMargin = reportPrintSetting.BottomMargin,
				LeftMargin = reportPrintSetting.LeftMargin,
				InstitutionNameAr = reportPrintSetting.InstitutionNameAr,
				TopNotesFirstPageOnlyAr = reportPrintSetting.TopNotesFirstPageOnlyAr,
				TopNotesAllPagesAr = reportPrintSetting.TopNotesAllPagesAr,
				TopNotesFirstPageOnlyEn = reportPrintSetting.TopNotesFirstPageOnlyEn,
				BottomNotesLastPageOnlyAr = reportPrintSetting.BottomNotesLastPageOnlyAr,
				BottomNotesLastPageOnlyEn = reportPrintSetting.BottomNotesLastPageOnlyEn,
				PrintDateTime = reportPrintSetting.PrintDateTime,
				PrintUserName = reportPrintSetting.PrintUserName,
				InstitutionNameFont = reportPrintSetting.InstitutionNameFont,
				InstitutionOtherNameFont = reportPrintSetting.InstitutionOtherNameFont,
				TopNotesFirstPageOnlyFont = reportPrintSetting.TopNotesFirstPageOnlyFont,
				TopNotesAllPagesFont = reportPrintSetting.TopNotesAllPagesFont,
				ReportNameFont = reportPrintSetting.ReportNameFont,
				GridFont = reportPrintSetting.GridFont,
				BusinessFont = reportPrintSetting.BusinessFont,
				DetailRounding = reportPrintSetting.DetailRounding,
				SumRounding = reportPrintSetting.SumRounding,
				Address1Font = reportPrintSetting.Address1Font,
				Address2Font = reportPrintSetting.Address2Font,
				Address1En = reportPrintSetting.Address1En,
				Address2En = reportPrintSetting.Address2En,
				Address3En = reportPrintSetting.Address3En,
				ReportPeriodFont = reportPrintSetting.ReportPeriodFont,
				BottomNotesAllPagesAr = reportPrintSetting.BottomNotesAllPagesAr,
				BottomNotesAllPagesEn = reportPrintSetting.BottomNotesAllPagesEn,
				BottomNotesLastPageOnlyFont = reportPrintSetting.BottomNotesLastPageOnlyFont,
				InstitutionNameEn = reportPrintSetting.InstitutionNameEn,
				InstitutionOtherNameAr = reportPrintSetting.InstitutionOtherNameAr,
				InstitutionOtherNameEn = reportPrintSetting.InstitutionOtherNameEn,
				TopNotesAllPagesEn = reportPrintSetting.TopNotesAllPagesEn,
				PrintInstitutionLogo = reportPrintSetting.PrintInstitutionLogo,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};
			await _repository.Insert(model);
			await _repository.SaveChanges();
			return new ResponseDto { Success = true, Message = _localizer["ReportPrintSettingCreatedSuccessfully"] };
		}

		public async Task<ResponseDto> UpdateReportPrintSetting(ReportPrintSettingDto reportPrintSetting)
		{
			var modelDb = await _repository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.ReportPrintSettingId == reportPrintSetting.ReportPrintSettingId);
			if (modelDb != null)
			{
				var model = new ReportPrintSetting()
				{
					ReportPrintSettingId = reportPrintSetting.ReportPrintSettingId,
					MenuCode = reportPrintSetting.MenuCode,
					CompanyId = reportPrintSetting.CompanyId,
					PrintFormId = reportPrintSetting.PrintFormId,
					PrintBusinessName = reportPrintSetting.PrintBusinessName,
					BottomNotesAllPagesFont = reportPrintSetting.BottomNotesAllPagesFont,
					Address1Ar = reportPrintSetting.Address1Ar,
					Address2Ar = reportPrintSetting.Address2Ar,
					Address3Ar = reportPrintSetting.Address3Ar,
					Address3Font = reportPrintSetting.Address3Font,
					TopMargin = reportPrintSetting.TopMargin,
					RightMargin = reportPrintSetting.RightMargin,
					BottomMargin = reportPrintSetting.BottomMargin,
					LeftMargin = reportPrintSetting.LeftMargin,
					InstitutionNameAr = reportPrintSetting.InstitutionNameAr,
					TopNotesFirstPageOnlyAr = reportPrintSetting.TopNotesFirstPageOnlyAr,
					TopNotesAllPagesAr = reportPrintSetting.TopNotesAllPagesAr,
					TopNotesFirstPageOnlyEn = reportPrintSetting.TopNotesFirstPageOnlyEn,
					BottomNotesLastPageOnlyAr = reportPrintSetting.BottomNotesLastPageOnlyAr,
					BottomNotesLastPageOnlyEn = reportPrintSetting.BottomNotesLastPageOnlyEn,
					PrintDateTime = reportPrintSetting.PrintDateTime,
					PrintUserName = reportPrintSetting.PrintUserName,
					InstitutionNameFont = reportPrintSetting.InstitutionNameFont,
					InstitutionOtherNameFont = reportPrintSetting.InstitutionOtherNameFont,
					TopNotesFirstPageOnlyFont = reportPrintSetting.TopNotesFirstPageOnlyFont,
					TopNotesAllPagesFont = reportPrintSetting.TopNotesAllPagesFont,
					ReportNameFont = reportPrintSetting.ReportNameFont,
					GridFont = reportPrintSetting.GridFont,
					BusinessFont = reportPrintSetting.BusinessFont,
					DetailRounding = reportPrintSetting.DetailRounding,
					SumRounding = reportPrintSetting.SumRounding,
					Address1Font = reportPrintSetting.Address1Font,
					Address2Font = reportPrintSetting.Address2Font,
					Address1En = reportPrintSetting.Address1En,
					Address2En = reportPrintSetting.Address2En,
					Address3En = reportPrintSetting.Address3En,
					ReportPeriodFont = reportPrintSetting.ReportPeriodFont,
					BottomNotesAllPagesAr = reportPrintSetting.BottomNotesAllPagesAr,
					BottomNotesAllPagesEn = reportPrintSetting.BottomNotesAllPagesEn,
					BottomNotesLastPageOnlyFont = reportPrintSetting.BottomNotesLastPageOnlyFont,
					InstitutionNameEn = reportPrintSetting.InstitutionNameEn,
					InstitutionOtherNameAr = reportPrintSetting.InstitutionOtherNameAr,
					InstitutionOtherNameEn = reportPrintSetting.InstitutionOtherNameEn,
					TopNotesAllPagesEn = reportPrintSetting.TopNotesAllPagesEn,
					PrintInstitutionLogo = reportPrintSetting.PrintInstitutionLogo,
					CreatedAt = modelDb?.CreatedAt,
					UserNameCreated = modelDb?.UserNameCreated,
					IpAddressCreated = modelDb?.IpAddressCreated,
					ModifiedAt = DateHelper.GetDateTimeNow(),
					UserNameModified = await _httpContextAccessor!.GetUserName(),
					IpAddressModified = _httpContextAccessor?.GetIpAddress(),
					Hide = false,
				};
				_repository.Update(model);
				await _repository.SaveChanges();
				return new ResponseDto { Success = true, Message = _localizer["ReportPrintSettingUpdatedSuccessfully"] };
			}
			else
			{
				return new ResponseDto { Success = false, Message = _localizer["ReportPrintSettingNotFound"] };
			}
		}

		private async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ReportPrintSettingId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
