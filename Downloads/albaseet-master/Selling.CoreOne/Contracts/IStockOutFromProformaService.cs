using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IStockOutFromProformaService
	{
		Task<StockOutDetailsWithResponseDto> GetStockOutDetailsFromProformaInvoice(int proformaInvoiceHeaderId, int storeId, decimal headerDiscountPercent);
		ResponseDto GenerateResponseMessage(List<IncompleteItemDto> incompleteItemIds);
		List<StockOutDetailDto> RecalculateStockOutDetailValues(List<StockOutDetailDto> stockOutDetails, decimal headerDiscountPercent);
		List<StockOutDetailDto> DistributeQuantityOnStockOutDetails(List<StockOutDetailDto> stockOutDetails, bool useAllKeys);
	}
}
