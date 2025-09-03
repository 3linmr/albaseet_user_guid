using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.UserDetail;

namespace Shared.CoreOne.Contracts.Settings
{
	public interface IApplicationSettingService
	{
		Task<List<ApplicationFlagTabDto>> GetSettingTabs();
		Task<List<SettingTabCardDto>> GetSettingTabCards(int? companyId,int applicationFlagTabId);
		Task<string?> GetApplicationSettingValueByCompanyId(int? companyId, int applicationFlagDetailId);
		Task<string?> GetApplicationSettingValueByStoreId(int? storeId, int applicationFlagDetailId);
		Task<ResponseDto> SaveApplicationSetting(SaveApplicationSettingDto model);
		Task<bool> SeparateYears(int storeId);
		Task<bool> SeparateSellers(int storeId);
		Task<bool> SeparateCashFromCreditPurchaseInvoice(int storeId);
		Task<bool> SeparateCashFromCreditPurchaseInvoiceReturn(int storeId);
		Task<bool> SeparateCashFromCreditSalesInvoice(int storeId);
		Task<bool> SeparateCashFromCreditSalesInvoiceReturn(int storeId);
		Task<bool> ShowItemsGrouped(int storeId);
		IQueryable<SettingTabCardDetailDto> GetSettingDetail();
		IQueryable<SettingTabCardDetailDto> GetSettingDetail(int applicationFlagHeaderId);
		Task<PrintSettingVm> GetPrintSetting();
		Task<InvoiceSettingDto> GetInvoicePrintSetting();
		Task<string> GetCurrentUserBusiness();
		Task<ApplicationFlagDetailImageDto?> GetApplicationCompanyLogo();
		Task<ApplicationFlagDetailImageDto?> GetApplicationPhoto(int companyId, int applicationFlagDetailId);
		Task<ImageBase64Dto?> ViewApplicationCompanyLogo(int detailLogoId);
		Task<ImageBase64Dto?> ViewApplicationPhoto(int companyId, int applicationFlagDetailId);
	}
}
