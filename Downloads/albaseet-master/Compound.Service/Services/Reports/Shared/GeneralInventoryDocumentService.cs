using Compound.CoreOne.Contracts.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Shared;
using Inventory.CoreOne.Contracts;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;

namespace Compound.Service.Services.Reports.Shared
{
	public class GeneralInventoryDocumentService: IGeneralInventoryDocumentService
	{
		private readonly IInventoryInHeaderService _inventoryInHeaderService;
		private readonly IInventoryInDetailService _inventoryInDetailService;
		private readonly IInventoryOutHeaderService _inventoryOutHeaderService;
		private readonly IInventoryOutDetailService _inventoryOutDetailService;
		private readonly IInternalTransferHeaderService _internalTransferHeaderService;
		private readonly IInternalTransferDetailService _internalTransferDetailService;
		private readonly IInternalTransferReceiveHeaderService _internalTransferReceiveHeaderService;
		private readonly IInternalTransferReceiveDetailService _internalTransferReceiveDetailService;
		private readonly IStockTakingCarryOverHeaderService _stockTakingCarryOverHeaderService;
		private readonly IStockTakingCarryOverEffectDetailService _stockTakingCarryOverEffectDetailService;
		private readonly IStockTakingCarryOverDetailService _stockTakingCarryOverDetailService;
		private readonly IStockInHeaderService _stockInHeaderService;
		private readonly IStockInDetailService _stockInDetailService;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IInvoiceStockInService _invoiceStockInService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IInvoiceStockInReturnService _invoiceStockInReturnService;
		private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
		private readonly IStockInReturnDetailService _stockInReturnDetailService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutDetailService _stockOutDetailService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IInvoiceStockOutService _invoiceStockOutService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IInvoiceStockOutReturnService _invoiceStockOutReturnService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly IStockOutReturnDetailService _stockOutReturnDetailService;
		private readonly IItemDisassembleService _itemDisassembleService;
		private readonly IItemDisassembleHeaderService _itemDisassembleHeaderService;
		private readonly IStoreService _storeService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IItemService _itemService;
		private readonly IMenuService _menuService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;

		public GeneralInventoryDocumentService(IInventoryInHeaderService inventoryInHeaderService, IInventoryInDetailService inventoryInDetailService, IInventoryOutHeaderService inventoryOutHeaderService, IInventoryOutDetailService inventoryOutDetailService, IInternalTransferHeaderService internalTransferHeaderService, IInternalTransferDetailService internalTransferDetailService, IInternalTransferReceiveHeaderService internalTransferReceiveHeaderService, IInternalTransferReceiveDetailService internalTransferReceiveDetailService, IStockTakingCarryOverHeaderService stockTakingCarryOverHeaderService, IStockTakingCarryOverEffectDetailService stockTakingCarryOverEffectDetailService, IStockTakingCarryOverDetailService stockTakingCarryOverDetailService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IInvoiceStockInService invoiceStockInService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IInvoiceStockInReturnService invoiceStockInReturnService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStockOutHeaderService stockOutHeaderService, IStockOutDetailService stockOutDetailService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IInvoiceStockOutService invoiceStockOutService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IInvoiceStockOutReturnService invoiceStockOutReturnService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IItemDisassembleService itemDisassembleService, IItemDisassembleHeaderService itemDisassembleHeaderService, IStoreService storeService, IItemPackingService itemPackingService, IItemService itemService, IMenuService menuService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService)
		{
			_inventoryInHeaderService = inventoryInHeaderService;
			_inventoryInDetailService = inventoryInDetailService;
			_inventoryOutHeaderService = inventoryOutHeaderService;
			_inventoryOutDetailService = inventoryOutDetailService;
			_internalTransferHeaderService = internalTransferHeaderService;
			_internalTransferDetailService = internalTransferDetailService;
			_internalTransferReceiveHeaderService = internalTransferReceiveHeaderService;
			_internalTransferReceiveDetailService = internalTransferReceiveDetailService;
			_stockTakingCarryOverHeaderService = stockTakingCarryOverHeaderService;
			_stockTakingCarryOverEffectDetailService = stockTakingCarryOverEffectDetailService;
			_stockTakingCarryOverDetailService = stockTakingCarryOverDetailService;
			_stockInHeaderService = stockInHeaderService;
			_stockInDetailService = stockInDetailService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_invoiceStockInService = invoiceStockInService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_invoiceStockInReturnService = invoiceStockInReturnService;
			_stockInReturnHeaderService = stockInReturnHeaderService;
			_stockInReturnDetailService = stockInReturnDetailService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutDetailService = stockOutDetailService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_invoiceStockOutService = invoiceStockOutService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_invoiceStockOutReturnService = invoiceStockOutReturnService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_stockOutReturnDetailService = stockOutReturnDetailService;
			_itemDisassembleService = itemDisassembleService;
			_itemDisassembleHeaderService = itemDisassembleHeaderService;
			_storeService = storeService;
			_itemPackingService = itemPackingService;
			_itemService = itemService;
			_menuService = menuService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
		}

		//This function is used when we only need the summation balances of the items but don't care 
		//about individual movements
		public IQueryable<GeneralInventoryDocumentDto> GetGeneralInventoryDocuments()
		{
			var inventoryIns = from inventoryInHeader in _inventoryInHeaderService.GetAll()
							   from inventoryInDetail in _inventoryInDetailService.GetAll().Where(x => x.InventoryInHeaderId == inventoryInHeader.InventoryInHeaderId)
							   select new GeneralInventoryDocumentDto
							   {
								   StoreId = inventoryInHeader.StoreId,
								   ItemId = inventoryInDetail.ItemId,
								   ItemPackageId = inventoryInDetail.ItemPackageId,
								   ExpireDate = inventoryInDetail.ExpireDate,
								   BatchNumber = inventoryInDetail.BatchNumber,
								   DocumentDate = inventoryInHeader.DocumentDate,
								   BarCode = inventoryInDetail.BarCode,
								   ItemNote = null,
								   OpenQuantity = 0,
								   InQuantity = inventoryInDetail.Quantity,
								   OutQuantity = 0,
								   PendingInQuantity = 0,
								   PendingOutQuantity = 0,
								   ReservedQuantity = 0,
							   };

			var inventoryOuts = from inventoryOutHeader in _inventoryOutHeaderService.GetAll()
								from inventoryOutDetail in _inventoryOutDetailService.GetAll().Where(x => x.InventoryOutHeaderId == inventoryOutHeader.InventoryOutHeaderId)
								select new GeneralInventoryDocumentDto
								{
									StoreId = inventoryOutHeader.StoreId,
									ItemId = inventoryOutDetail.ItemId,
									ItemPackageId = inventoryOutDetail.ItemPackageId,
									ExpireDate = inventoryOutDetail.ExpireDate,
									BatchNumber = inventoryOutDetail.BatchNumber,
									DocumentDate = inventoryOutHeader.DocumentDate,
									BarCode = inventoryOutDetail.BarCode,
								    ItemNote = null,
									OpenQuantity = 0,
									InQuantity = 0,
									OutQuantity = inventoryOutDetail.Quantity,
									PendingInQuantity = 0,
									PendingOutQuantity = 0,
									ReservedQuantity = 0,
								};

			//Only Internal Transfers and internal transfer receives that are manually created are gotten here, reservation documents
			//are handled directly
			var internalTransfers = from internalTransferHeader in _internalTransferHeaderService.GetAll().Where(x => x.MenuCode == null)
									from internalTransferDetail in _internalTransferDetailService.GetAll().Where(x => x.InternalTransferHeaderId == internalTransferHeader.InternalTransferId)
									select new GeneralInventoryDocumentDto
									{
										StoreId = internalTransferHeader.FromStoreId,
										ItemId = internalTransferDetail.ItemId,
										ItemPackageId = internalTransferDetail.ItemPackageId,
										ExpireDate = internalTransferDetail.ExpireDate,
										BatchNumber = internalTransferDetail.BatchNumber,
										DocumentDate = internalTransferHeader.DocumentDate,
										BarCode = internalTransferDetail.BarCode,
								        ItemNote = null,
										OpenQuantity = 0,
										InQuantity = 0,
										OutQuantity = 0,
										PendingInQuantity = 0,
										PendingOutQuantity = internalTransferDetail.Quantity,
										ReservedQuantity = 0,
									};


			var internalTransferReceivesFromStore = from internalTransferReceiveHeader in _internalTransferReceiveHeaderService.GetAll().Where(x => x.MenuCode == null)
													from internalTransferReceiveDetail in _internalTransferReceiveDetailService.GetAll().Where(x => x.InternalTransferReceiveHeaderId == internalTransferReceiveHeader.InternalTransferReceiveHeaderId)
													select new GeneralInventoryDocumentDto
													{
														StoreId = internalTransferReceiveHeader.FromStoreId,
														ItemId = internalTransferReceiveDetail.ItemId,
														ItemPackageId = internalTransferReceiveDetail.ItemPackageId,
														ExpireDate = internalTransferReceiveDetail.ExpireDate,
														BatchNumber = internalTransferReceiveDetail.BatchNumber,
														DocumentDate = internalTransferReceiveHeader.DocumentDate,
														BarCode = internalTransferReceiveDetail.BarCode,
								                        ItemNote = null,
														OpenQuantity = 0,
														InQuantity = 0,
														OutQuantity = internalTransferReceiveDetail.Quantity,
														PendingInQuantity = 0,
														PendingOutQuantity = -internalTransferReceiveDetail.Quantity,
														ReservedQuantity = 0,
													};

			var internalTransferReceivesToStore = from internalTransferReceiveHeader in _internalTransferReceiveHeaderService.GetAll().Where(x => x.MenuCode == null)
												  from internalTransferReceiveDetail in _internalTransferReceiveDetailService.GetAll().Where(x => x.InternalTransferReceiveHeaderId == internalTransferReceiveHeader.InternalTransferReceiveHeaderId)
												  select new GeneralInventoryDocumentDto
												  {
													  StoreId = internalTransferReceiveHeader.ToStoreId,
													  ItemId = internalTransferReceiveDetail.ItemId,
													  ItemPackageId = internalTransferReceiveDetail.ItemPackageId,
													  ExpireDate = internalTransferReceiveDetail.ExpireDate,
													  BatchNumber = internalTransferReceiveDetail.BatchNumber,
													  DocumentDate = internalTransferReceiveHeader.DocumentDate,
													  BarCode = internalTransferReceiveDetail.BarCode,
								                      ItemNote = null,
													  OpenQuantity = 0,
													  InQuantity = internalTransferReceiveDetail.Quantity,
													  OutQuantity = 0,
													  PendingInQuantity = 0,
													  PendingOutQuantity = 0,
													  ReservedQuantity = 0,
												  };

			var carryOverEffects = from stockTakingCarryOverHeader in _stockTakingCarryOverHeaderService.GetAll()
								   from stockTakingCarryOverEffectsDetail in _stockTakingCarryOverEffectDetailService.GetAll().Where(x => x.StockTakingCarryOverHeaderId == stockTakingCarryOverHeader.StockTakingCarryOverHeaderId)
								   from stockTakingCarryOverDetail in _stockTakingCarryOverDetailService.GetAll().Where(x =>
									   x.StockTakingCarryOverHeaderId == stockTakingCarryOverHeader.StockTakingCarryOverHeaderId &&
									   x.ItemId == stockTakingCarryOverEffectsDetail.ItemId &&
									   x.ItemPackageId == stockTakingCarryOverEffectsDetail.ItemPackageId &&
									   x.ExpireDate == stockTakingCarryOverEffectsDetail.ExpireDate &&
									   x.BatchNumber == stockTakingCarryOverEffectsDetail.BatchNumber
								   )
								   select new GeneralInventoryDocumentDto
								   {
									   StoreId = stockTakingCarryOverHeader.StoreId,
									   ItemId = stockTakingCarryOverEffectsDetail.ItemId,
									   ItemPackageId = stockTakingCarryOverEffectsDetail.ItemPackageId,
									   ExpireDate = stockTakingCarryOverEffectsDetail.ExpireDate,
									   BatchNumber = stockTakingCarryOverEffectsDetail.BatchNumber,
									   DocumentDate = stockTakingCarryOverHeader.DocumentDate,
									   BarCode = stockTakingCarryOverDetail.BarCode,
								       ItemNote = null,
									   OpenQuantity = stockTakingCarryOverEffectsDetail.OpenQuantity,
									   InQuantity = stockTakingCarryOverEffectsDetail.InQuantity,
									   OutQuantity = stockTakingCarryOverEffectsDetail.OutQuantity,
									   PendingInQuantity = 0,
									   PendingOutQuantity = 0,
									   ReservedQuantity = 0,
								   };

			var stockIns = from stockInHeader in _stockInHeaderService.GetAll()
						   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)
						   select new GeneralInventoryDocumentDto
						   {
							   StoreId = stockInHeader.StoreId,
							   ItemId = stockInDetail.ItemId,
							   ItemPackageId = stockInDetail.ItemPackageId,
							   ExpireDate = stockInDetail.ExpireDate,
							   BatchNumber = stockInDetail.BatchNumber,
							   DocumentDate = stockInHeader.DocumentDate,
							   BarCode = stockInDetail.BarCode,
							   ItemNote = stockInDetail.ItemNote,
							   OpenQuantity = 0,
							   InQuantity = stockInDetail.Quantity + stockInDetail.BonusQuantity,
							   OutQuantity = 0,
							   PendingInQuantity = 0,
							   PendingOutQuantity = 0,
							   ReservedQuantity = 0,
						   };

			var stockInReturns = from stockInReturnHeader in _stockInReturnHeaderService.GetAll()
								 from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
								 select new GeneralInventoryDocumentDto
								 {
									 StoreId = stockInReturnHeader.StoreId,
									 ItemId = stockInReturnDetail.ItemId,
									 ItemPackageId = stockInReturnDetail.ItemPackageId,
									 ExpireDate = stockInReturnDetail.ExpireDate,
									 BatchNumber = stockInReturnDetail.BatchNumber,
									 DocumentDate = stockInReturnHeader.DocumentDate,
									 BarCode = stockInReturnDetail.BarCode,
								     ItemNote = stockInReturnDetail.ItemNote,
									 OpenQuantity = 0,
									 InQuantity = 0,
									 OutQuantity = stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity,
									 PendingInQuantity = 0,
									 PendingOutQuantity = 0,
									 ReservedQuantity = 0,
								 };

			var stockOuts = from stockOutHeader in _stockOutHeaderService.GetAll()
							from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
							select new GeneralInventoryDocumentDto
							{
								StoreId = stockOutHeader.StoreId,
								ItemId = stockOutDetail.ItemId,
								ItemPackageId = stockOutDetail.ItemPackageId,
								ExpireDate = stockOutDetail.ExpireDate,
								BatchNumber = stockOutDetail.BatchNumber,
								DocumentDate = stockOutHeader.DocumentDate,
								BarCode = stockOutDetail.BarCode,
								ItemNote = stockOutDetail.ItemNote,
								OpenQuantity = 0,
								InQuantity = stockOutHeader.StockTypeId == StockTypeData.StockOutFromSalesInvoice ? (stockOutDetail.Quantity + stockOutDetail.BonusQuantity) : 0,
								OutQuantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity,
								PendingInQuantity = 0,
								PendingOutQuantity = 0,
								ReservedQuantity = stockOutHeader.StockTypeId == StockTypeData.StockOutFromSalesInvoice ? -(stockOutDetail.Quantity + stockOutDetail.BonusQuantity) : 0,
							};

			var stockOutReturns = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll()
								  from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								  select new GeneralInventoryDocumentDto
								  {
									  StoreId = stockOutReturnHeader.StoreId,
									  ItemId = stockOutReturnDetail.ItemId,
									  ItemPackageId = stockOutReturnDetail.ItemPackageId,
									  ExpireDate = stockOutReturnDetail.ExpireDate,
									  BatchNumber = stockOutReturnDetail.BatchNumber,
									  DocumentDate = stockOutReturnHeader.DocumentDate,
									  BarCode = stockOutReturnDetail.BarCode,
								      ItemNote = stockOutReturnDetail.ItemNote,
									  OpenQuantity = 0,
									  InQuantity = stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity,
									  OutQuantity = stockOutReturnHeader.StockTypeId == StockTypeData.StockOutReturnFromInvoicedStockOut ? (stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) : 0,
									  PendingInQuantity = 0,
									  PendingOutQuantity = 0,
									  ReservedQuantity = stockOutReturnHeader.StockTypeId == StockTypeData.StockOutReturnFromInvoicedStockOut ? (stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) : 0,
								  };

			var reservationInvoices = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.IsOnTheWay)
									  from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
									  select new GeneralInventoryDocumentDto
									  {
										  StoreId = salesInvoiceHeader.StoreId,
										  ItemId = salesInvoiceDetail.ItemId,
										  ItemPackageId = salesInvoiceDetail.ItemPackageId,
										  ExpireDate = salesInvoiceDetail.ExpireDate,
										  BatchNumber = salesInvoiceDetail.BatchNumber,
										  DocumentDate = salesInvoiceHeader.DocumentDate,
										  BarCode = salesInvoiceDetail.BarCode,
										  ItemNote = salesInvoiceDetail.ItemNote,
										  OpenQuantity = 0,
										  InQuantity = 0,
										  OutQuantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity,
										  PendingInQuantity = 0,
										  PendingOutQuantity = 0,
										  ReservedQuantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity,
									  };

			var reservationInvoiceCloseOuts = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.IsOnTheWay)
											  from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
											  select new GeneralInventoryDocumentDto
											  {
												  StoreId = salesInvoiceReturnHeader.StoreId,
												  ItemId = salesInvoiceReturnDetail.ItemId,
												  ItemPackageId = salesInvoiceReturnDetail.ItemPackageId,
												  ExpireDate = salesInvoiceReturnDetail.ExpireDate,
												  BatchNumber = salesInvoiceReturnDetail.BatchNumber,
												  DocumentDate = salesInvoiceReturnHeader.DocumentDate,
												  BarCode = salesInvoiceReturnDetail.BarCode,
												  ItemNote = salesInvoiceReturnDetail.ItemNote,
												  OpenQuantity = 0,
												  InQuantity = salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity,
												  OutQuantity = 0,
												  PendingInQuantity = 0,
												  PendingOutQuantity = 0,
												  ReservedQuantity = -(salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity),
											  };

			var itemDisassembles = from itemDisassembleHeader in _itemDisassembleHeaderService.GetAll()
								   from itemDisassemble in _itemDisassembleService.GetAll().Where(x => x.ItemDisassembleHeaderId == itemDisassembleHeader.ItemDisassembleHeaderId)
								   select new GeneralInventoryDocumentDto
								   {
									   StoreId = itemDisassembleHeader.StoreId,
									   ItemId = itemDisassemble.ItemId,
									   ItemPackageId = itemDisassemble.ItemPackageId,
									   ExpireDate = itemDisassemble.ExpireDate,
									   BatchNumber = itemDisassemble.BatchNumber,
									   DocumentDate = itemDisassembleHeader.DocumentDate,
									   BarCode = null, //todo: get barcode for item disassembles
									   ItemNote = null,
									   OpenQuantity = 0,
									   InQuantity = itemDisassemble.InQuantity,
									   OutQuantity = itemDisassemble.OutQuantity,
									   PendingInQuantity = 0,
									   PendingOutQuantity = 0,
									   ReservedQuantity = 0,
								   };

			return inventoryIns.Concat(inventoryOuts).Concat(internalTransfers).Concat(internalTransferReceivesFromStore).Concat(internalTransferReceivesToStore).Concat(carryOverEffects).Concat(stockIns).Concat(stockInReturns).Concat(stockOuts).Concat(stockOutReturns).Concat(reservationInvoices).Concat(reservationInvoiceCloseOuts).Concat(itemDisassembles);
		}

		//This function is used when we want data about each individual item movement
		public IQueryable<GeneralInventoryDocumentWithMenuCodeDto> GetGeneralInventoryDocumentsWithMenuData()
		{
			var inventoryIns = from inventoryInHeader in _inventoryInHeaderService.GetAll()
							   from inventoryInDetail in _inventoryInDetailService.GetAll().Where(x => x.InventoryInHeaderId == inventoryInHeader.InventoryInHeaderId)
							   select new GeneralInventoryDocumentWithMenuCodeDto
							   {
								   HeaderId = inventoryInHeader.InventoryInHeaderId,
								   MenuCode = MenuCodeData.InventoryIn,
								   DocumentFullCode = inventoryInHeader.Prefix + inventoryInHeader.InventoryInCode + inventoryInHeader.Suffix,
								   StoreId = inventoryInHeader.StoreId,
								   ClientId = null,
								   SupplierId = null,
								   SellerId = null,
								   ItemId = inventoryInDetail.ItemId,
								   ItemPackageId = inventoryInDetail.ItemPackageId,
								   ExpireDate = inventoryInDetail.ExpireDate,
								   BatchNumber = inventoryInDetail.BatchNumber,
								   DocumentDate = inventoryInHeader.DocumentDate,
								   EntryDate = inventoryInHeader.EntryDate,
								   BarCode = inventoryInDetail.BarCode,
								   CostCenterId = null,
								   ItemNote = null,
								   StoredQuantity = inventoryInDetail.Quantity,
								   OpenQuantity = 0,
								   InQuantity = inventoryInDetail.Quantity,
								   OutQuantity = 0,
								   PendingInQuantity = 0,
								   PendingOutQuantity = 0,
								   ReservedQuantity = 0,
								   CostPrice = inventoryInDetail.CostPrice,
								   CostPackage = inventoryInDetail.CostPackage,
								   Price = inventoryInDetail.ConsumerPrice,
								   Reference = inventoryInHeader.Reference,
								   RemarksAr = inventoryInHeader.RemarksAr,
								   RemarksEn = inventoryInHeader.RemarksEn,
								   Notes = null,
								   CreatedAt = inventoryInHeader.CreatedAt,
								   UserNameCreated = inventoryInHeader.UserNameCreated,
								   ModifiedAt = inventoryInHeader.ModifiedAt,
								   UserNameModified = inventoryInHeader.UserNameModified,

								   PurchaseInvoicesQuantity = 0,
								   SalesInvoiceReturnQuantity = 0,
								   StockInQuantity = 0,
								   StockOutReturnQuantity = 0,
								   InternalTransferInQuantity = 0,
								   InventoryInQuantity = inventoryInDetail.Quantity,
								   CarryOverInQuantity = 0,
							       DisassembleInQuantity = 0,

								   SalesInvoicesQuantity = 0,
								   PurchaseInvoiceReturnQuantity = 0,
								   StockOutQuantity = 0,
								   StockInReturnQuantity = 0,
								   InternalTransferOutQuantity = 0,
								   InventoryOutQuantity = 0,
								   CarryOverOutQuantity = 0,
							       DisassembleOutQuantity = 0,
							   };

			var inventoryOuts = from inventoryOutHeader in _inventoryOutHeaderService.GetAll()
								from inventoryOutDetail in _inventoryOutDetailService.GetAll().Where(x => x.InventoryOutHeaderId == inventoryOutHeader.InventoryOutHeaderId)
								select new GeneralInventoryDocumentWithMenuCodeDto
								{
									HeaderId = inventoryOutHeader.InventoryOutHeaderId,
									MenuCode = MenuCodeData.InventoryOut,
									DocumentFullCode = inventoryOutHeader.Prefix + inventoryOutHeader.InventoryOutCode + inventoryOutHeader.Suffix,
									StoreId = inventoryOutHeader.StoreId,
									ClientId = null,
									SupplierId = null,
									SellerId = null,
									ItemId = inventoryOutDetail.ItemId,
									ItemPackageId = inventoryOutDetail.ItemPackageId,
									ExpireDate = inventoryOutDetail.ExpireDate,
									BatchNumber = inventoryOutDetail.BatchNumber,
									DocumentDate = inventoryOutHeader.DocumentDate,
									EntryDate = inventoryOutHeader.EntryDate,
									BarCode = inventoryOutDetail.BarCode,
								    CostCenterId = null,
									ItemNote = null,
									StoredQuantity = inventoryOutDetail.Quantity,
									OpenQuantity = 0,
									InQuantity = 0,
									OutQuantity = inventoryOutDetail.Quantity,
									PendingInQuantity = 0,
									PendingOutQuantity = 0,
									ReservedQuantity = 0,
									CostPrice = inventoryOutDetail.CostPrice,
									CostPackage = inventoryOutDetail.CostPackage,
									Price = inventoryOutDetail.ConsumerPrice,
									Reference = inventoryOutHeader.Reference,
									RemarksAr = inventoryOutHeader.RemarksAr,
									RemarksEn = inventoryOutHeader.RemarksEn,
									Notes = null,
									CreatedAt = inventoryOutHeader.CreatedAt,
									UserNameCreated = inventoryOutHeader.UserNameCreated,
									ModifiedAt = inventoryOutHeader.ModifiedAt,
									UserNameModified = inventoryOutHeader.UserNameModified,

								    PurchaseInvoicesQuantity = 0,
								    SalesInvoiceReturnQuantity = 0,
								    StockInQuantity = 0,
									StockOutReturnQuantity = 0,
								    InternalTransferInQuantity = 0,
								    InventoryInQuantity = 0,
									CarryOverInQuantity = 0,
							        DisassembleInQuantity = 0,

								    SalesInvoicesQuantity = 0,
								    PurchaseInvoiceReturnQuantity = 0,
								    StockOutQuantity = 0,
								    StockInReturnQuantity = 0,
								    InternalTransferOutQuantity = 0,
								    InventoryOutQuantity = inventoryOutDetail.Quantity,
									CarryOverOutQuantity = 0,
							        DisassembleOutQuantity = 0,
								};

			/*
			 * Only Internal Transfers and internal transfer receives that are manually created are gotten here, reservation documents
			 * are handled directly
			 */
			var internalTransfers = from internalTransferHeader in _internalTransferHeaderService.GetAll().Where(x => x.MenuCode == null)
									from internalTransferDetail in _internalTransferDetailService.GetAll().Where(x => x.InternalTransferHeaderId == internalTransferHeader.InternalTransferId)
									select new GeneralInventoryDocumentWithMenuCodeDto
									{
										HeaderId = internalTransferHeader.InternalTransferId,
										MenuCode = MenuCodeData.InternalTransferItems,
										DocumentFullCode = (internalTransferHeader.Prefix + internalTransferHeader.InternalTransferCode + internalTransferHeader.Suffix),
										StoreId = internalTransferHeader.FromStoreId,
										ClientId = null,
										SupplierId = null,
									    SellerId = null,
										ItemId = internalTransferDetail.ItemId,
										ItemPackageId = internalTransferDetail.ItemPackageId,
										ExpireDate = internalTransferDetail.ExpireDate,
										BatchNumber = internalTransferDetail.BatchNumber,
										DocumentDate = internalTransferHeader.DocumentDate,
										EntryDate = internalTransferHeader.EntryDate,
										BarCode = internalTransferDetail.BarCode,
								        CostCenterId = null,
										ItemNote = null,
										StoredQuantity = internalTransferDetail.Quantity,
										OpenQuantity = 0,
										InQuantity = 0,
										OutQuantity = 0,
										PendingInQuantity = 0,
										PendingOutQuantity = internalTransferDetail.Quantity,
										ReservedQuantity = 0,
										CostPrice = internalTransferDetail.CostPrice,
										CostPackage = internalTransferDetail.CostPackage,
										Price = internalTransferDetail.ConsumerPrice,
										Reference = internalTransferHeader.Reference,
									    RemarksAr = internalTransferHeader.RemarksAr,
									    RemarksEn = internalTransferHeader.RemarksEn,
										Notes = null,
										CreatedAt = internalTransferHeader.CreatedAt,
										UserNameCreated = internalTransferHeader.UserNameCreated,
										ModifiedAt = internalTransferHeader.ModifiedAt,
										UserNameModified = internalTransferHeader.UserNameModified,

								        PurchaseInvoicesQuantity = 0,
								        SalesInvoiceReturnQuantity = 0,
								        StockInQuantity = 0,
									    StockOutReturnQuantity = 0,
								        InternalTransferInQuantity = 0,
								        InventoryInQuantity = 0,
									    CarryOverInQuantity = 0,
							            DisassembleInQuantity = 0,

								        SalesInvoicesQuantity = 0,
								        PurchaseInvoiceReturnQuantity = 0,
								        StockOutQuantity = 0,
								        StockInReturnQuantity = 0,
								        InternalTransferOutQuantity = 0,
								        InventoryOutQuantity = 0,
									    CarryOverOutQuantity = 0,
							            DisassembleOutQuantity = 0,
									};


			var internalTransferReceivesFromStore = from internalTransferReceiveHeader in _internalTransferReceiveHeaderService.GetAll().Where(x => x.MenuCode == null)
													from internalTransferReceiveDetail in _internalTransferReceiveDetailService.GetAll().Where(x => x.InternalTransferReceiveHeaderId == internalTransferReceiveHeader.InternalTransferReceiveHeaderId)
													select new GeneralInventoryDocumentWithMenuCodeDto
													{
														HeaderId = internalTransferReceiveHeader.ReferenceId ?? internalTransferReceiveHeader.InternalTransferReceiveHeaderId,
														MenuCode = internalTransferReceiveHeader.MenuCode ?? MenuCodeData.InternalReceiveItems,
														DocumentFullCode = (internalTransferReceiveHeader.Prefix + internalTransferReceiveHeader.InternalTransferReceiveCode + internalTransferReceiveHeader.Suffix),
														StoreId = internalTransferReceiveHeader.FromStoreId,
										                ClientId = null,
														SupplierId = null,
													    SellerId = null,
														ItemId = internalTransferReceiveDetail.ItemId,
														ItemPackageId = internalTransferReceiveDetail.ItemPackageId,
														ExpireDate = internalTransferReceiveDetail.ExpireDate,
														BatchNumber = internalTransferReceiveDetail.BatchNumber,
														DocumentDate = internalTransferReceiveHeader.DocumentDate,
														EntryDate = internalTransferReceiveHeader.EntryDate,
														BarCode = internalTransferReceiveDetail.BarCode,
								                        CostCenterId = null,
														ItemNote = null,
														StoredQuantity = internalTransferReceiveDetail.Quantity,
														OpenQuantity = 0,
														InQuantity = 0,
														OutQuantity = internalTransferReceiveDetail.Quantity,
														PendingInQuantity = 0,
														PendingOutQuantity = -internalTransferReceiveDetail.Quantity,
														ReservedQuantity = 0,
														CostPrice = internalTransferReceiveDetail.CostPrice,
														CostPackage = internalTransferReceiveDetail.CostPackage,
														Price = internalTransferReceiveDetail.ConsumerPrice,
														Reference = internalTransferReceiveHeader.Reference,
									                    RemarksAr = internalTransferReceiveHeader.RemarksAr,
									                    RemarksEn = internalTransferReceiveHeader.RemarksEn,
														Notes = null,
														CreatedAt = internalTransferReceiveHeader.CreatedAt,
														UserNameCreated = internalTransferReceiveHeader.UserNameCreated,
														ModifiedAt = internalTransferReceiveHeader.ModifiedAt,
														UserNameModified = internalTransferReceiveHeader.UserNameModified,

								                        PurchaseInvoicesQuantity = 0,
								                        SalesInvoiceReturnQuantity = 0,
								                        StockInQuantity = 0,
									                    StockOutReturnQuantity = 0,
								                        InternalTransferInQuantity = 0,
								                        InventoryInQuantity = 0,
									                    CarryOverInQuantity = 0,
							                            DisassembleInQuantity = 0,

								                        SalesInvoicesQuantity = 0,
								                        PurchaseInvoiceReturnQuantity = 0,
								                        StockOutQuantity = 0,
								                        StockInReturnQuantity = 0,
								                        InternalTransferOutQuantity = internalTransferReceiveDetail.Quantity,
								                        InventoryOutQuantity = 0,
									                    CarryOverOutQuantity = 0,
							                            DisassembleOutQuantity = 0,
													};

			var internalTransferReceivesToStore = from internalTransferReceiveHeader in _internalTransferReceiveHeaderService.GetAll().Where(x => x.MenuCode == null)
												  from internalTransferReceiveDetail in _internalTransferReceiveDetailService.GetAll().Where(x => x.InternalTransferReceiveHeaderId == internalTransferReceiveHeader.InternalTransferReceiveHeaderId)
												  select new GeneralInventoryDocumentWithMenuCodeDto
												  {
													  HeaderId = internalTransferReceiveHeader.ReferenceId ?? internalTransferReceiveHeader.InternalTransferReceiveHeaderId,
													  MenuCode = internalTransferReceiveHeader.MenuCode ?? MenuCodeData.InternalReceiveItems,
													  DocumentFullCode = (internalTransferReceiveHeader.Prefix + internalTransferReceiveHeader.InternalTransferReceiveCode + internalTransferReceiveHeader.Suffix),
													  StoreId = internalTransferReceiveHeader.ToStoreId,
										              ClientId = null,
													  SupplierId = null,
													  SellerId = null,
													  ItemId = internalTransferReceiveDetail.ItemId,
													  ItemPackageId = internalTransferReceiveDetail.ItemPackageId,
													  ExpireDate = internalTransferReceiveDetail.ExpireDate,
													  BatchNumber = internalTransferReceiveDetail.BatchNumber,
													  DocumentDate = internalTransferReceiveHeader.DocumentDate,
													  EntryDate = internalTransferReceiveHeader.EntryDate,
													  BarCode = internalTransferReceiveDetail.BarCode,
								                      CostCenterId = null,
													  ItemNote = null,
													  StoredQuantity = internalTransferReceiveDetail.Quantity,
													  OpenQuantity = 0,
													  InQuantity = internalTransferReceiveDetail.Quantity,
													  OutQuantity = 0,
													  PendingInQuantity = 0,
													  PendingOutQuantity = 0,
													  ReservedQuantity = 0,
													  CostPrice = internalTransferReceiveDetail.CostPrice,
													  CostPackage = internalTransferReceiveDetail.CostPackage,
													  Price = internalTransferReceiveDetail.ConsumerPrice,
													  Reference = internalTransferReceiveHeader.Reference,
									                  RemarksAr = internalTransferReceiveHeader.RemarksAr,
									                  RemarksEn = internalTransferReceiveHeader.RemarksEn,
													  Notes = null,
													  CreatedAt = internalTransferReceiveHeader.CreatedAt,
													  UserNameCreated = internalTransferReceiveHeader.UserNameCreated,
													  ModifiedAt = internalTransferReceiveHeader.ModifiedAt,
													  UserNameModified = internalTransferReceiveHeader.UserNameModified,

								                      PurchaseInvoicesQuantity = 0,
								                      SalesInvoiceReturnQuantity = 0,
								                      StockInQuantity = 0,
									                  StockOutReturnQuantity = 0,
								                      InternalTransferInQuantity = internalTransferReceiveDetail.Quantity,
								                      InventoryInQuantity = 0,
									                  CarryOverInQuantity = 0,
							                          DisassembleInQuantity = 0,

								                      SalesInvoicesQuantity = 0,
								                      PurchaseInvoiceReturnQuantity = 0,
								                      StockOutQuantity = 0,
								                      StockInReturnQuantity = 0,
								                      InternalTransferOutQuantity = 0,
								                      InventoryOutQuantity = 0,
									                  CarryOverOutQuantity = 0,
							                          DisassembleOutQuantity = 0,
												  };

			var carryOverEffects = from stockTakingCarryOverHeader in _stockTakingCarryOverHeaderService.GetAll()
								   from stockTakingCarryOverEffectDetail in _stockTakingCarryOverEffectDetailService.GetAll().Where(x => x.StockTakingCarryOverHeaderId == stockTakingCarryOverHeader.StockTakingCarryOverHeaderId)
								   from stockTakingCarryOverDetail in _stockTakingCarryOverDetailService.GetAll().Where(x =>
									   x.StockTakingCarryOverHeaderId == stockTakingCarryOverHeader.StockTakingCarryOverHeaderId &&
									   x.ItemId == stockTakingCarryOverEffectDetail.ItemId &&
									   x.ItemPackageId == stockTakingCarryOverEffectDetail.ItemPackageId &&
									   x.ExpireDate == stockTakingCarryOverEffectDetail.ExpireDate &&
									   x.BatchNumber == stockTakingCarryOverEffectDetail.BatchNumber
								   )
								   select new GeneralInventoryDocumentWithMenuCodeDto
								   {
									   HeaderId = stockTakingCarryOverHeader.StockTakingCarryOverHeaderId,
									   MenuCode = stockTakingCarryOverHeader.IsOpenBalance ? (short)MenuCodeData.ApprovalStockTakingAsOpenBalance : (short)MenuCodeData.ApprovalStockTakingAsCurrentBalance,
									   DocumentFullCode = stockTakingCarryOverHeader.Prefix + stockTakingCarryOverHeader.StockTakingCarryOverCode + stockTakingCarryOverHeader.Suffix,
									   StoreId = stockTakingCarryOverHeader.StoreId,
									   ClientId = null,
									   SupplierId = null,
									   SellerId = null,
									   ItemId = stockTakingCarryOverEffectDetail.ItemId,
									   ItemPackageId = stockTakingCarryOverEffectDetail.ItemPackageId,
									   ExpireDate = stockTakingCarryOverEffectDetail.ExpireDate,
									   BatchNumber = stockTakingCarryOverEffectDetail.BatchNumber,
									   DocumentDate = stockTakingCarryOverHeader.DocumentDate,
									   EntryDate = stockTakingCarryOverHeader.EntryDate,
									   BarCode = stockTakingCarryOverDetail.BarCode,
								       CostCenterId = null,
									   ItemNote = null,
									   StoredQuantity = 0,
									   OpenQuantity = stockTakingCarryOverEffectDetail.OpenQuantity,
									   InQuantity = stockTakingCarryOverEffectDetail.InQuantity,
									   OutQuantity = stockTakingCarryOverEffectDetail.OutQuantity,
									   PendingInQuantity = 0,
									   PendingOutQuantity = 0,
									   ReservedQuantity = 0,
									   CostPrice = stockTakingCarryOverDetail.StockTakingCostPrice,
									   CostPackage = stockTakingCarryOverDetail.StockTakingCostPackage,
									   Price = stockTakingCarryOverDetail.StockTakingConsumerPrice,
									   Reference = stockTakingCarryOverHeader.Reference,
									   RemarksAr = stockTakingCarryOverHeader.RemarksAr,
									   RemarksEn = stockTakingCarryOverHeader.RemarksEn,
									   Notes = null,
									   CreatedAt = stockTakingCarryOverHeader.CreatedAt,
									   UserNameCreated = stockTakingCarryOverHeader.UserNameCreated,
									   ModifiedAt = stockTakingCarryOverHeader.ModifiedAt,
									   UserNameModified = stockTakingCarryOverHeader.UserNameModified,

								       PurchaseInvoicesQuantity = 0,
								       SalesInvoiceReturnQuantity = 0,
								       StockInQuantity = 0,
									   StockOutReturnQuantity = 0,
								       InternalTransferInQuantity = 0,
								       InventoryInQuantity = 0,
									   CarryOverInQuantity = stockTakingCarryOverEffectDetail.InQuantity + stockTakingCarryOverEffectDetail.OpenQuantity,
							           DisassembleInQuantity = 0,

								       SalesInvoicesQuantity = 0,
								       PurchaseInvoiceReturnQuantity = 0,
								       StockOutQuantity = 0,
								       StockInReturnQuantity = 0,
								       InternalTransferOutQuantity = 0,
								       InventoryOutQuantity = 0,
									   CarryOverOutQuantity = stockTakingCarryOverEffectDetail.OutQuantity,
							           DisassembleOutQuantity = 0,
								   };

			/* All stock documents created from direct invoices should show the headerId, menuCode
			 * and documentCode of the direct invoice
			 */
			var stockIns = from stockInHeader in _stockInHeaderService.GetAll()
						   from invoiceStockIn in _invoiceStockInService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId).DefaultIfEmpty()
						   from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == invoiceStockIn.PurchaseInvoiceHeaderId).DefaultIfEmpty()
						   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)
						   select new GeneralInventoryDocumentWithMenuCodeDto
						   {
							   HeaderId = purchaseInvoiceHeader.IsDirectInvoice == true ? purchaseInvoiceHeader.PurchaseInvoiceHeaderId : stockInHeader.StockInHeaderId,
							   MenuCode = (purchaseInvoiceHeader.IsDirectInvoice == true ? purchaseInvoiceHeader.MenuCode : stockInHeader.MenuCode) ?? 0,
							   DocumentFullCode = (
								   purchaseInvoiceHeader.IsDirectInvoice == true ?
									   (purchaseInvoiceHeader.Prefix + purchaseInvoiceHeader.DocumentCode + purchaseInvoiceHeader.Suffix) :
									   (stockInHeader.Prefix + stockInHeader.DocumentCode + stockInHeader.Suffix)
								   ),
							   StoreId = stockInHeader.StoreId,
							   ClientId = null,
							   SupplierId = stockInHeader.SupplierId,
							   SellerId = null,
							   ItemId = stockInDetail.ItemId,
							   ItemPackageId = stockInDetail.ItemPackageId,
							   ExpireDate = stockInDetail.ExpireDate,
							   BatchNumber = stockInDetail.BatchNumber,
							   DocumentDate = stockInHeader.DocumentDate,
							   EntryDate = stockInHeader.EntryDate,
							   BarCode = stockInDetail.BarCode,
							   CostCenterId = stockInDetail.CostCenterId,
							   ItemNote = stockInDetail.ItemNote,
							   StoredQuantity = stockInDetail.Quantity + stockInDetail.BonusQuantity,
							   OpenQuantity = 0,
							   InQuantity = stockInDetail.Quantity + stockInDetail.BonusQuantity,
							   OutQuantity = 0,
							   PendingInQuantity = 0,
							   PendingOutQuantity = 0,
							   ReservedQuantity = 0,
							   CostPrice = stockInDetail.CostPrice,
							   CostPackage = stockInDetail.CostPackage,
							   Price = stockInDetail.PurchasePrice,
							   Reference = stockInHeader.Reference,
							   RemarksAr = stockInHeader.RemarksAr,
							   RemarksEn = stockInHeader.RemarksEn,
							   Notes = stockInDetail.Notes,
							   CreatedAt = stockInHeader.CreatedAt,
							   UserNameCreated = stockInHeader.UserNameCreated,
							   ModifiedAt = stockInHeader.ModifiedAt,
							   UserNameModified = stockInHeader.UserNameModified,

							   PurchaseInvoicesQuantity = purchaseInvoiceHeader.IsDirectInvoice == true ? stockInDetail.Quantity + stockInDetail.BonusQuantity : 0,
							   SalesInvoiceReturnQuantity = 0,
							   StockInQuantity = purchaseInvoiceHeader.IsDirectInvoice != true ? stockInDetail.Quantity + stockInDetail.BonusQuantity : 0,
							   StockOutReturnQuantity = 0,
							   InternalTransferInQuantity = 0,
							   InventoryInQuantity = 0,
							   CarryOverInQuantity = 0,
							   DisassembleInQuantity = 0,

							   SalesInvoicesQuantity = 0,
							   PurchaseInvoiceReturnQuantity = 0,
							   StockOutQuantity = 0,
							   StockInReturnQuantity = 0,
							   InternalTransferOutQuantity = 0,
							   InventoryOutQuantity = 0,
	   						   CarryOverOutQuantity = 0,
							   DisassembleOutQuantity = 0,
						   };

			var stockInReturns = from stockInReturnHeader in _stockInReturnHeaderService.GetAll()
								 from invoiceStockInReturn in _invoiceStockInReturnService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId).DefaultIfEmpty()
								 from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == invoiceStockInReturn.PurchaseInvoiceReturnHeaderId).DefaultIfEmpty()
								 from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
								 select new GeneralInventoryDocumentWithMenuCodeDto
								 {
									 HeaderId = purchaseInvoiceReturnHeader.IsDirectInvoice == true ? purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId : stockInReturnHeader.StockInReturnHeaderId,
									 MenuCode = (purchaseInvoiceReturnHeader.IsDirectInvoice == true ? purchaseInvoiceReturnHeader.MenuCode : stockInReturnHeader.MenuCode) ?? 0,
									 DocumentFullCode = (
										 purchaseInvoiceReturnHeader.IsDirectInvoice == true ?
											 (purchaseInvoiceReturnHeader.Prefix + purchaseInvoiceReturnHeader.DocumentCode + purchaseInvoiceReturnHeader.Suffix) :
											 (stockInReturnHeader.Prefix + stockInReturnHeader.DocumentCode + stockInReturnHeader.Suffix)
										 ),
									 StoreId = stockInReturnHeader.StoreId,
									 ClientId = null,
									 SupplierId = stockInReturnHeader.SupplierId,
									 SellerId = null,
									 ItemId = stockInReturnDetail.ItemId,
									 ItemPackageId = stockInReturnDetail.ItemPackageId,
									 ExpireDate = stockInReturnDetail.ExpireDate,
									 BatchNumber = stockInReturnDetail.BatchNumber,
									 DocumentDate = stockInReturnHeader.DocumentDate,
									 EntryDate = stockInReturnHeader.EntryDate,
									 BarCode = stockInReturnDetail.BarCode,
							         CostCenterId = stockInReturnDetail.CostCenterId,
									 ItemNote = stockInReturnDetail.ItemNote,
									 StoredQuantity = stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity,
									 OpenQuantity = 0,
									 InQuantity = 0,
									 OutQuantity = stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity,
									 PendingInQuantity = 0,
									 PendingOutQuantity = 0,
									 ReservedQuantity = 0,
									 CostPrice = stockInReturnDetail.CostPrice,
									 CostPackage = stockInReturnDetail.CostPackage,
									 Price = stockInReturnDetail.PurchasePrice,
									 Reference = stockInReturnHeader.Reference,
							         RemarksAr = stockInReturnHeader.RemarksAr,
							         RemarksEn = stockInReturnHeader.RemarksEn,
							         Notes = stockInReturnDetail.Notes,
									 CreatedAt = stockInReturnHeader.CreatedAt,
									 UserNameCreated = stockInReturnHeader.UserNameCreated,
									 ModifiedAt = stockInReturnHeader.ModifiedAt,
									 UserNameModified = stockInReturnHeader.UserNameModified,

	   						         PurchaseInvoicesQuantity = 0,
	   						         SalesInvoiceReturnQuantity = 0,
	   						         StockInQuantity = 0,
									 StockOutReturnQuantity = 0,
	   						         InternalTransferInQuantity = 0,
	   						         InventoryInQuantity = 0,
	   						         CarryOverInQuantity = 0,
							         DisassembleInQuantity = 0,

	   						         SalesInvoicesQuantity = 0,
	   						         PurchaseInvoiceReturnQuantity = purchaseInvoiceReturnHeader.IsDirectInvoice == true ? stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity : 0,
	   						         StockOutQuantity = 0,
									 StockInReturnQuantity = purchaseInvoiceReturnHeader.IsDirectInvoice != true ? stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity : 0,
									 InternalTransferOutQuantity = 0,
	   						         InventoryOutQuantity = 0,
	   						         CarryOverOutQuantity = 0,
								     DisassembleOutQuantity = 0,
								 };

			var stockOuts = from stockOutHeader in _stockOutHeaderService.GetAll()
							from invoiceStockOut in _invoiceStockOutService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId).DefaultIfEmpty()
							from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == invoiceStockOut.SalesInvoiceHeaderId).DefaultIfEmpty()
							from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
							select new GeneralInventoryDocumentWithMenuCodeDto
							{
								HeaderId = salesInvoiceHeader.IsDirectInvoice == true ? salesInvoiceHeader.SalesInvoiceHeaderId : stockOutHeader.StockOutHeaderId,
								MenuCode = (salesInvoiceHeader.IsDirectInvoice == true ? salesInvoiceHeader.MenuCode : stockOutHeader.MenuCode) ?? 0,
								DocumentFullCode = (
									salesInvoiceHeader.IsDirectInvoice == true ?
										(salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix) :
										(stockOutHeader.Prefix + stockOutHeader.DocumentCode + stockOutHeader.Suffix)
									),
								StoreId = stockOutHeader.StoreId,
								ClientId = stockOutHeader.ClientId,
								SupplierId = null,
								SellerId = stockOutHeader.SellerId,
								ItemId = stockOutDetail.ItemId,
								ItemPackageId = stockOutDetail.ItemPackageId,
								ExpireDate = stockOutDetail.ExpireDate,
								BatchNumber = stockOutDetail.BatchNumber,
								DocumentDate = stockOutHeader.DocumentDate,
								EntryDate = stockOutHeader.EntryDate,
								BarCode = stockOutDetail.BarCode,
							    CostCenterId = stockOutDetail.CostCenterId,
								ItemNote = stockOutDetail.ItemNote,
								StoredQuantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity,
								OpenQuantity = 0,
								InQuantity = stockOutHeader.StockTypeId == StockTypeData.StockOutFromSalesInvoice ? stockOutDetail.Quantity + stockOutDetail.BonusQuantity : 0,
								OutQuantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity,
								PendingInQuantity = 0,
								PendingOutQuantity = 0,
								ReservedQuantity = stockOutHeader.StockTypeId == StockTypeData.StockOutFromSalesInvoice ? -(stockOutDetail.Quantity + stockOutDetail.BonusQuantity) : 0,
								CostPrice = stockOutDetail.CostPrice,
								CostPackage = stockOutDetail.CostPackage,
								Price = stockOutDetail.SellingPrice,
								Reference = stockOutHeader.Reference,
							    RemarksAr = stockOutHeader.RemarksAr,
							    RemarksEn = stockOutHeader.RemarksEn,
							    Notes = stockOutDetail.Notes,
								CreatedAt = stockOutHeader.CreatedAt,
								UserNameCreated = stockOutHeader.UserNameCreated,
								ModifiedAt = stockOutHeader.ModifiedAt,
								UserNameModified = stockOutHeader.UserNameModified,

	   						    PurchaseInvoicesQuantity = 0,
	   						    SalesInvoiceReturnQuantity = 0,
	   						    StockInQuantity = 0,
								StockOutReturnQuantity = 0,
	   						    InternalTransferInQuantity = 0,
	   						    InventoryInQuantity = 0,
	   						    CarryOverInQuantity = 0,
							    DisassembleInQuantity = 0,

	   						    SalesInvoicesQuantity = salesInvoiceHeader.IsDirectInvoice == true ? stockOutDetail.Quantity + stockOutDetail.BonusQuantity : 0,
	   						    PurchaseInvoiceReturnQuantity = 0,
	   						    StockOutQuantity = salesInvoiceHeader.IsDirectInvoice != true ? stockOutDetail.Quantity + stockOutDetail.BonusQuantity : 0,
								StockInReturnQuantity = 0,
								InternalTransferOutQuantity = 0,
	   						    InventoryOutQuantity = 0,
	   						    CarryOverOutQuantity = 0,
								DisassembleOutQuantity = 0,
							};


			var stockOutReturns = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll()
								  from invoiceStockOutReturn in _invoiceStockOutReturnService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId).DefaultIfEmpty()
								  from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == invoiceStockOutReturn.SalesInvoiceReturnHeaderId).DefaultIfEmpty()
								  from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								  select new GeneralInventoryDocumentWithMenuCodeDto
								  {
									  HeaderId = salesInvoiceReturnHeader.IsDirectInvoice == true ? salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId : stockOutReturnHeader.StockOutReturnHeaderId,
									  MenuCode = (salesInvoiceReturnHeader.IsDirectInvoice == true ? salesInvoiceReturnHeader.MenuCode : stockOutReturnHeader.MenuCode) ?? 0,
									  DocumentFullCode = (
										  salesInvoiceReturnHeader.IsDirectInvoice == true ?
											  (salesInvoiceReturnHeader.Prefix + salesInvoiceReturnHeader.DocumentCode + salesInvoiceReturnHeader.Suffix) :
											  (stockOutReturnHeader.Prefix + stockOutReturnHeader.DocumentCode + stockOutReturnHeader.Suffix)
										  ),
									  StoreId = stockOutReturnHeader.StoreId,
									  ClientId = stockOutReturnHeader.ClientId,
									  SupplierId = null,
									  SellerId = stockOutReturnHeader.SellerId,
									  ItemId = stockOutReturnDetail.ItemId,
									  ItemPackageId = stockOutReturnDetail.ItemPackageId,
									  ExpireDate = stockOutReturnDetail.ExpireDate,
									  BatchNumber = stockOutReturnDetail.BatchNumber,
									  DocumentDate = stockOutReturnHeader.DocumentDate,
									  EntryDate = stockOutReturnHeader.EntryDate,
									  BarCode = stockOutReturnDetail.BarCode,
							          CostCenterId = stockOutReturnDetail.CostCenterId,
									  ItemNote = stockOutReturnDetail.ItemNote,
									  StoredQuantity = stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity,
									  OpenQuantity = 0,
									  InQuantity = stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity,
									  OutQuantity = stockOutReturnHeader.StockTypeId == StockTypeData.StockOutReturnFromInvoicedStockOut ? stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity : 0,
									  PendingInQuantity = 0,
									  PendingOutQuantity = 0,
									  ReservedQuantity = stockOutReturnHeader.StockTypeId == StockTypeData.StockOutReturnFromInvoicedStockOut ? stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity : 0,
									  CostPrice = stockOutReturnDetail.CostPrice,
									  CostPackage = stockOutReturnDetail.CostPackage,
									  Price = stockOutReturnDetail.SellingPrice,
									  Reference = stockOutReturnHeader.Reference,
							          RemarksAr = stockOutReturnHeader.RemarksAr,
							          RemarksEn = stockOutReturnHeader.RemarksEn,
							          Notes = stockOutReturnDetail.Notes,
									  CreatedAt = stockOutReturnHeader.CreatedAt,
									  UserNameCreated = stockOutReturnHeader.UserNameCreated,
									  ModifiedAt = stockOutReturnHeader.ModifiedAt,
									  UserNameModified = stockOutReturnHeader.UserNameModified,

	   						          PurchaseInvoicesQuantity = 0,
	   						          SalesInvoiceReturnQuantity = salesInvoiceReturnHeader.IsDirectInvoice == true ? stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity : 0,
	   						          StockInQuantity = 0,
								      StockOutReturnQuantity = salesInvoiceReturnHeader.IsDirectInvoice != true ? stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity : 0,
	   						          InternalTransferInQuantity = 0,
	   						          InventoryInQuantity = 0,
	   						          CarryOverInQuantity = 0,
									  DisassembleInQuantity = 0,

	   						          SalesInvoicesQuantity = 0,
	   						          PurchaseInvoiceReturnQuantity = 0,
	   						          StockOutQuantity = 0,
								      StockInReturnQuantity = 0,
								      InternalTransferOutQuantity = 0,
	   						          InventoryOutQuantity = 0,
	   						          CarryOverOutQuantity = 0,
									  DisassembleOutQuantity = 0,
								  };

            var reservationInvoices = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.IsOnTheWay)
            						  from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
            						  select new GeneralInventoryDocumentWithMenuCodeDto
            						  {
            							  HeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
            							  MenuCode = salesInvoiceHeader.MenuCode ?? 0,
            							  DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
            							  StoreId = salesInvoiceHeader.StoreId,
            							  ClientId = salesInvoiceHeader.ClientId,
            							  SupplierId = null,
            							  SellerId = salesInvoiceHeader.SellerId,
            							  ItemId = salesInvoiceDetail.ItemId,
            							  ItemPackageId = salesInvoiceDetail.ItemPackageId,
            							  ExpireDate = salesInvoiceDetail.ExpireDate,
            							  BatchNumber = salesInvoiceDetail.BatchNumber,
            							  DocumentDate = salesInvoiceHeader.DocumentDate,
            							  EntryDate = salesInvoiceHeader.EntryDate,
            							  BarCode = salesInvoiceDetail.BarCode,
            							  CostCenterId = salesInvoiceDetail.CostCenterId,
            							  ItemNote = salesInvoiceDetail.ItemNote,
            							  StoredQuantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity,
            							  OpenQuantity = 0,
            							  InQuantity = 0,
            							  OutQuantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity,
            							  PendingInQuantity = 0,
            							  PendingOutQuantity = 0,
            							  ReservedQuantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity,
            							  CostPrice = salesInvoiceDetail.CostPrice,
            							  CostPackage = salesInvoiceDetail.CostPackage,
            							  Price = salesInvoiceDetail.SellingPrice,
            							  Reference = salesInvoiceHeader.Reference,
            							  RemarksAr = salesInvoiceHeader.RemarksAr,
            							  RemarksEn = salesInvoiceHeader.RemarksEn,
            							  Notes = salesInvoiceDetail.Notes,
            							  CreatedAt = salesInvoiceHeader.CreatedAt,
            							  UserNameCreated = salesInvoiceHeader.UserNameCreated,
            							  ModifiedAt = salesInvoiceHeader.ModifiedAt,
            							  UserNameModified = salesInvoiceHeader.UserNameModified,
            
            							  PurchaseInvoicesQuantity = 0,
            							  SalesInvoiceReturnQuantity = 0,
            							  StockInQuantity = 0,
            							  StockOutReturnQuantity = 0,
            							  InternalTransferInQuantity = 0,
            							  InventoryInQuantity = 0,
            							  CarryOverInQuantity = 0,
            							  DisassembleInQuantity = 0,
            
            							  SalesInvoicesQuantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity,
            							  PurchaseInvoiceReturnQuantity = 0,
            							  StockOutQuantity = 0,
            							  StockInReturnQuantity = 0,
            							  InternalTransferOutQuantity = 0,
            							  InventoryOutQuantity = 0,
            							  CarryOverOutQuantity = 0,
            							  DisassembleOutQuantity = 0,
            						  };

			var reservationInvoiceCloseOuts = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.IsOnTheWay)
											  from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
											  select new GeneralInventoryDocumentWithMenuCodeDto
											  {
												  HeaderId = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId,
												  MenuCode = salesInvoiceReturnHeader.MenuCode ?? 0,
												  DocumentFullCode = salesInvoiceReturnHeader.Prefix + salesInvoiceReturnHeader.DocumentCode + salesInvoiceReturnHeader.Suffix,
												  StoreId = salesInvoiceReturnHeader.StoreId,
												  ClientId = salesInvoiceReturnHeader.ClientId,
												  SupplierId = null,
												  SellerId = salesInvoiceReturnHeader.SellerId,
												  ItemId = salesInvoiceReturnDetail.ItemId,
												  ItemPackageId = salesInvoiceReturnDetail.ItemPackageId,
												  ExpireDate = salesInvoiceReturnDetail.ExpireDate,
												  BatchNumber = salesInvoiceReturnDetail.BatchNumber,
												  DocumentDate = salesInvoiceReturnHeader.DocumentDate,
												  EntryDate = salesInvoiceReturnHeader.EntryDate,
												  BarCode = salesInvoiceReturnDetail.BarCode,
												  CostCenterId = salesInvoiceReturnDetail.CostCenterId,
												  ItemNote = salesInvoiceReturnDetail.ItemNote,
												  StoredQuantity = salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity,
												  OpenQuantity = 0,
												  InQuantity = salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity,
												  OutQuantity = 0,
												  PendingInQuantity = 0,
												  PendingOutQuantity = 0,
												  ReservedQuantity = -(salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity),
												  CostPrice = salesInvoiceReturnDetail.CostPrice,
												  CostPackage = salesInvoiceReturnDetail.CostPackage,
												  Price = salesInvoiceReturnDetail.SellingPrice,
												  Reference = salesInvoiceReturnHeader.Reference,
												  RemarksAr = salesInvoiceReturnHeader.RemarksAr,
												  RemarksEn = salesInvoiceReturnHeader.RemarksEn,
												  Notes = salesInvoiceReturnDetail.Notes,
												  CreatedAt = salesInvoiceReturnHeader.CreatedAt,
												  UserNameCreated = salesInvoiceReturnHeader.UserNameCreated,
												  ModifiedAt = salesInvoiceReturnHeader.ModifiedAt,
												  UserNameModified = salesInvoiceReturnHeader.UserNameModified,

												  PurchaseInvoicesQuantity = 0,
												  SalesInvoiceReturnQuantity = salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity,
												  StockInQuantity = 0,
												  StockOutReturnQuantity = 0,
												  InternalTransferInQuantity = 0,
												  InventoryInQuantity = 0,
												  CarryOverInQuantity = 0,
												  DisassembleInQuantity = 0,

												  SalesInvoicesQuantity = 0,
												  PurchaseInvoiceReturnQuantity = 0,
												  StockOutQuantity = 0,
												  StockInReturnQuantity = 0,
												  InternalTransferOutQuantity = 0,
												  InventoryOutQuantity = 0,
												  CarryOverOutQuantity = 0,
												  DisassembleOutQuantity = 0,
											  };

			var itemDisassembles = from itemDisassembleHeader in _itemDisassembleHeaderService.GetAll()
								   from itemDisassemble in _itemDisassembleService.GetAll().Where(x => x.ItemDisassembleHeaderId == itemDisassembleHeader.ItemDisassembleHeaderId)
								   select new GeneralInventoryDocumentWithMenuCodeDto
								   {
									   HeaderId = itemDisassembleHeader.ItemDisassembleHeaderId,
									   MenuCode = MenuCodeData.DisassembleItemPackages, //TODO: when implementing automatic item disassemble, update this to handle it
									   DocumentFullCode = itemDisassembleHeader.ItemDisassembleCode.ToString(),
									   StoreId = itemDisassembleHeader.StoreId,
									   ClientId = null,
									   SupplierId = null,
									   SellerId = null,
									   ItemId = itemDisassemble.ItemId,
									   ItemPackageId = itemDisassemble.ItemPackageId,
									   ExpireDate = itemDisassemble.ExpireDate,
									   BatchNumber = itemDisassemble.BatchNumber,
									   DocumentDate = itemDisassembleHeader.DocumentDate,
									   EntryDate = itemDisassembleHeader.EntryDate,
									   BarCode = null,
							           CostCenterId = null,
									   ItemNote = null,
									   StoredQuantity = 0,
									   OpenQuantity = 0,
									   InQuantity = itemDisassemble.InQuantity,
									   OutQuantity = itemDisassemble.OutQuantity,
									   PendingInQuantity = 0,
									   PendingOutQuantity = 0,
									   ReservedQuantity = 0,
									   CostPrice = 0, //TODO: cost price in item disassembles
									   CostPackage = 0, //TODO: cost value in item disassembles
									   Price = 0, //TODO: price in item disassembles
									   Reference = null,
							           RemarksAr = itemDisassembleHeader.RemarksAr,
							           RemarksEn = itemDisassembleHeader.RemarksEn,
							           Notes = null,
									   CreatedAt = itemDisassembleHeader.CreatedAt,
									   UserNameCreated = itemDisassembleHeader.UserNameCreated,
									   ModifiedAt = itemDisassembleHeader.ModifiedAt,
									   UserNameModified = itemDisassembleHeader.UserNameModified,

	   						           PurchaseInvoicesQuantity = 0,
	   						           SalesInvoiceReturnQuantity = 0,
	   						           StockInQuantity = 0,
								       StockOutReturnQuantity = 0,
	   						           InternalTransferInQuantity = 0,
	   						           InventoryInQuantity = 0,
	   						           CarryOverInQuantity = 0,
									   DisassembleInQuantity = itemDisassemble.InQuantity,

	   						           SalesInvoicesQuantity = 0,
	   						           PurchaseInvoiceReturnQuantity = 0,
	   						           StockOutQuantity = 0,
								       StockInReturnQuantity = 0,
								       InternalTransferOutQuantity = 0,
	   						           InventoryOutQuantity = 0,
	   						           CarryOverOutQuantity = 0,
									   DisassembleOutQuantity = itemDisassemble.OutQuantity,
								   };

			return inventoryIns.Concat(inventoryOuts).Concat(internalTransfers).Concat(internalTransferReceivesFromStore).Concat(internalTransferReceivesToStore).Concat(carryOverEffects).Concat(stockIns).Concat(stockInReturns).Concat(stockOuts).Concat(stockOutReturns).Concat(reservationInvoices).Concat(reservationInvoiceCloseOuts).Concat(itemDisassembles);
		}
	}
}
