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
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.Service.Services;
using Shared.Repository;

namespace Sales.Service.Helpers.Data
{
    public static class InjectServices
    {
        public static IServiceCollection InjectSalesServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Client Price Request
            services.AddScoped<IClientPriceRequestDetailService, ClientPriceRequestDetailService>();
            services.AddScoped(typeof(IRepository<ClientPriceRequestDetail>), typeof(Repository<ClientPriceRequestDetail>));

            services.AddScoped<IClientPriceRequestHeaderService, ClientPriceRequestHeaderService>();
            services.AddScoped(typeof(IRepository<ClientPriceRequestHeader>), typeof(Repository<ClientPriceRequestHeader>));

            services.AddScoped<IClientPriceRequestService, ClientPriceRequestService>();

            // Client Quotation
            services.AddScoped<IClientQuotationDetailService, ClientQuotationDetailService>();
            services.AddScoped(typeof(IRepository<ClientQuotationDetail>), typeof(Repository<ClientQuotationDetail>));

            services.AddScoped<IClientQuotationHeaderService, ClientQuotationHeaderService>();
            services.AddScoped(typeof(IRepository<ClientQuotationHeader>), typeof(Repository<ClientQuotationHeader>));

            services.AddScoped<IClientQuotationDetailTaxService, ClientQuotationDetailTaxService>();
            services.AddScoped(typeof(IRepository<ClientQuotationDetailTax>), typeof(Repository<ClientQuotationDetailTax>));

            services.AddScoped<IClientQuotationService, ClientQuotationService>();

			// Client Quotation Approval
			services.AddScoped<IClientQuotationApprovalDetailService, ClientQuotationApprovalDetailService>();
			services.AddScoped(typeof(IRepository<ClientQuotationApprovalDetail>), typeof(Repository<ClientQuotationApprovalDetail>));

			services.AddScoped<IClientQuotationApprovalHeaderService, ClientQuotationApprovalHeaderService>();
			services.AddScoped(typeof(IRepository<ClientQuotationApprovalHeader>), typeof(Repository<ClientQuotationApprovalHeader>));

			services.AddScoped<IClientQuotationApprovalDetailTaxService, ClientQuotationApprovalDetailTaxService>();
			services.AddScoped(typeof(IRepository<ClientQuotationApprovalDetailTax>), typeof(Repository<ClientQuotationApprovalDetailTax>));

			services.AddScoped<IClientQuotationApprovalService, ClientQuotationApprovalService>();

			// Proforma Invoice
			services.AddScoped<IProformaInvoiceDetailService, ProformaInvoiceDetailService>();
            services.AddScoped(typeof(IRepository<ProformaInvoiceDetail>), typeof(Repository<ProformaInvoiceDetail>));

            services.AddScoped<IProformaInvoiceHeaderService, ProformaInvoiceHeaderService>();
            services.AddScoped(typeof(IRepository<ProformaInvoiceHeader>), typeof(Repository<ProformaInvoiceHeader>));

            services.AddScoped<IProformaInvoiceDetailTaxService, ProformaInvoiceDetailTaxService>();
            services.AddScoped(typeof(IRepository<ProformaInvoiceDetailTax>), typeof(Repository<ProformaInvoiceDetailTax>));

            services.AddScoped<IProformaInvoiceService, ProformaInvoiceService>();
            services.AddScoped<IProformaInvoiceStatusService, ProformaInvoiceStatusService>();

            // Stock Out
            services.AddScoped<IStockOutDetailTaxService, StockOutDetailTaxService>();
            services.AddScoped(typeof(IRepository<StockOutDetailTax>), typeof(Repository<StockOutDetailTax>));

            services.AddScoped<IStockOutDetailService, StockOutDetailService>();
            services.AddScoped(typeof(IRepository<StockOutDetail>), typeof(Repository<StockOutDetail>));

            services.AddScoped<IStockOutHeaderService, StockOutHeaderService>();
            services.AddScoped(typeof(IRepository<StockOutHeader>), typeof(Repository<StockOutHeader>));

            services.AddScoped<IStockOutService, StockOutService>();

            services.AddScoped<IStockOutFromProformaService, StockOutFromProformaService>();

            services.AddScoped<IStockOutHandlingService, StockOutHandlingService>();

            // Stock Out Return
            services.AddScoped<IStockOutReturnDetailTaxService, StockOutReturnDetailTaxService>();
            services.AddScoped(typeof(IRepository<StockOutReturnDetailTax>), typeof(Repository<StockOutReturnDetailTax>));

            services.AddScoped<IStockOutReturnDetailService, StockOutReturnDetailService>();
            services.AddScoped(typeof(IRepository<StockOutReturnDetail>), typeof(Repository<StockOutReturnDetail>));

            services.AddScoped<IStockOutReturnHeaderService, StockOutReturnHeaderService>();
            services.AddScoped(typeof(IRepository<StockOutReturnHeader>), typeof(Repository<StockOutReturnHeader>));

            services.AddScoped<IStockOutReturnService, StockOutReturnService>();

            services.AddScoped<IStockOutReturnHandlingService, StockOutReturnHandlingService>();

            // Stock Out Quantity Service
            services.AddScoped<IStockOutQuantityService, StockOutQuantityService>();

            // Invoice-StockOut Service
            services.AddScoped<IInvoiceStockOutService, InvoiceStockOutService>();
            services.AddScoped(typeof(IRepository<InvoiceStockOut>), typeof(Repository<InvoiceStockOut>));

			// Invoice-StockOutReturn Service
			services.AddScoped<IInvoiceStockOutReturnService, InvoiceStockOutReturnService>();
			services.AddScoped(typeof(IRepository<InvoiceStockOutReturn>), typeof(Repository<InvoiceStockOutReturn>));

			// Sales Invoice
			services.AddScoped<ISalesInvoiceDetailTaxService, SalesInvoiceDetailTaxService>();
            services.AddScoped(typeof(IRepository<SalesInvoiceDetailTax>), typeof(Repository<SalesInvoiceDetailTax>));

            services.AddScoped<ISalesInvoiceDetailService, SalesInvoiceDetailService>();
            services.AddScoped(typeof(IRepository<SalesInvoiceDetail>), typeof(Repository<SalesInvoiceDetail>));

			services.AddScoped<ISalesInvoiceCollectionService, SalesInvoiceCollectionService>();
			services.AddScoped(typeof(IRepository<SalesInvoiceCollection>), typeof(Repository<SalesInvoiceCollection>));

			services.AddScoped<ISalesInvoiceHeaderService, SalesInvoiceHeaderService>();
            services.AddScoped(typeof(IRepository<SalesInvoiceHeader>), typeof(Repository<SalesInvoiceHeader>));

            services.AddScoped<ISalesInvoiceService, SalesInvoiceService>();
            services.AddScoped<IDirectSalesInvoiceFromClientQuotationApprovalService, DirectSalesInvoiceFromClientQuotationApprovalService>();
            services.AddScoped<ISalesInvoiceHandlingService, SalesInvoiceHandlingService>();

            // Sales Invoice Return
            services.AddScoped<ISalesInvoiceReturnDetailTaxService, SalesInvoiceReturnDetailTaxService>();
            services.AddScoped(typeof(IRepository<SalesInvoiceReturnDetailTax>), typeof(Repository<SalesInvoiceReturnDetailTax>));

            services.AddScoped<ISalesInvoiceReturnDetailService, SalesInvoiceReturnDetailService>();
            services.AddScoped(typeof(IRepository<SalesInvoiceReturnDetail>), typeof(Repository<SalesInvoiceReturnDetail>));

            services.AddScoped<ISalesInvoiceReturnHeaderService, SalesInvoiceReturnHeaderService>();
            services.AddScoped(typeof(IRepository<SalesInvoiceReturnHeader>), typeof(Repository<SalesInvoiceReturnHeader>));

			services.AddScoped<ISalesInvoiceReturnPaymentService, SalesInvoiceReturnPaymentService>();
			services.AddScoped(typeof(IRepository<SalesInvoiceReturnPayment>), typeof(Repository<SalesInvoiceReturnPayment>));

			services.AddScoped<ISalesInvoiceReturnService, SalesInvoiceReturnService>();
            services.AddScoped<ISalesInvoiceReturnHandlingService, SalesInvoiceReturnHandlingService>();

            // Client Credit Memo
            services.AddScoped<IClientCreditMemoService, ClientCreditMemoService>();
            services.AddScoped(typeof(IRepository<ClientCreditMemo>), typeof(Repository<ClientCreditMemo>));

            services.AddScoped<IClientCreditMemoHandlingService, ClientCreditMemoHandlingService>();

            // Client Debit Memo
            services.AddScoped<IClientDebitMemoService, ClientDebitMemoService>();
            services.AddScoped(typeof(IRepository<ClientDebitMemo>), typeof(Repository<ClientDebitMemo>));

            services.AddScoped<IClientDebitMemoHandlingService, ClientDebitMemoHandlingService>();

            // Client Memo effects service
            services.AddScoped<IClientMemoEffectsService, ClientMemoEffectsService>();

            // Sales Value Service
            services.AddScoped<ISalesValueService, SalesValueService>();

			//Sales Message Service
			services.AddScoped<ISalesMessageService, SalesMessageService>();

            // Sales SettledValue Service
            services.AddScoped<IGetSalesInvoiceSettleValueService, GetSalesInvoiceSettleValueService>();
			return services;
        }
    }
}
