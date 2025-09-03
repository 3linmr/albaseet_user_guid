using Shared.CoreOne.Contracts.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Archive;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.UserDetail;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Extensions;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;
using Shared.Service.Services.Modules;

namespace Shared.Service.Services.Settings
{
	public class ApplicationSettingService : IApplicationSettingService
	{
		private readonly IApplicationFlagTypeService _applicationFlagTypeService;
		private readonly IApplicationFlagHeaderService _applicationFlagHeaderService;
		private readonly IApplicationFlagDetailService _applicationFlagDetailService;
		private readonly IApplicationFlagDetailSelectService _applicationFlagDetailSelectService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMenuService _menuService;
		private readonly IApplicationFlagDetailCompanyService _applicationFlagDetailCompanyService;
		private readonly IStoreService _storeService;
		private readonly IApplicationFlagTabService _applicationFlagTabService;
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly IStringLocalizer<ApplicationSettingService> _localizer;
		private readonly IMapper _mapper;
		private readonly IApplicationFlagDetailImageService _applicationFlagDetailImageService;
		private readonly IBlobImageService _blobImageService;

		public ApplicationSettingService(IApplicationFlagTypeService applicationFlagTypeService, IApplicationFlagHeaderService applicationFlagHeaderService, IApplicationFlagDetailService applicationFlagDetailService, IApplicationFlagDetailSelectService applicationFlagDetailSelectService, IHttpContextAccessor httpContextAccessor, IMenuService menuService, IApplicationFlagDetailCompanyService applicationFlagDetailCompanyService, IStoreService storeService,IApplicationFlagTabService applicationFlagTabService,ICompanyService companyService,IBranchService branchService,IStringLocalizer<ApplicationSettingService> localizer,IMapper mapper,IApplicationFlagDetailImageService applicationFlagDetailImageService,IBlobImageService blobImageService)
		{
			_applicationFlagTypeService = applicationFlagTypeService;
			_applicationFlagHeaderService = applicationFlagHeaderService;
			_applicationFlagDetailService = applicationFlagDetailService;
			_applicationFlagDetailSelectService = applicationFlagDetailSelectService;
			_httpContextAccessor = httpContextAccessor;
			_menuService = menuService;
			_applicationFlagDetailCompanyService = applicationFlagDetailCompanyService;
			_storeService = storeService;
			_applicationFlagTabService = applicationFlagTabService;
			_companyService = companyService;
			_branchService = branchService;
			_localizer = localizer;
			_mapper = mapper;
			_applicationFlagDetailImageService = applicationFlagDetailImageService;
			_blobImageService = blobImageService;
		}

		public async Task<List<ApplicationFlagTabDto>> GetSettingTabs()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
			await (from header in _applicationFlagTabService.GetAll().OrderBy(x => x.Order)
				   select new ApplicationFlagTabDto
				   {
					   ApplicationFlagTabId = header.ApplicationFlagTabId,
					   ApplicationFlagTabName = language == LanguageCode.Arabic ? header.ApplicationFlagTabNameAr : header.ApplicationFlagTabNameEn
				   }).ToListAsync();
			return data;
		}

		public async Task<List<SettingTabCardDto>> GetSettingTabCards(int? companyId, int applicationFlagTabId)
		{
			var company = companyId == 0 ? null : companyId;
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<SettingTabCardDto>();

			var data = await
				(from applicationFlagHeader in _applicationFlagHeaderService.GetAll().Where(x => x.ApplicationFlagTabId == applicationFlagTabId)
				 select new SettingTabCardDto
				 {
					 ApplicationFlagHeaderId = applicationFlagHeader.ApplicationFlagHeaderId,
					 ApplicationFlagHeaderName = language == LanguageCode.Arabic ? applicationFlagHeader.ApplicationFlagHeaderNameAr : applicationFlagHeader.ApplicationFlagHeaderNameEn,
					 Order = applicationFlagHeader.Order
				 }).ToListAsync();


			var menuDetails =
				await (from applicationFlagHeader in _applicationFlagHeaderService.GetAll().Where(x => x.ApplicationFlagTabId == applicationFlagTabId)
					   from applicationFlagDetail in _applicationFlagDetailService.GetAll().Where(x =>
						   x.ApplicationFlagHeaderId == applicationFlagHeader.ApplicationFlagHeaderId)
					   from applicationFlagDetailCompany in _applicationFlagDetailCompanyService.GetAll().Where(x => x.ApplicationFlagDetailId == applicationFlagDetail.ApplicationFlagDetailId && x.CompanyId == company).DefaultIfEmpty()
					   select new SettingTabCardDetailDto()
					   {
						   ApplicationFlagHeaderId = applicationFlagHeader.ApplicationFlagHeaderId,
						   ApplicationFlagDetailId = applicationFlagDetail.ApplicationFlagDetailId,
						   ApplicationFlagTypeId = applicationFlagDetail.ApplicationFlagTypeId,
						   FlagValue = applicationFlagDetailCompany != null ? applicationFlagDetailCompany.FlagValue : applicationFlagDetail.FlagValue,
						   FlagName = language == LanguageCode.Arabic ? applicationFlagDetail.FlagNameAr : applicationFlagDetail.FlagNameEn,
						   Order = applicationFlagDetail.Order
					   }).ToListAsync();

			var detailSelectData =
				await (from applicationFlagHeader in _applicationFlagHeaderService.GetAll().Where(x => x.ApplicationFlagTabId == applicationFlagTabId)
					   from applicationFlagDetail in _applicationFlagDetailService.GetAll().Where(x =>
						   x.ApplicationFlagHeaderId == applicationFlagHeader.ApplicationFlagHeaderId)
					   from detailSelect in _applicationFlagDetailSelectService.GetAll().Where(x => x.ApplicationFlagDetailId == applicationFlagDetail.ApplicationFlagDetailId)
					   select new SettingTabCardDetailSelectDto()
					   {
						   ApplicationFlagDetailSelectId = detailSelect.ApplicationFlagDetailSelectId,
						   ApplicationFlagHeaderId = applicationFlagHeader.ApplicationFlagHeaderId,
						   ApplicationFlagDetailId = applicationFlagDetail.ApplicationFlagDetailId,
						   SelectId = detailSelect.SelectId,
						   SelectName = language == LanguageCode.Arabic ? detailSelect.SelectNameAr : detailSelect.SelectNameEn,
						   Order = detailSelect.Order
					   }).ToListAsync();

			var detailList = new List<SettingTabCardDetailDto>();

			foreach (var item in data)
			{
				var detail = menuDetails.Where(x => x.ApplicationFlagHeaderId == item.ApplicationFlagHeaderId).ToList().OrderBy(x => x.Order).ToList();
				foreach (var oneDetail in detail)
				{
					var detailSelect = detailSelectData.Where(x => x.ApplicationFlagDetailId == oneDetail.ApplicationFlagDetailId).ToList().OrderBy(x => x.Order).ToList();
					oneDetail.SelectDetail = detailSelect;
					detailList?.Add(oneDetail);
				}
				var model = new SettingTabCardDto()
				{
					ApplicationFlagHeaderId = item.ApplicationFlagHeaderId,
					ApplicationFlagHeaderName = item.ApplicationFlagHeaderName,
					CardList = detailList?.OrderBy(x => x.Order).ToList(),
					Order = item.Order
				};
				modelList.Add(model);
				detailList = new List<SettingTabCardDetailDto>();
			}
			return modelList.OrderBy(x => x.Order).ToList();
		}

		public async Task<string?> GetApplicationSettingValueByCompanyId(int? companyId, int applicationFlagDetailId)
		{
			if (companyId > 0)
			{
				var companyValue = await _applicationFlagDetailCompanyService.GetApplicationFlagDetailCompanyValue(companyId.GetValueOrDefault(), applicationFlagDetailId);
				if (string.IsNullOrEmpty(companyValue))
				{
					return await _applicationFlagDetailService.GetApplicationFlagDetailValue(applicationFlagDetailId);
				}
				else
				{
					return companyValue;
				}
			}
			else
			{
				return await _applicationFlagDetailService.GetApplicationFlagDetailValue(applicationFlagDetailId);
			}
		}

		public async Task<string?> GetApplicationSettingValueByStoreId(int? storeId, int applicationFlagDetailId)
		{
			if (storeId > 0)
			{
				var companyValue = await _applicationFlagDetailCompanyService.GetApplicationFlagDetailCompanyValueByStoreId(storeId.GetValueOrDefault(), applicationFlagDetailId);
				if (string.IsNullOrEmpty(companyValue))
				{
					return await _applicationFlagDetailService.GetApplicationFlagDetailValue(applicationFlagDetailId);
				}
				else
				{
					return companyValue;
				}
			}
			else
			{
				return await _applicationFlagDetailService.GetApplicationFlagDetailValue(applicationFlagDetailId);
			}
		}

		public async Task<ResponseDto> SaveApplicationSetting(SaveApplicationSettingDto model)
		{
			if (model.CompanyId > 0)
			{
				return await _applicationFlagDetailCompanyService.SaveApplicationCompanySetting(model);
			}
			else
			{
				return await _applicationFlagDetailService.SaveApplicationSetting(model);
			}
		}

		private async Task<bool> GetApplicationSettingValueByStoreIdBoolean(int storeId, int applicationFlagDetail)
		{
			return await GetApplicationSettingValueByStoreId(storeId, applicationFlagDetail) == "1";
		}

		public async Task<bool> SeparateYears(int storeId) => await GetApplicationSettingValueByStoreIdBoolean(storeId, ApplicationSettingDetailData.DocumentsSequence);
		public async Task<bool> SeparateSellers(int storeId) => await GetApplicationSettingValueByStoreIdBoolean(storeId, ApplicationSettingDetailData.SeparatingTheSequenceOfInvoicesForEachSeller);
		public async Task<bool> SeparateCashFromCreditPurchaseInvoice(int storeId) => await GetApplicationSettingValueByStoreIdBoolean(storeId, ApplicationSettingDetailData.SeparatingCashFromCredit);
		public async Task<bool> SeparateCashFromCreditPurchaseInvoiceReturn(int storeId) => await GetApplicationSettingValueByStoreIdBoolean(storeId, ApplicationSettingDetailData.SeparatingCashFromCreditInReturn);
		public async Task<bool> SeparateCashFromCreditSalesInvoice(int storeId) => await GetApplicationSettingValueByStoreIdBoolean(storeId, ApplicationSettingDetailData.SeparatingCashFromCreditSales);
		public async Task<bool> SeparateCashFromCreditSalesInvoiceReturn(int storeId) => await GetApplicationSettingValueByStoreIdBoolean(storeId, ApplicationSettingDetailData.SeparatingCashFromCreditInReturnSales);

		public async Task<bool> ShowItemsGrouped(int storeId)
		{
			var companyId = await _storeService.GetCompanyIdByStoreId(storeId);
			var result = await GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.ShowItemsGrouped);
			return result == "1";
		}

		public IQueryable<SettingTabCardDetailDto> GetSettingDetail()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var data =
				from applicationFlag in _applicationFlagDetailService.GetAll()
				from companyFlag in _applicationFlagDetailCompanyService.GetAll().Where(x=>x.CompanyId == companyId && x.ApplicationFlagDetailId == applicationFlag.ApplicationFlagDetailId).DefaultIfEmpty()
				select new SettingTabCardDetailDto()
				{
					ApplicationFlagHeaderId = applicationFlag.ApplicationFlagHeaderId,
					ApplicationFlagTypeId = applicationFlag.ApplicationFlagTypeId,
					ApplicationFlagDetailId = applicationFlag.ApplicationFlagDetailId,
					FlagValue = companyFlag != null ? companyFlag.FlagValue : applicationFlag.FlagValue,
					FlagName = applicationFlag.FlagValue
				};
			return data;
		}

		public IQueryable<SettingTabCardDetailDto> GetSettingDetail(int applicationFlagHeaderId)
		{
			return GetSettingDetail().Where(x => x.ApplicationFlagHeaderId == applicationFlagHeaderId);
		}

		public async Task<PrintSettingVm> GetPrintSetting()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var flags = await 
				(from applicationFlagTab in _applicationFlagTabService.GetAll().Where(x=>x.ApplicationFlagTabId == ApplicationFlagTabData.PrintSetting)
				from applicationFlagHeader in _applicationFlagHeaderService.GetAll().Where(x=>x.ApplicationFlagTabId == applicationFlagTab.ApplicationFlagTabId) 
				from applicationFlagDetail in _applicationFlagDetailService.GetAll().Where(x=>x.ApplicationFlagHeaderId == applicationFlagHeader.ApplicationFlagHeaderId)
				from companyFlag in _applicationFlagDetailCompanyService.GetAll().Where(x => x.CompanyId == companyId && x.ApplicationFlagDetailId == applicationFlagDetail.ApplicationFlagDetailId).DefaultIfEmpty()
				select new ApplicationFLagValueDto()
				{
					ApplicationFlagDetailId = applicationFlagDetail.ApplicationFlagDetailId,
					ApplicationFlagValue = companyFlag != null ? companyFlag.FlagValue : applicationFlagDetail.FlagValue,
				}).ToListAsync();

			var logo = await ViewApplicationCompanyLogo(ApplicationSettingDetailData.Logo);


			var printSetting = new PrintSettingDto()
			{
				InstitutionNameAr = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InstitutionNameAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				InstitutionNameEn = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InstitutionNameEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				InstitutionOtherNameAr = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InstitutionOtherNameAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				InstitutionOtherNameEn = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InstitutionOtherNameEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				Address1Ar = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address1Ar)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				Address1En = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address1En)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				Address2Ar = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address2Ar)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				Address2En = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address2En)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				Address3Ar = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address3Ar)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				Address3En = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address3En)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				BottomNotesAllPagesAr = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BottomNotesAllPagesAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				BottomNotesAllPagesEn = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BottomNotesAllPagesEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				BottomNotesLastPageOnlyAr = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BottomNotesLastPageOnlyAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				BottomNotesLastPageOnlyEn = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BottomNotesLastPageOnlyEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				TopNotesAllPagesAr = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.TopNotesAllPagesAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				TopNotesAllPagesEn = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.TopNotesAllPagesEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				TopNotesFirstPageOnlyAr = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.TopNotesFirstPageOnlyAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				TopNotesFirstPageOnlyEn = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.TopNotesFirstPageOnlyEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				BottomNotesAllPagesFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BottomNotesAllPagesFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				BottomNotesLastPageOnlyFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BottomNotesFirstPageOnlyFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				Address1Font = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address1Font)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				Address2Font = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address2Font)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				Address3Font = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.Address3Font)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				BusinessFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BusinessFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				ReportPeriodFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ReportPeriodFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				DetailRounding = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.DetailRounding)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				GridFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.GridFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				InstitutionNameFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InstitutionNameFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				InstitutionOtherNameFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InstitutionOtherNameFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				ReportNameFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ReportNameFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				SumRounding = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SumRounding)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				TopNotesAllPagesFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.TopNotesAllPagesFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				TopNotesFirstPageOnlyFont = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.TopNotesFirstPageOnlyFont)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				PrintBusinessName = (flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.PrintBusinessName)?.ApplicationFlagValue ?? "1") == "1",
				PrintUserName = (flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.PrintUserName)?.ApplicationFlagValue ?? "0") == "1",
				PrintDateTime = (flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.PrintDateTime)?.ApplicationFlagValue ?? "1") == "1",
				PrintInstitutionLogo = (flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.PrintInstitutionLogo)?.ApplicationFlagValue ?? "1") == "1",
				Logo = "",
				PrintFormId = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.PrintFormId)?.ApplicationFlagValue),
				TopMargin = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.TopMargin)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.TopMargin),
				BottomMargin = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BottomMargin)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.BottomMargin),
				RightMargin = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.RightMargin)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.RightMargin),
				LeftMargin = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.LeftMargin)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.LeftMargin)
			};
			var businessName = await GetCurrentUserBusiness();

			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var returnedData = _mapper.Map<PrintSettingVm>(printSetting);
			returnedData.Address1 = language == LanguageCode.Arabic ? printSetting.Address1Ar : printSetting.Address1En;
			returnedData.Address2 = language == LanguageCode.Arabic ? printSetting.Address2Ar : printSetting.Address2En;
			returnedData.Address3 = language == LanguageCode.Arabic ? printSetting.Address3Ar : printSetting.Address3En;
			returnedData.InstitutionName = language == LanguageCode.Arabic ? printSetting.InstitutionNameAr : printSetting.InstitutionNameEn;
			returnedData.InstitutionOtherName = language == LanguageCode.Arabic ? printSetting.InstitutionOtherNameAr : printSetting.InstitutionOtherNameEn;
			returnedData.TopNotesFirstPageOnly = language == LanguageCode.Arabic ? printSetting.TopNotesFirstPageOnlyAr : printSetting.TopNotesFirstPageOnlyEn;
			returnedData.TopNotesAllPages = language == LanguageCode.Arabic ? printSetting.TopNotesAllPagesAr : printSetting.TopNotesAllPagesEn;
			returnedData.BottomNotesLastPageOnly = language == LanguageCode.Arabic ? printSetting.BottomNotesLastPageOnlyAr : printSetting.BottomNotesLastPageOnlyEn;
			returnedData.BottomNotesAllPages = language == LanguageCode.Arabic ? printSetting.BottomNotesAllPagesAr : printSetting.BottomNotesAllPagesEn;
			returnedData.BusinessName = businessName;
			returnedData.Logo = logo != null ? $"data:{logo.ContentType};base64,{logo.ImageData}" : null;

			return returnedData;
		}

		public async Task<InvoiceSettingDto> GetInvoicePrintSetting()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var flags = await
				(from applicationFlagTab in _applicationFlagTabService.GetAll().Where(x => x.ApplicationFlagTabId == ApplicationFlagTabData.PrintInvoiceSetting)
					from applicationFlagHeader in _applicationFlagHeaderService.GetAll().Where(x => x.ApplicationFlagTabId == applicationFlagTab.ApplicationFlagTabId)
					from applicationFlagDetail in _applicationFlagDetailService.GetAll().Where(x => x.ApplicationFlagHeaderId == applicationFlagHeader.ApplicationFlagHeaderId)
					from companyFlag in _applicationFlagDetailCompanyService.GetAll().Where(x => x.CompanyId == companyId && x.ApplicationFlagDetailId == applicationFlagDetail.ApplicationFlagDetailId).DefaultIfEmpty()
					select new ApplicationFLagValueDto()
					{
						ApplicationFlagDetailId = applicationFlagDetail.ApplicationFlagDetailId,
						ApplicationFlagValue = companyFlag != null ? companyFlag.FlagValue : applicationFlagDetail.FlagValue,
					}).ToListAsync();

			var logo = await ViewApplicationCompanyLogo(ApplicationSettingDetailData.InstitutionLogo);

			var printSetting = new InvoiceSettingDto()
			{
				InstitutionName = language == LanguageCode.Arabic ? flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InvoiceInstitutionNameAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces() : flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InvoiceInstitutionNameEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				InstitutionAddress = language == LanguageCode.Arabic ? flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InvoiceInstitutionAddressAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces() : flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InvoiceInstitutionAddressEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				InstitutionContact = language == LanguageCode.Arabic ? flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InvoiceInstitutionContactInfoAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces() : flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.InvoiceInstitutionContactInfoEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				InstitutionLogo = logo != null ? $"data:{logo.ContentType};base64,{logo.ImageData}" : null,
				StoreName = language == LanguageCode.Arabic ? flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.StoreNameAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces() : flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.StoreNameEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				StoreAddress = language == LanguageCode.Arabic ? flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.StoreAddressAr)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces() : flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.StoreAddressEn)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				VatNo = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.VatNo)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterBoldLine1 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BasicNote1)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterBoldLine2 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BasicNote2)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterBoldLine3 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.BasicNote3)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine1 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote1)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine2 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote2)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine3 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote3)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine4 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote4)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine5 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote5)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine6 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote6)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine7 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote7)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine8 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote8)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine9 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote9)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				FooterLine10 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.SideNote10)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				ClosureFooterLine1 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ClosingLine1)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				ClosureFooterLine2 = flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ClosingLine2)?.ApplicationFlagValue?.ToNullIfEmptyOrWhiteSpaces(),
				RoundNumberInTable = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.NumbersInGrid)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				RoundNumberInFooter = Convert.ToInt32(flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.NumbersInSummation)?.ApplicationFlagValue ?? ApplicationSettingDefaultData.FontSize),
				ShowDueDate = (flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ShowDueDate)?.ApplicationFlagValue ?? "1") == "1",
				ShowPaymentType = (flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ShowCollectionMethod)?.ApplicationFlagValue ?? "1") == "1",
				ShowPrintTime = (flags.FirstOrDefault(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ShowPrintTime)?.ApplicationFlagValue ?? "1") == "1",
			};

			return printSetting;
		}

		public async Task<string> GetCurrentUserBusiness()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var storeId = _httpContextAccessor.GetCurrentUserStore();
			var companyDef = _localizer["Company"].Value;
			var branchDef = _localizer["Branch"].Value;
			var storeDef = _localizer["Store"].Value;

			var data =
			 await (from store in _storeService.GetAll().Where(x=>x.StoreId == storeId)
				from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
				from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
				select new 
				{
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn
				}).FirstOrDefaultAsync();
			return $"{companyDef}{data?.CompanyName} - {branchDef}{data?.BranchName} - {storeDef}{data?.StoreName}";
		}

		public async Task<ApplicationFlagDetailImageDto?> GetApplicationPhoto(int companyId, int applicationFlagDetailId)
		{
			var data =
				await(from applicationFlagDetail in _applicationFlagDetailService.GetAll().Where(x => x.ApplicationFlagDetailId == applicationFlagDetailId)
					from applicationFlagDetailCompany in _applicationFlagDetailCompanyService.GetAll().Where(x => x.ApplicationFlagDetailId == applicationFlagDetail.ApplicationFlagDetailId && x.CompanyId == companyId)
					from applicationFlagDetailImage in _applicationFlagDetailImageService.GetAll().Where(x => x.ApplicationFlagDetailCompanyId == applicationFlagDetailCompany.ApplicationFlagDetailCompanyId)
					select new ApplicationFlagDetailImageDto
					{
						FileName = applicationFlagDetailImage.FileName,
						FileType = applicationFlagDetailImage.FileType,
						ImageBinary = applicationFlagDetailImage.Image,
						ApplicationFlagDetailCompanyId = applicationFlagDetailImage.ApplicationFlagDetailCompanyId,
						UserNameCreated = applicationFlagDetailImage.UserNameCreated,
						CreatedAt = applicationFlagDetailImage.CreatedAt,
						ApplicationFlagDetailImageId = applicationFlagDetailImage.ApplicationFlagDetailImageId
					}).FirstOrDefaultAsync();
			return data;
		}

		public async Task<ApplicationFlagDetailImageDto?> GetApplicationCompanyLogo()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return await GetApplicationPhoto(companyId, ApplicationSettingDetailData.Logo);
		}

		public async Task<ImageBase64Dto?> ViewApplicationPhoto(int companyId, int applicationFlagDetailId)
		{
			var data =
				await(from applicationFlagDetail in _applicationFlagDetailService.GetAll().Where(x => x.ApplicationFlagDetailId == applicationFlagDetailId)
					from applicationFlagDetailCompany in _applicationFlagDetailCompanyService.GetAll().Where(x => x.ApplicationFlagDetailId == applicationFlagDetail.ApplicationFlagDetailId && x.CompanyId == companyId)
					from applicationFlagDetailImage in _applicationFlagDetailImageService.GetAll().Where(x => x.ApplicationFlagDetailCompanyId == applicationFlagDetailCompany.ApplicationFlagDetailCompanyId)
					select new ApplicationFlagDetailImageDto
					{
						FileName = applicationFlagDetailImage.FileName,
						FileType = applicationFlagDetailImage.FileType,
						ImageBinary = applicationFlagDetailImage.Image,
						ApplicationFlagDetailCompanyId = applicationFlagDetailImage.ApplicationFlagDetailCompanyId,
						UserNameCreated = applicationFlagDetailImage.UserNameCreated,
						CreatedAt = applicationFlagDetailImage.CreatedAt,
						ApplicationFlagDetailImageId = applicationFlagDetailImage.ApplicationFlagDetailImageId
					}).FirstOrDefaultAsync();

			if (data != null)
			{
				if (data.ImageBinary != null)
				{
					var image = _blobImageService.GetImage(data.ImageBinary);
					return new ImageBase64Dto()
					{
						ImageData = image,
						ContentType = data.FileType
					};
				}
			}

			return new ImageBase64Dto();
		}

		public async Task<ImageBase64Dto?> ViewApplicationCompanyLogo(int detailLogoId)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return await ViewApplicationPhoto(companyId, detailLogoId);
		}
	}
}
