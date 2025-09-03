using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Shared.CoreOne;
using Purchases.CoreOne.Contracts;
using Purchases.Service.Services;
using Shared.Repository;
using Purchases.CoreOne.Models.Domain;
using Shared.Service.Services.Menus;

namespace Purchases.Service.Helpers.Data
{
    public static class InjectServices
    {
        public static IServiceCollection InjectPurchasesServices(this IServiceCollection services, IConfiguration configuration)
        {
			//ItemCostHandling
			services.AddScoped<IItemCostHandlingService, ItemCostHandlingService>();

			//ProductRequest
			services.AddScoped<IProductRequestHeaderService,ProductRequestHeaderService>();
            services.AddScoped(typeof(IRepository<ProductRequestHeader>), typeof(Repository<ProductRequestHeader>));
            
            services.AddScoped<IProductRequestDetailService,ProductRequestDetailService>();
            services.AddScoped(typeof(IRepository<ProductRequestDetail>), typeof(Repository<ProductRequestDetail>));
            
            services.AddScoped<IProductRequestService,ProductRequestService>();

            //ProductRequestPrice
            services.AddScoped<IProductRequestPriceHeaderService, ProductRequestPriceHeaderService>();
            services.AddScoped(typeof(IRepository<ProductRequestPriceHeader>), typeof(Repository<ProductRequestPriceHeader>));

            services.AddScoped<IProductRequestPriceDetailService, ProductRequestPriceDetailService>();
            services.AddScoped(typeof(IRepository<ProductRequestPriceDetail>), typeof(Repository<ProductRequestPriceDetail>));

            services.AddScoped<IProductRequestPriceDetailTaxService, ProductRequestPriceDetailTaxService>();
            services.AddScoped(typeof(IRepository<ProductRequestPriceDetailTax>), typeof(Repository<ProductRequestPriceDetailTax>));

            services.AddScoped<IProductRequestPriceService, ProductRequestPriceService>();

            //Supplier Quotation
            services.AddScoped<ISupplierQuotationHeaderService, SupplierQuotationHeaderService>();
            services.AddScoped(typeof(IRepository<SupplierQuotationHeader>), typeof(Repository<SupplierQuotationHeader>));

            services.AddScoped<ISupplierQuotationDetailService, SupplierQuotationDetailService>();
            services.AddScoped(typeof(IRepository<SupplierQuotationDetail>), typeof(Repository<SupplierQuotationDetail>));

            services.AddScoped<ISupplierQuotationDetailTaxService, SupplierQuotationDetailTaxService>();
            services.AddScoped(typeof(IRepository<SupplierQuotationDetailTax>), typeof(Repository<SupplierQuotationDetailTax>));

            services.AddScoped<ISupplierQuotationService, SupplierQuotationService>();

            //Purchase Order
            services.AddScoped<IPurchaseOrderHeaderService, PurchaseOrderHeaderService>();
            services.AddScoped(typeof(IRepository<PurchaseOrderHeader>), typeof(Repository<PurchaseOrderHeader>));

            services.AddScoped<IPurchaseOrderDetailService, PurchaseOrderDetailService>();
            services.AddScoped(typeof(IRepository<PurchaseOrderDetail>), typeof(Repository<PurchaseOrderDetail>));

            services.AddScoped<IPurchaseOrderDetailTaxService, PurchaseOrderDetailTaxService>();
            services.AddScoped(typeof(IRepository<PurchaseOrderDetailTax>), typeof(Repository<PurchaseOrderDetailTax>));

            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

            //Purchase Invoice
            services.AddScoped<IPurchaseInvoiceHeaderService, PurchaseInvoiceHeaderService>();
            services.AddScoped(typeof(IRepository<PurchaseInvoiceHeader>), typeof(Repository<PurchaseInvoiceHeader>));

            services.AddScoped<IPurchaseInvoiceDetailService, PurchaseInvoiceDetailService>();
            services.AddScoped(typeof(IRepository<PurchaseInvoiceDetail>), typeof(Repository<PurchaseInvoiceDetail>));

            services.AddScoped<IPurchaseInvoiceDetailTaxService, PurchaseInvoiceDetailTaxService>();
            services.AddScoped(typeof(IRepository<PurchaseInvoiceDetailTax>), typeof(Repository<PurchaseInvoiceDetailTax>));
            
            services.AddScoped<IPurchaseInvoiceExpenseService, PurchaseInvoiceExpenseService>();
            services.AddScoped(typeof(IRepository<PurchaseInvoiceExpense>), typeof(Repository<PurchaseInvoiceExpense>));

            services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();

            services.AddScoped<IPurchaseInvoiceHandlingService, PurchaseInvoiceHandlingService>();

            //Purchase Invoice Returns
            services.AddScoped<IPurchaseInvoiceReturnHeaderService, PurchaseInvoiceReturnHeaderService>();
            services.AddScoped(typeof(IRepository<PurchaseInvoiceReturnHeader>), typeof(Repository<PurchaseInvoiceReturnHeader>));

            services.AddScoped<IPurchaseInvoiceReturnDetailService, PurchaseInvoiceReturnDetailService>();
            services.AddScoped(typeof(IRepository<PurchaseInvoiceReturnDetail>), typeof(Repository<PurchaseInvoiceReturnDetail>));

            services.AddScoped<IPurchaseInvoiceReturnDetailTaxService, PurchaseInvoiceReturnDetailTaxService>();
            services.AddScoped(typeof(IRepository<PurchaseInvoiceReturnDetailTax>), typeof(Repository<PurchaseInvoiceReturnDetailTax>));

            services.AddScoped<IPurchaseInvoiceReturnService, PurchaseInvoiceReturnService>();

            services.AddScoped<IPurchaseInvoiceReturnHandlingService, PurchaseInvoiceReturnHandlingService>();

            //Stock Quantity Service
            services.AddScoped<IStockInQuantityService, StockInQuantityService>();

            //Purchases Value Service
            services.AddScoped<IPurchaseValueService, PurchaseValueService>();

            //Stock In
            services.AddScoped<IStockInHeaderService, StockInHeaderService>();
            services.AddScoped(typeof(IRepository<StockInHeader>), typeof(Repository<StockInHeader>));

            services.AddScoped<IStockInDetailService, StockInDetailService>();
            services.AddScoped(typeof(IRepository<StockInDetail>), typeof(Repository<StockInDetail>));

            services.AddScoped<IStockInDetailTaxService, StockInDetailTaxService>();
            services.AddScoped(typeof(IRepository<StockInDetailTax>), typeof(Repository<StockInDetailTax>));

            services.AddScoped<IStockInService, StockInService>();

            services.AddScoped<IStockInHandlingService, StockInHandlingService>();

            //Stock In Return
            services.AddScoped<IStockInReturnHeaderService, StockInReturnHeaderService>();
            services.AddScoped(typeof(IRepository<StockInReturnHeader>), typeof(Repository<StockInReturnHeader>));

            services.AddScoped<IStockInReturnDetailService, StockInReturnDetailService>();
            services.AddScoped(typeof(IRepository<StockInReturnDetail>), typeof(Repository<StockInReturnDetail>));

            services.AddScoped<IStockInReturnDetailTaxService, StockInReturnDetailTaxService>();
            services.AddScoped(typeof(IRepository<StockInReturnDetailTax>), typeof(Repository<StockInReturnDetailTax>));

            services.AddScoped<IStockInReturnService, StockInReturnService>();

            services.AddScoped<IStockInReturnHandlingService, StockInReturnHandlingService>();

            //Supplier Debit Memo
            services.AddScoped<ISupplierDebitMemoService, SupplierDebitMemoService>();
            services.AddScoped<ISupplierDebitMemoHandlingService, SupplierDebitMemoHandlingService>();
            services.AddScoped(typeof(IRepository<SupplierDebitMemo>), typeof(Repository<SupplierDebitMemo>));

            //Supplier Credit Memo
            services.AddScoped<ISupplierCreditMemoService, SupplierCreditMemoService>();
            services.AddScoped<ISupplierCreditMemoHandlingService, SupplierCreditMemoHandlingService>();
            services.AddScoped(typeof(IRepository<SupplierCreditMemo>), typeof(Repository<SupplierCreditMemo>));

            //SupplierMemoEffects
            services.AddScoped<ISupplierMemoEffectsService, SupplierMemoEffectsService>();

            //PurchaseOrderStatusService
            services.AddScoped<IPurchaseOrderStatusService, PurchaseOrderStatusService>();

            //InvoiceStockInService
            services.AddScoped<IInvoiceStockInService, InvoiceStockInService>();
            services.AddScoped(typeof(IRepository<InvoiceStockIn>), typeof(Repository<InvoiceStockIn>));

			//InvoiceStockInReturnService
			services.AddScoped<IInvoiceStockInReturnService, InvoiceStockInReturnService>();
			services.AddScoped(typeof(IRepository<InvoiceStockInReturn>), typeof(Repository<InvoiceStockInReturn>));

            //Purchase Message Service
            services.AddScoped<IPurchaseMessageService, PurchaseMessageService>();

			// Purchase SettledValue Service
			services.AddScoped<IGetPurchaseInvoiceSettleValueService, GetPurchaseInvoiceSettleValueService>();
			return services;
        }
    }
}
