using Compound.CoreOne.Contracts.Print;
using Compound.CoreOne.Models.Dtos.Print;
using Microsoft.VisualBasic;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;

namespace Compound.Service.Services.Print
{
	public class PrintInvoiceService(IApplicationSettingService applicationSettingService,ICompanyService companyService,IStoreService storeService,ISalesInvoiceHeaderService salesInvoiceHeaderService,ISalesInvoiceDetailService salesInvoiceDetailService,ISalesInvoiceCollectionService salesInvoiceCollectionService) : IPrintInvoiceService
	{
		public async Task<SimplifiedInvoiceDto> GetSimplifiedInvoice(int salesInvoiceHeaderId)
		{
			var printSetting = await applicationSettingService.GetInvoicePrintSetting();

			var salesInvoiceHeader = await salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
			var salesInvoiceDetail = await salesInvoiceDetailService.GetSalesInvoiceDetails(salesInvoiceHeaderId);
			var salesInvoiceCollection = await salesInvoiceCollectionService.GetSalesInvoiceCollections(salesInvoiceHeaderId);

			if (salesInvoiceHeader != null && salesInvoiceDetail.Any())
			{
				var taxDetail = await storeService.GetStoreTaxDetails(salesInvoiceHeader.StoreId);
				var varPercent = salesInvoiceDetail.Where(x=>x.VatPercent > 0).Select(x=>x.VatPercent).FirstOrDefault();

				var invoiceHeader = new SimplifiedInvoiceHeaderDto
				{
					InvoiceDate = DateHelper.Combine(salesInvoiceHeader.DocumentDate, salesInvoiceHeader.EntryDate),
					InvoiceNumber = salesInvoiceHeader.DocumentFullCode,
					TotalValue = NumberHelper.RoundNumber(salesInvoiceHeader.TotalValue,printSetting.RoundNumberInFooter),
					VatValue = NumberHelper.RoundNumber(salesInvoiceHeader.VatValue, printSetting.RoundNumberInFooter),
					VatPercent = varPercent,
					DiscountValue = NumberHelper.RoundNumber((salesInvoiceHeader.DiscountValue + salesInvoiceHeader.TotalItemDiscount), printSetting.RoundNumberInFooter),
					NetValue = NumberHelper.RoundNumber(salesInvoiceHeader.NetValue, printSetting.RoundNumberInFooter),
					PaymentType = string.Join(", ", salesInvoiceCollection.Select(p => p.PaymentMethodName).Distinct().OrderBy(n => n)),
					VatNumber = taxDetail.TaxCode,
					InvoiceType = salesInvoiceHeader.MenuName
				};

				var invoiceDetail = salesInvoiceDetail.Select(x => new SimplifiedInvoiceDetailDto
				{
					ItemName = x.ItemName,
					Package = x.ItemPackageName,
					Quantity = NumberHelper.RoundNumber(x.Quantity, printSetting.RoundNumberInTable),
					UnitPrice = NumberHelper.RoundNumber(x.SellingPrice, printSetting.RoundNumberInTable),
					VatValue = NumberHelper.RoundNumber(x.VatValue, printSetting.RoundNumberInTable),
					DiscountValue = NumberHelper.RoundNumber((x.ItemDiscountValue + x.HeaderDiscountValue), printSetting.RoundNumberInTable),
					NetValue = NumberHelper.RoundNumber(x.NetValue, printSetting.RoundNumberInTable)
				}).ToList();


				var returnData = new SimplifiedInvoiceDto
				{
					SimplifiedInvoiceHeader = invoiceHeader,
					SimplifiedInvoiceDetail = invoiceDetail,
					InvoiceSetting = printSetting
				};

				return returnData;
			}
		
			throw new NotImplementedException("Simplified invoice generation is not yet implemented.");
		}

		public async Task<TaxInvoiceDto> GetTaxInvoice(int salesInvoiceHeaderId)
		{
			var printSetting = await applicationSettingService.GetInvoicePrintSetting();


			throw new NotImplementedException("Simplified invoice generation is not yet implemented.");

		}
	}
}
