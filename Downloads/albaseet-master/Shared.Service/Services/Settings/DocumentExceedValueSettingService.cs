using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Settings;

namespace Shared.Service.Services.Settings
{
	public class DocumentExceedValueSettingService: IDocumentExceedValueSettingService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DocumentExceedValueSettingService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<bool> GetSettingByMenuCode(int menuCode, int storeId)
		{
			var flagId =  menuCode switch
			{
				MenuCodeData.CashSalesInvoice => CashSalesInvoiceFlagData.ExceedDocumentValue,
				MenuCodeData.CreditSalesInvoice => CreditSalesInvoiceFlagData.ExceedDocumentValue,
				MenuCodeData.SalesInvoiceInterim => SalesInvoiceFlagData.ExceedDocumentValue,
				MenuCodeData.CashReservationInvoice => CashReservationInvoiceFlagData.ExceedDocumentValue,
				MenuCodeData.CreditReservationInvoice => CreditReservationInvoiceFlagData.ExceedDocumentValue,
				MenuCodeData.ClientCreditMemo => ClientCreditMemoFlagData.ExceedDocumentValue,
				MenuCodeData.ClientDebitMemo => ClientDebitMemoFlagData.ExceedDocumentValue,

				MenuCodeData.CashSalesInvoiceReturn => CashSalesInvoiceReturnFlagData.ExceedDocumentValue,
				MenuCodeData.CreditSalesInvoiceReturn => CreditSalesInvoiceReturnFlagData.ExceedDocumentValue,
				MenuCodeData.SalesInvoiceReturn => SalesInvoiceReturnFlagData.ExceedDocumentValue,
				//MenuCodeData.ReservationInvoiceCloseOut => ReservationInvoiceCloseOutFlagData.ExceedDocumentValue,

				MenuCodeData.CashPurchaseInvoice => PurchaseInvoiceCashFlagData.ExceedDocumentValue,
				MenuCodeData.CreditPurchaseInvoice => PurchaseInvoiceCreditFlagData.ExceedDocumentValue,
				MenuCodeData.PurchaseInvoiceInterim => PurchaseInvoiceFlagData.ExceedDocumentValue,
				MenuCodeData.PurchaseInvoiceOnTheWayCash => PurchaseInvoiceCashOnTheWayFlagData.ExceedDocumentValue,
				MenuCodeData.PurchaseInvoiceOnTheWayCredit => PurchaseInvoiceCreditOnTheWayFlagData.ExceedDocumentValue,
				MenuCodeData.SupplierCreditMemo => SupplierCreditMemoFlagData.ExceedDocumentValue,
				MenuCodeData.SupplierDebitMemo => SupplierDebitMemoFlagData.ExceedDocumentValue,

				MenuCodeData.CashPurchaseInvoiceReturn => PurchaseInvoiceReturnCashFlagData.ExceedDocumentValue,
				MenuCodeData.CreditPurchaseInvoiceReturn => PurchaseInvoiceReturnCreditFlagData.ExceedDocumentValue,
				MenuCodeData.PurchaseInvoiceReturn => PurchaseInvoiceReturnFlagData.ExceedDocumentValue,
				MenuCodeData.PurchaseInvoiceReturnOnTheWay => PurchaseInvoiceReturnOnTheWayFlagData.ExceedDocumentValue,

				_ => 0
			};

			return await _httpContextAccessor.UserHasRight(flagId, storeId);
		}
	}
}
