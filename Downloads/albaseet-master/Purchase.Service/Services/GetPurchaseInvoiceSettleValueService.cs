using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Purchases.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Data;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.Service.Services
{
	class GetPurchaseInvoiceSettleValueService : IGetPurchaseInvoiceSettleValueService
	{
		private readonly IConfiguration _configuration;
		private readonly IPurchaseValueService _purchaseValueService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;

		public GetPurchaseInvoiceSettleValueService(IConfiguration configuration, IPurchaseValueService purchaseValueService, IGenericMessageService genericMessageService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService)
		{
			_configuration = configuration;
			_purchaseValueService = purchaseValueService;
			_genericMessageService = genericMessageService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
		}

		public async Task<decimal> GetPurchaseInvoiceSettleValue(int purchaseInvoiceHeaderId)
		{
			try
			{
				var compound = _configuration.GetRequiredSection("Application:CompoundAPI").Value;
				var link = $"{compound}/api/PurchaseInvoiceSettlement/GetPurchaseInvoiceSettledValue?purchaseInvoiceHeaderId={purchaseInvoiceHeaderId}";
				return JsonConvert.DeserializeObject<decimal>(await ApiHelper.GetAPI(link));
			}
			catch
			{
				throw new Exception("Could not get purchase invoice settle value");
			}
		}

		public async Task<ResponseDto> CheckSettlementExceedingAndUpdateSettlementCompletedFlag(int purchaseInvoiceHeaderId, int menuCode, int parentMenuCode, bool isSave)
		{
			var invoiceValue = await _purchaseValueService.GetOverallValueOfPurchaseInvoice(purchaseInvoiceHeaderId);
			var settleValue = await GetPurchaseInvoiceSettleValue(purchaseInvoiceHeaderId);

			if (settleValue > invoiceValue)
			{
				return new ResponseDto
				{
					Success = false,
					Message = await _genericMessageService.GetMessage
						(
							menuCode,
							parentMenuCode,
							isSave ? GenericMessageData.CannotSaveBecauseLessThanSettlement : GenericMessageData.CannotDeleteBecauseLessThanSettlment,
							invoiceValue.ToNormalizedString(), settleValue.ToNormalizedString()
						)
				};
			}
			else if (settleValue == invoiceValue)
			{
				await _purchaseInvoiceHeaderService.UpdateIsSettlementCompletedFlags([purchaseInvoiceHeaderId], true);
				return new ResponseDto { Success = true };
			}
			else
			{
				await _purchaseInvoiceHeaderService.UpdateIsSettlementCompletedFlags([purchaseInvoiceHeaderId], false);
				return new ResponseDto { Success = true };
			}
		}

		public async Task<decimal> GetPurchaseInvoiceValueMinusSettledValue(int purchaseInvoiceHeaderId, int supplierDebitMemoId = 0)
		{
			var invoiceValue = await _purchaseValueService.GetOverallValueOfPurchaseInvoiceExceptSupplierDebit(purchaseInvoiceHeaderId, supplierDebitMemoId);
			var settleValue = await GetPurchaseInvoiceSettleValue(purchaseInvoiceHeaderId);

			return invoiceValue - settleValue;
		}
	}
}
