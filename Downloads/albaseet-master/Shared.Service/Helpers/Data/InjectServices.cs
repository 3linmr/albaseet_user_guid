using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.CoreOne;
using Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Contracts.Basics;
using Shared.Service.Services.Basics;
using Shared.CoreOne.Models.Domain.Basics;
using System.IO;
using System.Reflection;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Localization;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service.Services.Approval;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.Service.Services.Localization;
using Shared.Service.Services.Menus;
using Shared.Service.Services.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Settings;
using Shared.Service.Services.Settings;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.Service.Services.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.Service.Services.Taxes;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.Service.Services.CostCenters;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Admin;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Journal;
using Shared.Service.Services.Accounts;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.Service.Services.Journal;
using Shared.Service.Services.Inventory;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.Service.Services.Admin;
using Microsoft.EntityFrameworkCore.Query;
using Shared.CoreOne.Contracts.Archive;
using Shared.CoreOne.Contracts.FixedAssets;
using Shared.Service.Services.FixedAssets;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.Service.Services.Archive;

namespace Shared.Service.Helpers.Data
{
    public static class InjectServices
    {
        public static IServiceCollection InjectSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            //General
            services.AddScoped<IDatabaseTransaction, DatabaseTransaction>();
            services.AddScoped<ISharedResource, SharedResource>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IApplicationDataService, ApplicationDataService>();


			//Approval
			services.AddScoped<IApproveRequestTypeService, ApproveRequestTypeService>();
			services.AddScoped(typeof(IRepository<ApproveRequestType>), typeof(Repository<ApproveRequestType>));

			services.AddScoped<IApproveService, ApproveService>();
            services.AddScoped(typeof(IRepository<Approve>), typeof(Repository<Approve>));

            services.AddScoped<IApproveStepService, ApproveStepService>();
            services.AddScoped(typeof(IRepository<ApproveStep>), typeof(Repository<ApproveStep>));

            services.AddScoped<IApproveStatusService, ApproveStatusService>();
            services.AddScoped(typeof(IRepository<ApproveStatus>), typeof(Repository<ApproveStatus>));

            services.AddScoped<IApproveRequestService, ApproveRequestService>();
            services.AddScoped(typeof(IRepository<ApproveRequest>), typeof(Repository<ApproveRequest>));

            services.AddScoped<IApproveRequestDetailService, ApproveRequestDetailService>();
			services.AddScoped(typeof(IRepository<ApproveRequestDetail>), typeof(Repository<ApproveRequestDetail>));

			services.AddScoped<IApproveRequestUserService, ApproveRequestUserService>();
            services.AddScoped(typeof(IRepository<ApproveRequestUser>), typeof(Repository<ApproveRequestUser>));

			services.AddScoped<IHandleApprovalRequestService, HandleApprovalRequestService>();
			services.AddScoped<IApproveDefinitionService, ApproveDefinitionService>();

			services.AddScoped<IApprovalSystemService, ApprovalSystemService>();



			//Basics

			services.AddScoped<ISystemTaskService, SystemTaskService>();
            services.AddScoped(typeof(IRepository<SystemTask>), typeof(Repository<SystemTask>));

            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped(typeof(IRepository<Country>), typeof(Repository<Country>));

            services.AddScoped<IStateService, StateService>();
            services.AddScoped(typeof(IRepository<State>), typeof(Repository<State>));

            services.AddScoped<ICityService, CityService>();
            services.AddScoped(typeof(IRepository<City>), typeof(Repository<City>));

            services.AddScoped<ICityService, CityService>();
            services.AddScoped(typeof(IRepository<City>), typeof(Repository<City>));

            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped(typeof(IRepository<District>), typeof(Repository<District>));

            services.AddScoped<IColumnIdentifierService, ColumnIdentifierService>();
            services.AddScoped(typeof(IRepository<ColumnIdentifier>), typeof(Repository<ColumnIdentifier>));

			services.AddScoped<IInvoiceExpenseTypeService, InvoiceExpenseTypeService>();
			services.AddScoped(typeof(IRepository<InvoiceExpenseType>), typeof(Repository<InvoiceExpenseType>));

			services.AddScoped<IDocumentStatusService, DocumentStatusService>();
			services.AddScoped(typeof(IRepository<DocumentStatus>), typeof(Repository<DocumentStatus>));

            services.AddScoped<IShippingStatusService, ShippingStatusService>();
            services.AddScoped(typeof(IRepository<ShippingStatus>), typeof(Repository<ShippingStatus>));

			services.AddScoped<IInvoiceTypeService, InvoiceTypeService>();
			services.AddScoped(typeof(IRepository<InvoiceType>), typeof(Repository<InvoiceType>));

			//Menus
			services.AddScoped<IMenuService, MenuService>();
			services.AddScoped(typeof(IRepository<Menu>), typeof(Repository<Menu>));

			services.AddScoped<IMenuEncodingService, MenuEncodingService>();
			services.AddScoped(typeof(IRepository<MenuEncoding>), typeof(Repository<MenuEncoding>));

			services.AddScoped<IMenuNoteIdentifierService, MenuNoteIdentifierService>();
			services.AddScoped(typeof(IRepository<MenuNoteIdentifier>), typeof(Repository<MenuNoteIdentifier>));

			services.AddScoped<IMenuHelperService, MenuHelperService>();

			services.AddScoped<IGenericMessageService, GenericMessageService>();

			//Settings

			services.AddScoped<IApplicationFlagTypeService, ApplicationFlagTypeService>();
			services.AddScoped(typeof(IRepository<ApplicationFlagType>), typeof(Repository<ApplicationFlagType>));

			services.AddScoped<IApplicationFlagTabService, ApplicationFlagTabService>();
			services.AddScoped(typeof(IRepository<ApplicationFlagTab>), typeof(Repository<ApplicationFlagTab>));

			services.AddScoped<IApplicationFlagHeaderService, ApplicationFlagHeaderService>();
			services.AddScoped(typeof(IRepository<ApplicationFlagHeader>), typeof(Repository<ApplicationFlagHeader>));

			services.AddScoped<IApplicationFlagDetailService, ApplicationFlagDetailService>();
			services.AddScoped(typeof(IRepository<ApplicationFlagDetail>), typeof(Repository<ApplicationFlagDetail>));

			services.AddScoped<IApplicationFlagDetailCompanyService, ApplicationFlagDetailCompanyService>();
			services.AddScoped(typeof(IRepository<ApplicationFlagDetailCompany>), typeof(Repository<ApplicationFlagDetailCompany>));

			services.AddScoped<IApplicationFlagDetailSelectService, ApplicationFlagDetailSelectService>();
			services.AddScoped(typeof(IRepository<ApplicationFlagDetailSelect>), typeof(Repository<ApplicationFlagDetailSelect>));
			
			services.AddScoped<IReportPrintFormService, ReportPrintFormService>();
			services.AddScoped(typeof(IRepository<ReportPrintForm>), typeof(Repository<ReportPrintForm>));
			
			services.AddScoped<IReportPrintSettingService, ReportPrintSettingService>();
			services.AddScoped(typeof(IRepository<ReportPrintSetting>), typeof(Repository<ReportPrintSetting>));
			
			services.AddScoped<IApplicationFlagDetailImageService, ApplicationFlagDetailImageService>();
			services.AddScoped(typeof(IRepository<ApplicationFlagDetailImage>), typeof(Repository<ApplicationFlagDetailImage>));

			services.AddScoped<IApplicationSettingService, ApplicationSettingService>();

			services.AddScoped<IZeroStockSettingService, ZeroStockSettingService>();

			services.AddScoped<IDocumentExceedValueSettingService, DocumentExceedValueSettingService>();

			//Modules
			services.AddScoped<ICompanyService, CompanyService>();
			services.AddScoped(typeof(IRepository<Company>), typeof(Repository<Company>));

			services.AddScoped<IBranchService, BranchService>();
			services.AddScoped(typeof(IRepository<Branch>), typeof(Repository<Branch>));

			services.AddScoped<IStoreService, StoreService>();
			services.AddScoped(typeof(IRepository<Store>), typeof(Repository<Store>));

			services.AddScoped<IStoreClassificationService, StoreClassificationService>();
			services.AddScoped(typeof(IRepository<StoreClassification>), typeof(Repository<StoreClassification>));

			services.AddScoped<IShipmentTypeService ,ShipmentTypeService>();
			services.AddScoped(typeof(IRepository<ShipmentType>), typeof(Repository<ShipmentType>));

			services.AddScoped<ISupplierService, SupplierService>();
			services.AddScoped(typeof(IRepository<Supplier>), typeof(Repository<Supplier>));

			services.AddScoped<IClientService, ClientService>();
			services.AddScoped(typeof(IRepository<Client>), typeof(Repository<Client>));

			services.AddScoped<IMenuNoteService, MenuNoteService>();
			services.AddScoped(typeof(IRepository<MenuNote>), typeof(Repository<MenuNote>));

			services.AddScoped<IBankService, BankService>();
			services.AddScoped(typeof(IRepository<Bank>), typeof(Repository<Bank>));

			services.AddScoped<ISellerTypeService, SellerTypeService>();
			services.AddScoped(typeof(IRepository<SellerType>), typeof(Repository<SellerType>));

			services.AddScoped<ISellerCommissionTypeService, SellerCommissionTypeService>();
			services.AddScoped(typeof(IRepository<SellerCommissionType>), typeof(Repository<SellerCommissionType>));

			services.AddScoped<ISellerCommissionMethodService, SellerCommissionMethodService>();
			services.AddScoped(typeof(IRepository<SellerCommissionMethod>), typeof(Repository<SellerCommissionMethod>));	
			
			services.AddScoped<ISellerCommissionService, SellerCommissionService>();
			services.AddScoped(typeof(IRepository<SellerCommission>), typeof(Repository<SellerCommission>));

			services.AddScoped<ISellerService, SellerService>();
			services.AddScoped(typeof(IRepository<Seller>), typeof(Repository<Seller>));

			services.AddScoped<ICurrencyService, CurrencyService>();
			services.AddScoped(typeof(IRepository<Currency>), typeof(Repository<Currency>));

			services.AddScoped<ICurrencyRateService, CurrencyRateService>();
			services.AddScoped(typeof(IRepository<CurrencyRate>), typeof(Repository<CurrencyRate>));

			services.AddScoped<IItemTypeService, ItemTypeService>();
			services.AddScoped(typeof(IRepository<ItemType>), typeof(Repository<ItemType>));

			services.AddScoped<IVendorService, VendorService>();
			services.AddScoped(typeof(IRepository<Vendor>), typeof(Repository<Vendor>));

			services.AddScoped<IItemPackageService, ItemPackageService>();
			services.AddScoped(typeof(IRepository<ItemPackage>), typeof(Repository<ItemPackage>));

			services.AddScoped<IItemCategoryService, ItemCategoryService>();
			services.AddScoped(typeof(IRepository<ItemCategory>), typeof(Repository<ItemCategory>));

			services.AddScoped<IItemSubCategoryService, ItemSubCategoryService>();
			services.AddScoped(typeof(IRepository<ItemSubCategory>), typeof(Repository<ItemSubCategory>));

			services.AddScoped<IItemSectionService, ItemSectionService>();
			services.AddScoped(typeof(IRepository<ItemSection>), typeof(Repository<ItemSection>));

			services.AddScoped<IItemSubSectionService, ItemSubSectionService>();
			services.AddScoped(typeof(IRepository<ItemSubSection>), typeof(Repository<ItemSubSection>));

			services.AddScoped<IMainItemService, MainItemService>();
			services.AddScoped(typeof(IRepository<MainItem>), typeof(Repository<MainItem>));

			services.AddScoped<IItemDivisionService, ItemDivisionService>();

			services.AddScoped<IItemAttributeTypeService, ItemAttributeTypeService>();
			services.AddScoped(typeof(IRepository<ItemAttributeType>), typeof(Repository<ItemAttributeType>));

			services.AddScoped<ITaxTypeService, TaxTypeService>();
			services.AddScoped(typeof(IRepository<TaxType>), typeof(Repository<TaxType>));

			services.AddScoped<ITaxService, TaxService>();
			services.AddScoped(typeof(IRepository<Tax>), typeof(Repository<Tax>));

			services.AddScoped<ITaxPercentService, TaxPercentService>();
			services.AddScoped(typeof(IRepository<TaxPercent>), typeof(Repository<TaxPercent>));

			services.AddScoped<IItemService, ItemService>();
			services.AddScoped(typeof(IRepository<Item>), typeof(Repository<Item>));

			services.AddScoped<IItemAttributeService, ItemAttributeService>();
			services.AddScoped(typeof(IRepository<ItemAttribute>), typeof(Repository<ItemAttribute>));

			services.AddScoped<IItemBarCodeService, ItemBarCodeService>();
			services.AddScoped(typeof(IRepository<ItemBarCode>), typeof(Repository<ItemBarCode>));

			services.AddScoped<IItemBarCodeDetailService, ItemBarCodeDetailService>();
			services.AddScoped(typeof(IRepository<ItemBarCodeDetail>), typeof(Repository<ItemBarCodeDetail>));

			services.AddScoped<IItemCostCalculationTypeService, ItemCostCalculationTypeService>();
			services.AddScoped(typeof(IRepository<ItemCostCalculationType>), typeof(Repository<ItemCostCalculationType>));

			services.AddScoped<IItemTaxService, ItemTaxService>();
			services.AddScoped(typeof(IRepository<ItemTax>), typeof(Repository<ItemTax>));



			services.AddScoped<ITransactionTypeService, TransactionTypeService>();
			services.AddScoped(typeof(IRepository<TransactionType>), typeof(Repository<TransactionType>));

			services.AddScoped<IPaymentTypeService, PaymentTypeService>();
			services.AddScoped(typeof(IRepository<PaymentType>), typeof(Repository<PaymentType>));

			services.AddScoped<IPaymentMethodService, PaymentMethodService>();
			services.AddScoped(typeof(IRepository<PaymentMethod>), typeof(Repository<PaymentMethod>));

			services.AddScoped<IItemPackingService, ItemPackingService>();
			services.AddScoped(typeof(IRepository<ItemPacking>), typeof(Repository<ItemPacking>));

			//Account
			services.AddScoped<ICostCenterService, CostCenterService>();
			services.AddScoped(typeof(IRepository<CostCenter>), typeof(Repository<CostCenter>));

			services.AddScoped<ICostCenterOpenBalanceService, CostCenterOpenBalanceService>();
			services.AddScoped(typeof(IRepository<CostCenterOpenBalance>), typeof(Repository<CostCenterOpenBalance>));

			services.AddScoped<IAccountCategoryService, AccountCategoryService>();
			services.AddScoped(typeof(IRepository<AccountCategory>), typeof(Repository<AccountCategory>));

			services.AddScoped<IAccountLedgerService, AccountLedgerService>();
			services.AddScoped(typeof(IRepository<AccountLedger>), typeof(Repository<AccountLedger>));

			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped(typeof(IRepository<Account>), typeof(Repository<Account>));

			services.AddScoped<IAccountStoreService, AccountStoreService>();
			services.AddScoped(typeof(IRepository<AccountStore>), typeof(Repository<AccountStore>));

			services.AddScoped<IAccountStoreTypeService, AccountStoreTypeService>();
			services.AddScoped(typeof(IRepository<AccountStoreType>), typeof(Repository<AccountStoreType>));

			services.AddScoped<IAccountEntityService, AccountEntityService>();

			services.AddScoped<IItemNoteValidationService, ItemNoteValidationService>();

			services.AddScoped<IClientBalanceService, ClientBalanceService>();

			services.AddScoped<IAccountTypeService, AccountTypeService>();
			services.AddScoped(typeof(IRepository<AccountType>), typeof(Repository<AccountType>));

			//Fixed Assets

			services.AddScoped<IFixedAssetService, FixedAssetService>();
			services.AddScoped(typeof(IRepository<FixedAsset>), typeof(Repository<FixedAsset>));

			//Journal

			services.AddScoped<IJournalTypeService, JournalTypeService>();
			services.AddScoped(typeof(IRepository<JournalType>), typeof(Repository<JournalType>));

			services.AddScoped<IEntityTypeService, EntityTypeService>();
			services.AddScoped(typeof(IRepository<EntityType>), typeof(Repository<EntityType>));

			services.AddScoped<IJournalHeaderService, JournalHeaderService>();
			services.AddScoped(typeof(IRepository<JournalHeader>), typeof(Repository<JournalHeader>));

			services.AddScoped<IJournalDetailService, JournalDetailService>();
			services.AddScoped(typeof(IRepository<JournalDetail>), typeof(Repository<JournalDetail>));

			services.AddScoped<ICostCenterJournalDetailService, CostCenterJournalDetailService>();
			services.AddScoped(typeof(IRepository<CostCenterJournalDetail>), typeof(Repository<CostCenterJournalDetail>));

			services.AddScoped<IJournalService, JournalService>();
			services.AddScoped<IAccountTaxService, AccountTaxService>();


			//Inventory

			services.AddScoped<IItemCurrentBalanceService, ItemCurrentBalanceService>();
			services.AddScoped(typeof(IRepository<ItemCurrentBalance>), typeof(Repository<ItemCurrentBalance>));

			services.AddScoped<IItemCostService, ItemCostService>();
			services.AddScoped(typeof(IRepository<ItemCost>), typeof(Repository<ItemCost>));

			services.AddScoped<IItemDisassembleHeaderService, ItemDisassembleHeaderService>();
			services.AddScoped(typeof(IRepository<ItemDisassembleHeader>), typeof(Repository<ItemDisassembleHeader>));

			services.AddScoped<IItemDisassembleDetailService, ItemDisassembleDetailService>();
			services.AddScoped(typeof(IRepository<ItemDisassembleDetail>), typeof(Repository<ItemDisassembleDetail>));

			services.AddScoped<IItemDisassembleSerialService, ItemDisassembleSerialService>();
			services.AddScoped(typeof(IRepository<ItemDisassembleSerial>), typeof(Repository<ItemDisassembleSerial>));

			services.AddScoped<IItemDisassembleService, ItemDisassembleService>();
			services.AddScoped(typeof(IRepository<ItemDisassemble>), typeof(Repository<ItemDisassemble>));

			services.AddScoped<IItemDisassembleHandlerService, ItemDisassembleHandlerService>();


			// Zero Stock Validation Service
			services.AddScoped<IZeroStockValidationService, ZeroStockValidationService>();

			services.AddScoped<IItemDisassembleLogicService, ItemDisassembleLogicService>();


			//Archive
			services.AddScoped<IBlobImageService, BlobImageService>();


			return services;
        }
    }
}
