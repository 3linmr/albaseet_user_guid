using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Data;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Service.Services
{
	class GetSalesInvoiceSettleValueService : IGetSalesInvoiceSettleValueService
	{
		private readonly IConfiguration _configuration;
		private readonly ISalesValueService _salesValueService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;

		public GetSalesInvoiceSettleValueService(IConfiguration configuration, ISalesValueService salesValueService, IGenericMessageService genericMessageService, ISalesInvoiceHeaderService salesInvoiceHeaderService)
		{
			_configuration = configuration;
			_salesValueService = salesValueService;
			_genericMessageService = genericMessageService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
		}

		public async Task<decimal> GetSalesInvoiceSettleValue(int salesInvoiceHeaderId)
		{
			try
			{
				var compound = _configuration.GetRequiredSection("Application:CompoundAPI").Value;
				var link = $"{compound}/api/SalesInvoiceSettlement/GetSalesInvoiceSettledValue?salesInvoiceHeaderId={salesInvoiceHeaderId}";
				return JsonConvert.DeserializeObject<decimal>(await ApiHelper.GetAPI(link));
			}
			catch
			{
				throw new Exception("Could not get sales invoice settle value");
			}
		}

		public async Task<ResponseDto> CheckSettlementExceedingAndUpdateSettlementCompletedFlag(int salesInvoiceHeaderId, int menuCode, int parentMenuCode, bool isSave)
		{
			var invoiceValue = await _salesValueService.GetOverallValueOfSalesInvoice(salesInvoiceHeaderId);
			var settleValue = await GetSalesInvoiceSettleValue(salesInvoiceHeaderId);

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
			}else if (settleValue == invoiceValue)
			{
				await _salesInvoiceHeaderService.UpdateIsSettlementCompletedFlags([salesInvoiceHeaderId], true);
				return new ResponseDto { Success = true };
			}
			else
			{
				await _salesInvoiceHeaderService.UpdateIsSettlementCompletedFlags([salesInvoiceHeaderId], false);
				return new ResponseDto { Success = true };
			}
		}

		public async Task<decimal> GetSalesInvoiceValueMinusSettledValue(int salesInvoiceHeaderId, int clientCreditMemoId = 0)
		{
			var invoiceValue = await _salesValueService.GetOverallValueOfSalesInvoiceExceptClientCredit(salesInvoiceHeaderId, clientCreditMemoId);
			var settleValue = await GetSalesInvoiceSettleValue(salesInvoiceHeaderId);

			return invoiceValue - settleValue;
		}
	}
}
