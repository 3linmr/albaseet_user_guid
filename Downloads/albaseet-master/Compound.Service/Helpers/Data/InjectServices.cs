using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Domain;
using Compound.CoreOne.Contracts.Approval;
using Compound.Service.Services.Approval;
using Shared.CoreOne;
using Shared.Repository;
using Compound.CoreOne.Contracts.Items;
using Compound.Service.Services.Items;
using Compound.CoreOne.Contracts.Reservation;
using Compound.Service.Services.Reservation;
using Compound.CoreOne.Contracts.InvoiceSettlement;
using Compound.CoreOne.Contracts.Print;
using Compound.Service.Services.InvoiceSettlement;
using Compound.CoreOne.Models.Domain.InvoiceSettlement;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.Service.Services.Reports.Accounting;
using Compound.Service.Services.Reports.CostCenters;
using Compound.Service.Services.Reports.Shared;
using Compound.Service.Services.Reports.Suppliers;
using Compound.Service.Services.Reports.Inventory;
using Compound.CoreOne.Contracts.Reports.CostCenters;
using Compound.CoreOne.Contracts.Reports.Shared;
using Compound.CoreOne.Contracts.Reports.Suppliers;
using Compound.CoreOne.Contracts.Reports.Clients;
using Compound.Service.Services.Reports.Clients;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Compound.CoreOne.Contracts.Reports.SellerCommission;
using Compound.Service.Services.Reports.SellerCommission;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using Compound.Service.Services.Reports.FollowUpReports;
using Compound.Service.Services.Reports.FollowUpDetailReports;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports;
using Compound.Service.Services.Print;
using Compound.Service.Services.Reports.FollowUpCombinedReports;

namespace Compound.Service.Helpers.Data
{
    public static class InjectServices
    {
        public static IServiceCollection InjectCompoundServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IRedirectApproveRequestService, RedirectApproveRequestService>();
            services.AddScoped<IRedirectDeclinedRequestService, RedirectDeclinedRequestService>();

            services.AddScoped<IItemHandlingService, ItemHandlingService>();

            services.AddScoped<IReservationInvoiceService, ReservationInvoiceService>();
            services.AddScoped<IStockOutFromReservationService, StockOutFromReservationService>();
            services.AddScoped<IStockOutReturnFromReservationService, StockOutReturnFromReservationService>();
            services.AddScoped<IReservationInvoiceCloseOutService, ReservationInvoiceCloseOutService>();

            services.AddScoped<ISalesInvoiceSettlementService, SalesInvoiceSettlementService>();
            services.AddScoped(typeof(IRepository<SalesInvoiceSettlement>), typeof(Repository<SalesInvoiceSettlement>));

            services.AddScoped<IPurchaseInvoiceSettlementService, PurchaseInvoiceSettlementService>();
            services.AddScoped(typeof(IRepository<PurchaseInvoiceSettlement>), typeof(Repository<PurchaseInvoiceSettlement>));
           

            #region Reports
            services.AddScoped<IGeneralInvoiceService, GeneralInvoiceService>();
            services.AddScoped<IGeneralInvoiceDetailService, GeneralInvoiceDetailService>();
            services.AddScoped<IGeneralInventoryDocumentService, GeneralInventoryDocumentService>();

            services.AddScoped<IGeneralJournalReportService, GeneralJournalReportService>();
            services.AddScoped<IAccountStatementReportService, AccountStatementReportService>();

          //  services.AddScoped<IIincomeStatmentservice, IncomeStatmentService>();
            services.AddScoped<IStoreIncomeReportService, StoreIncomeReportService>();
            services.AddScoped<IPaymentMethodsIncomeReportService, PaymentMethodsIncomeReportService>();
            
            services.AddScoped<IMissingDocumentCodesReportService, MissingDocumentCodesReportService>();

            services.AddScoped<IIndividualCostCenterReportService, IndividualCostCenterReportService>();
            services.AddScoped<IMainCostCenterReportService, MainCostCenterReportService>();
            services.AddScoped<ICostCenterJournalReportService, CostCenterJournalReportService>();
            services.AddScoped<ICostCenterAccountsReportService, CostCenterAccountsReportService>();

            services.AddScoped<IAgeingPurchaseInvoiceReportService, AgeingPurchaseInvoiceReportService>();
            services.AddScoped<IUnSettledOrSettledPurchaseInvoiceReportService, UnSettledOrSettledPurchaseInvoiceReportService>();
            services.AddScoped<IPurchaseInvoiceSettlementsReportService, PurchaseInvoiceSettlementsReportService>();
            services.AddScoped<ISuppliersExceedCreditLimitReportService, SuppliersExceedCreditLimitReportService>();
            services.AddScoped<IPurchaseInvoicesDueWithinPeriodReportService, PurchaseInvoicesDueWithinPeriodReportService>();
            services.AddScoped<IPurchaseTradingActivityReportService, PurchaseTradingActivityReportService>();

            services.AddScoped<IAccountBalanceReportService, AccountBalanceReportService>();


            services.AddScoped<IAgeingSalesInvoiceReportService, AgeingSalesInvoiceReportService>();
            services.AddScoped<IUnSettledOrSettledSalesInvoicesReportService, UnSettledOrSettledSalesInvoiceReportService>();
            services.AddScoped<ISalesInvoiceSettlementsReportService, SalesInvoiceSettlementsReportService>();
            services.AddScoped<IClientsExceedCreditLimitReportService, ClientsExceedCreditLimitReportService>();
            services.AddScoped<ISalesInvoicesDueWithinPeriodReportService, SalesInvoicesDueWithinPeriodReportService>();
            services.AddScoped<ILowProfitSalesInvoicesReportService, LowProfitSalesInvoicesReportService>();
            services.AddScoped<ISalesInvoicesWithDiscountReportService, SalesInvoicesWithDiscountReportService>();
            services.AddScoped<IClientsProfitReportService, ClientsProfitReportService>();
            services.AddScoped<ITopSellingClientsWithAmountsReportService, TopSellingClientsWithAmountsReportService>();
            services.AddScoped<ITopSellingClientsInvoicesReportService, TopSellingClientsInvoicesReportService>();
            services.AddScoped<ITotalClientSalesService, TotalClientSalesService>();
            services.AddScoped<ISalesTradingActivityReportService, SalesTradingActivityReportService>();

            services.AddScoped<ISellerCommissionReportService, SellerCommissionReportService>();

            services.AddScoped<IStockTakingsReportService, StockTakingsReportService>();
            services.AddScoped<IInventoryValueReportService, InventoryValueReportService>();
            services.AddScoped<IItemsMovementReportService, ItemsMovementReportService>();
            services.AddScoped<IItemDetailMovementReportService, ItemDetailMovementReportService>();
            services.AddScoped<IItemsTradingMovementReportService, ItemsTradingMovementReportService>();
            services.AddScoped<IItemsExpireReportService, ItemsExpireReportService>();
            services.AddScoped<IStagnantItemsReportService, StagnantItemsReportService>();
            services.AddScoped<IItemsProfitReportService, ItemsProfitReportService>();
            services.AddScoped<IPopularItemsReportService, PopularItemsReportService>();
            services.AddScoped<IItemsReorderReportService, ItemsReorderReportService>();
            services.AddScoped<ITaxesDetailReportService, TaxesDetailReportService>();
            services.AddScoped<IItemReportDataService, ItemReportDataService>();
            services.AddScoped<ITaxesTotalReportService, TaxesTotalReportService>();
            services.AddScoped<IOtherTaxesTotalReportService, OtherTaxesTotalReportService>();
            services.AddScoped<IProfitAndLossReportService, ProfitAndLossReportService>();
            services.AddScoped<IBalanceSheetReportService, BalanceSheetReportService>();
            //services.AddScoped<IIncomeStatementService, IncomeStatementService>();
            //services.AddScoped<IincomeStatmentservice, IncomeStatmentService>();

            services.AddScoped<IProductRequestFollowUpReportService, ProductRequestFollowUpReportService>();
            services.AddScoped<IProductRequestPriceFollowUpReportService, ProductRequestPriceFollowUpReportService>();
            services.AddScoped<ISupplierQuotationFollowUpReportService, SupplierQuotationFollowUpReportService>();
            services.AddScoped<IPurchaseOrderFollowUpReportService, PurchaseOrderFollowUpReportService>();
			services.AddScoped<IStockInFromPurchaseOrderFollowUpReportService, StockInFromPurchaseOrderFollowUpReportService>();
            services.AddScoped<IStockInReturnFromStockInFollowUpReportService, StockInReturnFromStockInFollowUpReportService>();
            services.AddScoped<IPurchaseInvoiceInterimFollowUpReportService, PurchaseInvoiceInterimFollowUpReportService>();
            services.AddScoped<IStockInReturnFromPurchaseInvoiceFollowUpReportService, StockInReturnFromPurchaseInvoiceFollowUpReportService>();
            services.AddScoped<IPurchaseInvoiceOnTheWayFollowUpReportService, PurchaseInvoiceOnTheWayFollowUpReportService>();

            services.AddScoped<IClientPriceRequestFollowUpReportService, ClientPriceRequestFollowUpReportService>();
            services.AddScoped<IClientQuotationFollowUpReportService, ClientQuotationFollowUpReportService>();
            services.AddScoped<IClientQuotationApprovalFollowUpReportService, ClientQuotationApprovalFollowUpReportService>();
            services.AddScoped<IProformaInvoiceFollowUpReportService, ProformaInvoiceFollowUpReportService>();
            services.AddScoped<IStockOutFromProformaInvoiceFollowUpReportService, StockOutFromProformaInvoiceFollowUpReportService>();
            services.AddScoped<IStockOutReturnFromStockOutFollowUpReportService, StockOutReturnFromStockOutFollowUpReportService>();
            services.AddScoped<ISalesInvoiceInterimFollowUpReportService, SalesInvoiceInterimFollowUpReportService>();
            services.AddScoped<IStockOutReturnFromSalesInvoiceFollowUpReportService, StockOutReturnFromSalesInvoiceFollowUpReportService>();
            services.AddScoped<IReservationInvoiceFollowUpReportService, ReservationInvoiceFollowUpReportService>();
            services.AddScoped<ITaxesDetailReportService, TaxesDetailReportService>();

            services.AddScoped<IProductRequestFollowUpDetailReportService, ProductRequestFollowUpDetailReportService>();
            services.AddScoped<IProductRequestPriceFollowUpDetailReportService, ProductRequestPriceFollowUpDetailReportService>();
            services.AddScoped<ISupplierQuotationFollowUpDetailReportService, SupplierQuotationFollowUpDetailReportService>();
            services.AddScoped<IPurchaseOrderFollowUpDetailReportService, PurchaseOrderFollowUpDetailReportService>();
            services.AddScoped<IStockInFromPurchaseOrderFollowUpDetailReportService, StockInFromPurchaseOrderFollowUpDetailReportService>();
            services.AddScoped<IStockInReturnFromStockInFollowUpDetailReportService, StockInReturnFromStockInFollowUpDetailReportService>();
            services.AddScoped<IPurchaseInvoiceInterimFollowUpDetailReportService, PurchaseInvoiceInterimFollowUpDetailReportService>();
            services.AddScoped<IStockInReturnFromPurchaseInvoiceFollowUpDetailReportService, StockInReturnFromPurchaseInvoiceFollowUpDetailReportService>();
            services.AddScoped<IPurchaseInvoiceOnTheWayFollowUpDetailReportService, PurchaseInvoiceOnTheWayFollowUpDetailReportService>();

            services.AddScoped<IClientPriceRequestFollowUpDetailReportService, ClientPriceRequestFollowUpDetailReportService>();
            services.AddScoped<IClientQuotationFollowUpDetailReportService, ClientQuotationFollowUpDetailReportService>();
            services.AddScoped<IClientQuotationApprovalFollowUpDetailReportService, ClientQuotationApprovalFollowUpDetailReportService>();
            services.AddScoped<IProformaInvoiceFollowUpDetailReportService, ProformaInvoiceFollowUpDetailReportService>();
            services.AddScoped<IStockOutFromProformaInvoiceFollowUpDetailReportService, StockOutFromProformaInvoiceFollowUpDetailReportService>();
            services.AddScoped<IStockOutReturnFromStockOutFollowUpDetailReportService, StockOutReturnFromStockOutFollowUpDetailReportService>();
            services.AddScoped<ISalesInvoiceInterimFollowUpDetailReportService, SalesInvoiceInterimFollowUpDetailReportService>();
            services.AddScoped<IStockOutReturnFromSalesInvoiceFollowUpDetailReportService, StockOutReturnFromSalesInvoiceFollowUpDetailReportService>();
            services.AddScoped<IReservationInvoiceFollowUpDetailReportService, ReservationInvoiceFollowUpDetailReportService>();

            services.AddScoped<IProductRequestFollowUpCombinedReportService, ProductRequestFollowUpCombinedReportService>();
            services.AddScoped<IProductRequestPriceFollowUpCombinedReportService, ProductRequestPriceFollowUpCombinedReportService>();
            services.AddScoped<ISupplierQuotationFollowUpCombinedReportService, SupplierQuotationFollowUpCombinedReportService>();
            services.AddScoped<IPurchaseOrderFollowUpCombinedReportService, PurchaseOrderFollowUpCombinedReportService>();
            services.AddScoped<IStockInFromPurchaseOrderFollowUpCombinedReportService, StockInFromPurchaseOrderFollowUpCombinedReportService>();
            services.AddScoped<IStockInReturnFromStockInFollowUpCombinedReportService, StockInReturnFromStockInFollowUpCombinedReportService>();
            services.AddScoped<IPurchaseInvoiceInterimFollowUpCombinedReportService, PurchaseInvoiceInterimFollowUpCombinedReportService>();
            services.AddScoped<IStockInReturnFromPurchaseInvoiceFollowUpCombinedReportService, StockInReturnFromPurchaseInvoiceFollowUpCombinedReportService>();
            services.AddScoped<IPurchaseInvoiceOnTheWayFollowUpCombinedReportService, PurchaseInvoiceOnTheWayFollowUpCombinedReportService>();

            services.AddScoped<IClientPriceRequestFollowUpCombinedReportService, ClientPriceRequestFollowUpCombinedReportService>();
            services.AddScoped<IClientQuotationFollowUpCombinedReportService, ClientQuotationFollowUpCombinedReportService>();
            services.AddScoped<IClientQuotationApprovalFollowUpCombinedReportService, ClientQuotationApprovalFollowUpCombinedReportService>();
            services.AddScoped<IProformaInvoiceFollowUpCombinedReportService, ProformaInvoiceFollowUpCombinedReportService>();
            services.AddScoped<IStockOutFromProformaInvoiceFollowUpCombinedReportService, StockOutFromProformaInvoiceFollowUpCombinedReportService>();
            services.AddScoped<IStockOutReturnFromStockOutFollowUpCombinedReportService, StockOutReturnFromStockOutFollowUpCombinedReportService>();
            services.AddScoped<ISalesInvoiceInterimFollowUpCombinedReportService, SalesInvoiceInterimFollowUpCombinedReportService>();
            services.AddScoped<IStockOutReturnFromSalesInvoiceFollowUpCombinedReportService, StockOutReturnFromSalesInvoiceFollowUpCombinedReportService>();
            services.AddScoped<IReservationInvoiceFollowUpCombinedReportService, ReservationInvoiceFollowUpCombinedReportService>();
            #endregion


            #region PrintInvoices

            services.AddScoped<IPrintInvoiceService, PrintInvoiceService>();


			#endregion

			return services;
        }
    }
}
