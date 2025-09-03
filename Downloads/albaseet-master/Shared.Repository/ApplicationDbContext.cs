using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Domain;
using Compound.CoreOne.Models.Domain.InvoiceSettlement;
using Inventory.CoreOne.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Purchases.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Notifications;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.Helper.Services.Tenant;
using Shared.Repository.Seed;

namespace Shared.Repository
{
    public class ApplicationDbContext : DbContext
    {
	    private readonly ITenantProvider _tenantProvider;
	    private readonly IConfiguration _configuration;

	    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider, IConfiguration configuration) : base(options)
	    {
		    _tenantProvider = tenantProvider;
		    _configuration = configuration;
	    }

        #region SharedDbsets

        //ApprovalModels
        public DbSet<Approve>? Approves { get; set; }
        public DbSet<ApproveStep>? ApproveStep { get; set; }
        public DbSet<ApproveStatus>? ApproveStatuses { get; set; }
        public DbSet<ApproveRequestType>? ApproveRequestTypes { get; set; }
        public DbSet<ApproveRequest>? ApproveRequests { get; set; }
        public DbSet<ApproveRequestDetail>? ApproveRequestDetails { get; set; }
        public DbSet<ApproveRequestUser>? ApproveRequestUsers { get; set; }


        //Basics
        public DbSet<Country>? Countries { get; set; }
        public DbSet<State>? States { get; set; }
        public DbSet<City>? Cities { get; set; }
        public DbSet<District>? Districts { get; set; }
        public DbSet<SystemTask>? SystemTasks { get; set; }
        public DbSet<InvoiceExpenseType>? InvoiceExpenseTypes { get; set; }
        public DbSet<DocumentStatus>? DocumentStatuses { get; set; }
        public DbSet<ShippingStatus>? ShippingStatuses { get; set; }
        public DbSet<InvoiceType>? InvoiceTypes { get; set; }
        public DbSet<EntityType>? EntityTypes { get; set; }

        //Archive
        public DbSet<ArchiveHeader>? ArchiveHeaders { get; set; }
        public DbSet<ArchiveDetail>? ArchiveDetails { get; set; }
        public DbSet<Menu>? Menus { get; set; }
        public DbSet<MenuCompany>? MenuCompanies { get; set; }


        //Modules
        public DbSet<Currency>? Currencies { get; set; }
        public DbSet<CurrencyRate>? CurrencyRates { get; set; }
        public DbSet<Bank>? Banks { get; set; }
        public DbSet<Company>? Companies { get; set; }
        public DbSet<Branch>? Branches { get; set; }
        public DbSet<Store>? Stores { get; set; }
        public DbSet<StoreClassification>? StoreClassifications { get; set; }
        public DbSet<Client>? Clients { get; set; }
        public DbSet<Supplier>? Suppliers { get; set; }
        public DbSet<ColumnIdentifier>? ColumnIdentifiers { get; set; }
        public DbSet<MenuNoteIdentifier>? MenuNoteIdentifiers { get; set; }
        public DbSet<MenuNote>? MenuNotes { get; set; }
        public DbSet<CostCenter>? CostCenters { get; set; }
        public DbSet<Item>? Items { get; set; }
        public DbSet<ItemAttribute>? ItemAttributes { get; set; }
        public DbSet<ItemAttributeType>? ItemAttributeTypes { get; set; }
        public DbSet<ItemBarCode>? ItemBarCodes { get; set; }
        public DbSet<ItemBarCodeDetail>? ItemBarCodeDetails { get; set; }
        public DbSet<ItemPacking>? ItemPacking { get; set; }
        public DbSet<ItemCategory>? ItemCategories { get; set; }
        public DbSet<ItemType>? ItemTypes { get; set; }
        public DbSet<ItemPackage>? ItemPackages { get; set; }
        public DbSet<ItemSection>? ItemSections { get; set; }
        public DbSet<ItemSubCategory>? ItemSubCategories { get; set; }
        public DbSet<ItemSubSection>? ItemSubSections { get; set; }
        public DbSet<MainItem>? MainItems { get; set; }
        public DbSet<ShipmentType>? ShipmentTypes { get; set; }
        public DbSet<TaxType>? TaxTypes { get; set; }
        public DbSet<Tax>? Taxes { get; set; }
        public DbSet<ItemTax>? ItemTaxes { get; set; }
        public DbSet<TaxPercent>? TaxPercents { get; set; }
        public DbSet<Vendor>? Vendors { get; set; }
        public DbSet<DocumentType>? DocumentTypes { get; set; }
        public DbSet<NotificationType>? NotificationTypes { get; set; }
        public DbSet<NotificationHeader>? NotificationHeaders { get; set; }
        public DbSet<NotificationDetail>? NotificationDetails { get; set; }
        public DbSet<PaymentType>? PaymentTypes { get; set; }
        public DbSet<PaymentMethod>? PaymentMethods { get; set; }
        public DbSet<SellerType>? SellerTypes { get; set; }
        public DbSet<Seller>? Sellers { get; set; }
        public DbSet<SellerCommissionType>? SellerCommissionTypes { get; set; }
        public DbSet<SellerCommission>? SellerCommissions { get; set; }
		public DbSet<FixedAssetVoucherType>? FixedAssetVoucherTypes { get; set; }
		public DbSet<FixedAsset>? FixedAssets { get; set; }
		public DbSet<MenuEncoding>? MenuEncodings { get; set; }
        public DbSet<ReportPrintForm>? ReportPrintForms { get; set; }
        public DbSet<ApplicationFlagType>? ApplicationFlagTypes { get; set; }
		public DbSet<ApplicationFlagTab>? ApplicationFlagTabs { get; set; }
		public DbSet<ApplicationFlagHeader>? ApplicationFlagHeaders { get; set; }
		public DbSet<ApplicationFlagDetail>? ApplicationFlagDetails { get; set; }
		public DbSet<ApplicationFlagDetailCompany>? ApplicationFlagDetailCompanies { get; set; }
		public DbSet<ApplicationFlagDetailSelect>? ApplicationFlagDetailSelects { get; set; }
		public DbSet<ApplicationFlagDetailImage>? ApplicationFlagDetailImages { get; set; }
		public DbSet<ReportPrintSetting>? ReportPrintSettings { get; set; }


		#endregion

		#region AccountingDbsets

		public DbSet<AccountLedger>? AccountLedgers { get; set; }
		public DbSet<AccountCategory>? AccountCategories { get; set; }
		public DbSet<AccountType>? AccountTypes { get; set; }
		public DbSet<Account>? Accounts { get; set; }
		public DbSet<CostCenterJournalDetail>? CostCenterJournalDetails { get; set; }
		public DbSet<JournalDetail>? JournalDetails { get; set; }
		public DbSet<JournalHeader>? JournalHeaders { get; set; }
		public DbSet<JournalType>? JournalTypes { get; set; }
		public DbSet<TransactionType>? TransactionTypes { get; set; }
		public DbSet<AccountStoreType>? AccountStoreTypes { get; set; }
		public DbSet<AccountStore>? AccountStore { get; set; }
		public DbSet<ReceiptVoucherHeader>? ReceiptVoucherHeaders { get; set; }
		public DbSet<ReceiptVoucherDetail>? ReceiptVoucherDetails { get; set; }
		public DbSet<ReceiptVoucherInvoice>? ReceiptVoucherInvoices { get; set; }
		public DbSet<PaymentVoucherHeader>? PaymentVoucherHeaders { get; set; }
		public DbSet<PaymentVoucherDetail>? PaymentVoucherDetails { get; set; }
		public DbSet<PaymentVoucherInvoice>? PaymentVoucherInvoices { get; set; }

		public DbSet<FixedAssetVoucherHeader>? FixedAssetVoucherHeaders { get; set; }
		public DbSet<FixedAssetVoucherDetail>? FixedAssetVoucherDetails { get; set; }
		public DbSet<FixedAssetVoucherDetailPayment>? FixedAssetVoucherDetailPayments { get; set; }
		public DbSet<FixedAssetMovementHeader>? FixedAssetMovementHeaders { get; set; }
		public DbSet<FixedAssetMovementDetail>? FixedAssetMovementDetails { get; set; }


		#endregion

		#region InventoryDbsets

		public DbSet<ItemCurrentBalance>? ItemCurrentBalances { get; set; }
		public DbSet<ItemCost>? ItemCosts { get; set; }
		public DbSet<ItemCostUpdateHeader>? ItemCostUpdateHeaders { get; set; }
		public DbSet<ItemCostUpdateDetail>? ItemCostUpdateDetails { get; set; }
		public DbSet<ItemDisassembleHeader>? ItemDisassembleHeaders { get; set; }
		public DbSet<ItemDisassembleDetail>? ItemDisassembleDetails { get; set; }
		public DbSet<ItemDisassembleSerial>? ItemDisassembleSerials { get; set; }
		public DbSet<ItemDisassemble>? ItemDisassembles { get; set; }
		public DbSet<ItemNegativeSalesHeader>? ItemNegativeSalesHeaders { get; set; }
		public DbSet<ItemNegativeSalesDetail>? ItemNegativeSalesDetails { get; set; }

		public DbSet<InternalTransferHeader>? InternalTransferHeaders { get; set; }
		public DbSet<InternalTransferDetail>? InternalTransferDetails { get; set; }
		//public DbSet<ItemImportExcel>? ItemImportExcels { get; set; }
		public DbSet<ItemImportExcelHistory>? ItemImportExcelHistories { get; set; }
		public DbSet<InternalTransferReceiveDetail>? InternalTransferReceiveDetails { get; set; }
		public DbSet<InternalTransferReceiveHeader>? InternalTransferReceiveHeaders { get; set; }
		public DbSet<StockTakingHeader>? StockTakingHeaders { get; set; }
		public DbSet<StockTakingDetail>? StockTakingDetails { get; set; }
		public DbSet<StockTakingCarryOverHeader>? StockTakingCarryOverHeaders { get; set; }
		public DbSet<StockTakingCarryOverDetail>? StockTakingCarryOverDetails { get; set; }
		public DbSet<StockTakingCarryOverEffectDetail>? StockTakingCarryOverEffectDetails { get; set; }
		public DbSet<InventoryInHeader>? InventoryInHeaders { get; set; }
		public DbSet<InventoryInDetail>? InventoryInDetails { get; set; }
		public DbSet<InventoryOutHeader>? InventoryOutHeaders { get; set; }
		public DbSet<InventoryOutDetail>? InventoryOutDetails { get; set; }


		#endregion

		#region PurchasesDbsets

		public DbSet<ProductRequestHeader>? ProductRequestHeaders { get; set; }
		public DbSet<ProductRequestDetail>? ProductRequestDetails { get; set; }
		public DbSet<ProductRequestPriceHeader>? ProductRequestPriceHeaders { get; set; }
		public DbSet<ProductRequestPriceDetail>? ProductRequestPriceDetails { get; set; }
		public DbSet<ProductRequestPriceDetailTax>? ProductRequestPriceDetailTaxes { get; set; }
		public DbSet<SupplierQuotationHeader>? SupplierQuotationHeaders { get; set; }
		public DbSet<SupplierQuotationDetail>? SupplierQuotationDetails { get; set; }
		public DbSet<SupplierQuotationDetailTax>? SupplierQuotationDetailTaxes { get; set; }
		public DbSet<PurchaseOrderHeader>? PurchaseOrderHeaders { get; set; }
		public DbSet<PurchaseOrderDetail>? PurchaseOrderDetails { get; set; }
		public DbSet<PurchaseOrderDetailTax>? PurchaseOrderDetailTaxes { get; set; }
		public DbSet<StockType>? StockTypes { get; set; }
		public DbSet<StockInHeader>? StockInHeaders { get; set; }
		public DbSet<StockInDetail>? StockInDetails { get; set; }
		public DbSet<StockInDetailTax>? StockInDetailTaxes { get; set; }
		public DbSet<StockInReturnHeader>? StockInReturnHeaders { get; set; }
		public DbSet<StockInReturnDetail>? StockInReturnDetails { get; set; }
		public DbSet<StockInReturnDetailTax>? StockInReturnDetailTaxes { get; set; }
		public DbSet<SupplierDebitMemo>? SupplierDebitMemos { get; set; }
		public DbSet<SupplierCreditMemo>? SupplierCreditMemos { get; set; }
		public DbSet<PurchaseInvoiceHeader>? PurchaseInvoiceHeaders { get; set; }
		public DbSet<PurchaseInvoiceDetail>? PurchaseInvoiceDetails { get; set; }
		public DbSet<PurchaseInvoiceDetailTax>? PurchaseInvoiceDetailTaxes { get; set; }
		public DbSet<PurchaseInvoiceExpense>? PurchaseInvoiceExpenses { get; set; }
		public DbSet<PurchaseInvoiceReturnHeader>? PurchaseInvoiceReturnHeaders { get; set; }
		public DbSet<PurchaseInvoiceReturnDetail>? PurchaseInvoiceReturnDetails { get; set; }
		public DbSet<PurchaseInvoiceReturnDetailTax>? PurchaseInvoiceReturnDetailTaxes { get; set; }
		public DbSet<InvoiceStockIn>? InvoiceStockIns { get; set; }
		public DbSet<InvoiceStockInReturn>? InvoiceStockInReturns { get; set; }


		#endregion

		#region SalesDbsets

		public DbSet<ClientPriceRequestHeader>? ClientPriceRequestHeaders { get; set; }
		public DbSet<ClientPriceRequestDetail>? ClientPriceRequestDetails { get; set; }
		public DbSet<ClientQuotationHeader>? ClientQuotationHeaders { get; set; }
		public DbSet<ClientQuotationDetail>? ClientQuotationDetails { get; set; }
		public DbSet<ClientQuotationDetailTax>? ClientQuotationDetailTaxes { get; set; }
		public DbSet<ClientQuotationApprovalHeader>? ClientQuotationApprovalHeaders { get; set; }
		public DbSet<ClientQuotationApprovalDetail>? ClientQuotationApprovalDetails { get; set; }
		public DbSet<ClientQuotationApprovalDetailTax>? ClientQuotationDetailApprovalTaxes { get; set; }
		public DbSet<ProformaInvoiceHeader>? ProformaInvoiceHeaders { get; set; }
		public DbSet<ProformaInvoiceDetail>? ProformaInvoiceDetails { get; set; }
		public DbSet<ProformaInvoiceDetailTax>? ProformaInvoiceDetailTaxes { get; set; }
        public DbSet<StockOutHeader>? StockOutHeaders { get; set; }
        public DbSet<StockOutDetail>? StockOutDetails { get; set; }
        public DbSet<StockOutDetailTax>? StockOutDetailTaxes { get; set; }
        public DbSet<SalesInvoiceHeader>? SalesInvoiceHeaders { get; set; }
		public DbSet<SalesInvoiceDetail>? SalesInvoiceDetails { get; set; }
		public DbSet<SalesInvoiceDetailTax>? SalesInvoiceDetailTaxes { get; set; }
		public DbSet<SalesInvoiceCollection>? SalesInvoiceCollection { get; set; }
        public DbSet<StockOutReturnHeader>? StockOutReturnHeaders { get; set; }
        public DbSet<StockOutReturnDetail>? StockOutReturnDetails { get; set; }
        public DbSet<StockOutReturnDetailTax>? StockOutReturnDetailTaxes { get; set; }
        public DbSet<SalesInvoiceReturnHeader>? SalesInvoiceReturnHeaders { get; set; }
        public DbSet<SalesInvoiceReturnDetail>? SalesInvoiceReturnDetails { get; set; }
        public DbSet<SalesInvoiceReturnDetailTax>? SalesInvoiceReturnDetailTaxes { get; set; }
        public DbSet<SalesInvoiceReturnPayment>? SalesInvoiceReturnPayments { get; set; }
		public DbSet<ClientDebitMemo>? ClientDebitMemos { get; set; }
        public DbSet<ClientCreditMemo>? ClientCreditMemos { get; set; }
		public DbSet<InvoiceStockOut>? InvoiceStockOuts { get; set; }
		public DbSet<InvoiceStockOutReturn>? InvoiceStockOutReturns { get; set; }

		#endregion

		#region CompoundDbSets

		public DbSet<PurchaseInvoiceSettlement> PurchaseInvoiceSettlements { get; set; }
		public DbSet<SalesInvoiceSettlement> SalesInvoiceSettlements { get; set; }

		#endregion


		protected override void OnModelCreating(ModelBuilder builder)
        {
            //Make All Relations Restrict
            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }


			SharedModels.Seed(builder);
			AccountingModels.Seed(builder);

			base.OnModelCreating(builder);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// Ensure tenant info is available.
			if (_tenantProvider?.CurrentTenant != null)
			{
				// Retrieve the base connection string from configuration.
				// The connection string should contain a placeholder {0} for the database name.
				var baseConnectionString = _configuration.GetConnectionString("albaseet");

				// Build the final connection string by inserting the tenant's database name.
				var tenantConnectionString = string.Format(baseConnectionString, _tenantProvider.CurrentTenant.DatabaseName);

				// Specify the MySQL server version (adjust the version accordingly).
				var serverVersion = new MySqlServerVersion(new Version(8, 0, 37));

				// Use MySQL with the dynamic connection string.
				optionsBuilder.UseMySql(tenantConnectionString, serverVersion);
			}

			base.OnConfiguring(optionsBuilder);
		}
	}
}