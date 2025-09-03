using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Services.Settings
{
	public class ZeroStockSettingService: IZeroStockSettingService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ZeroStockSettingService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<bool> GetZeroStockSettingByMenuCode(int menuCode, int storeId)
		{
			var flagId =  menuCode switch
			{
				MenuCodeData.CashSalesInvoice => CashSalesInvoiceFlagData.SellWithZeroStock,
				MenuCodeData.CreditSalesInvoice => CreditSalesInvoiceFlagData.SellWithZeroStock,
				MenuCodeData.ProformaInvoice => ProformaInvoiceFlagData.SellWithZeroStock,
				MenuCodeData.SalesInvoiceInterim => SalesInvoiceFlagData.SellWithZeroStock,
				MenuCodeData.CashReservationInvoice => CashReservationInvoiceFlagData.SellWithZeroStock,
				MenuCodeData.CreditReservationInvoice => CreditReservationInvoiceFlagData.SellWithZeroStock,
				MenuCodeData.StockOutFromProformaInvoice => StockOutFromProformaInvoiceFlagData.SellWithZeroStock,
				MenuCodeData.InventoryOut => InventoryOutFlagData.SellWithZeroStock,
				MenuCodeData.InternalTransferItems => InternalTransferFlagData.TransferWithZeroBalance,
				_ => 0
			};

			return await _httpContextAccessor.UserHasRight(flagId, storeId);
		}
	}
}
