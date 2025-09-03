using Purchases.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Contracts.Inventory;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service.Logic.Calculation;
using Shared.CoreOne.Contracts.Taxes;
using Shared.Service.Services.Taxes;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.StaticData;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Shared.Service.Services.Items;
using Shared.Service.Services.Modules;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Purchases.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;

namespace Purchases.Service.Services
{
	public class PurchaseInvoiceService : IPurchaseInvoiceService
	{
		private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceDetailTaxService _purchaseInvoiceDetailTaxService;
		private readonly IPurchaseInvoiceExpenseService _purchaseInvoiceExpenseService;
		private readonly IJournalService _journalService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IItemService _itemService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;

		public PurchaseInvoiceService(IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailTaxService purchaseInvoiceDetailTaxService, IPurchaseInvoiceExpenseService purchaseInvoiceExpenseService, IJournalService journalService, IMenuNoteService menuNoteService, IItemService itemService, IItemPackageService itemPackageService, IItemPackingService itemPackingService, IGenericMessageService genericMessageService, ICostCenterJournalDetailService costCenterJournalDetailService)
		{
			_purchaseInvoiceDetailService = purchaseInvoiceDetailService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceDetailTaxService = purchaseInvoiceDetailTaxService;
			_purchaseInvoiceExpenseService = purchaseInvoiceExpenseService;
			_journalService = journalService;
			_menuNoteService = menuNoteService;
			_itemService = itemService;
			_itemPackageService = itemPackageService;
			_itemPackingService = itemPackingService;
			_genericMessageService = genericMessageService;
			_costCenterJournalDetailService = costCenterJournalDetailService;
		}

		public List<RequestChangesDto> GetPurchaseInvoiceRequestChanges(PurchaseInvoiceDto oldItem, PurchaseInvoiceDto newItem)
		{
			var requestChanges = new List<RequestChangesDto>();
			var items = CompareLogic.GetDifferences(oldItem.PurchaseInvoiceHeader, newItem.PurchaseInvoiceHeader);
			requestChanges.AddRange(items);

			if (oldItem.PurchaseInvoiceDetails.Any() && newItem.PurchaseInvoiceDetails.Any())
			{
				var oldCount = oldItem.PurchaseInvoiceDetails.Count;
				var newCount = newItem.PurchaseInvoiceDetails.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.PurchaseInvoiceDetails[i], newItem.PurchaseInvoiceDetails[i]);
							requestChanges.AddRange(changes);

							var detailTaxChanges = GetPurchaseInvoiceDetailTaxesRequestChanges(oldItem.PurchaseInvoiceDetails[i], newItem.PurchaseInvoiceDetails[i]);
							requestChanges.AddRange(detailTaxChanges);

							index++;
							break;
						}
					}
				}
			}

			if (oldItem.MenuNotes != null && oldItem.MenuNotes.Any() && newItem.MenuNotes != null && newItem.MenuNotes.Any())
			{
				var oldCount = oldItem.MenuNotes.Count;
				var newCount = newItem.MenuNotes.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.MenuNotes[i], newItem.MenuNotes[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}

			if (oldItem.PurchaseInvoiceExpenses != null && oldItem.PurchaseInvoiceExpenses.Any() && newItem.PurchaseInvoiceExpenses != null && newItem.PurchaseInvoiceExpenses.Any())
			{
				var oldCount = oldItem.PurchaseInvoiceExpenses.Count;
				var newCount = newItem.PurchaseInvoiceExpenses.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.PurchaseInvoiceExpenses[i], newItem.PurchaseInvoiceExpenses[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}

			if(oldItem.Journal != null && newItem.Journal != null)
			{
				var journalChanges = _journalService.GetJournalEntryRequestChanges(oldItem.Journal, newItem.Journal);
                requestChanges.AddRange(journalChanges);
            }

			requestChanges.RemoveAll(x => x.ColumnName == "PurchaseInvoiceDetailTaxes" || x.ColumnName == "ItemTaxData");

			return requestChanges;
		}

		private static List<RequestChangesDto> GetPurchaseInvoiceDetailTaxesRequestChanges(PurchaseInvoiceDetailDto oldItem, PurchaseInvoiceDetailDto newItem)
		{
			var requestChanges = new List<RequestChangesDto>();

			if (oldItem.PurchaseInvoiceDetailTaxes.Any() && newItem.PurchaseInvoiceDetailTaxes.Any())
			{
				var oldCount = oldItem.PurchaseInvoiceDetailTaxes.Count;
				var newCount = newItem.PurchaseInvoiceDetailTaxes.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.PurchaseInvoiceDetailTaxes[i], newItem.PurchaseInvoiceDetailTaxes[i]);
							requestChanges.AddRange(changes);

							index++;
							break;
						}
					}
				}
			}

			return requestChanges;
		}

		public async Task<PurchaseInvoiceDto> GetPurchaseInvoice(int purchaseInvoiceHeaderId)
		{
			var header = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);
			if (header == null) return new PurchaseInvoiceDto();

			var details = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetails(purchaseInvoiceHeaderId);
			var menuNotes = await _menuNoteService.GetMenuNotes(PurchaseInvoiceMenuCodeHelper.GetMenuCode(header), header.PurchaseInvoiceHeaderId).ToListAsync();
			var purchaseInvoiceDetailTaxes = await _purchaseInvoiceDetailTaxService.GetPurchaseInvoiceDetailTaxes(purchaseInvoiceHeaderId).ToListAsync();
			var purchaseInvoiceExpenses = await _purchaseInvoiceExpenseService.GetPurchaseInvoiceExpenses(purchaseInvoiceHeaderId);
			var journal = await _journalService.GetJournal(header.JournalHeaderId);

			foreach (var detail in details)
			{
				detail.PurchaseInvoiceDetailTaxes = purchaseInvoiceDetailTaxes.Where(x => x.PurchaseInvoiceDetailId == detail.PurchaseInvoiceDetailId).ToList();
			}

			return new PurchaseInvoiceDto() { PurchaseInvoiceHeader = header, PurchaseInvoiceDetails = details, PurchaseInvoiceExpenses = purchaseInvoiceExpenses, Journal = journal, MenuNotes = menuNotes };
		}

		public async Task<ResponseDto> SavePurchaseInvoice(PurchaseInvoiceDto purchaseInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference = null)
		{
			if (purchaseInvoice.PurchaseInvoiceHeader != null)
			{
				var menuCode = (short)PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoice.PurchaseInvoiceHeader);
				if (purchaseInvoice.Journal != null)
				{
					purchaseInvoice.Journal.JournalHeader!.MenuCode = menuCode;
					purchaseInvoice.Journal.JournalHeader!.MenuReferenceId = await _purchaseInvoiceHeaderService.GetNextId();

					var journalResult = await _journalService.SaveJournal(purchaseInvoice.Journal, hasApprove, approved, requestId);

					if (journalResult.Success)
					{
						purchaseInvoice.PurchaseInvoiceHeader.JournalHeaderId = journalResult.Id;
					}
					else
					{
						return journalResult;
					}
				}

				var result = await _purchaseInvoiceHeaderService.SavePurchaseInvoiceHeader(purchaseInvoice.PurchaseInvoiceHeader, hasApprove, approved, requestId, documentReference);
				if (result.Success)
				{
					var modifiedPurchaseInvoiceDetails = await _purchaseInvoiceDetailService.SavePurchaseInvoiceDetails(result.Id, purchaseInvoice.PurchaseInvoiceDetails);
					await SavePurchaseInvoiceDetailTaxes(result.Id, modifiedPurchaseInvoiceDetails);
					await UpdateCostCenterJournalDetails(result.Id, modifiedPurchaseInvoiceDetails, menuCode);
					await _purchaseInvoiceDetailService.DeletePurchaseInvoiceDetailList(modifiedPurchaseInvoiceDetails, result.Id);

					await _purchaseInvoiceExpenseService.SavePurchaseInvoiceExpenses(result.Id, purchaseInvoice.PurchaseInvoiceExpenses);
					if (purchaseInvoice.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(purchaseInvoice.MenuNotes, result.Id);
					}
				}

				return result;
			}
			return new ResponseDto { Message = "Header should not be null"};
		}

		private async Task UpdateCostCenterJournalDetails(int purchaseInvoiceHeaderId, List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails, short menuCode)
		{
			await _costCenterJournalDetailService.UpdateCostCenterJournalDetailsBasedOnInvoiceDetails(
				invoiceHeaderId: purchaseInvoiceHeaderId, 
				invoiceDetails: purchaseInvoiceDetails,
				detailIdSelector: x => x.PurchaseInvoiceDetailId,
				itemIdSelector: x => x.ItemId,
				costCenterIdSelector: x => x.CostCenterId,
				creditValueSelector: x => 0,
				debitValueSelector: x => x.NetValue,
				remarksSelector: x => x.Notes,
				menuCode: menuCode
			);
		}

		private async Task SavePurchaseInvoiceDetailTaxes(int purchaseInvoiceHeaderId, List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails)
		{
			List<PurchaseInvoiceDetailTaxDto> purchaseInvoiceDetailTaxes = new List<PurchaseInvoiceDetailTaxDto>();

			foreach (var purchaseInvoiceDetail in purchaseInvoiceDetails)
			{
				foreach (var purchaseInvoiceDetailTax in purchaseInvoiceDetail.PurchaseInvoiceDetailTaxes)
				{
					purchaseInvoiceDetailTax.PurchaseInvoiceDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId;
					purchaseInvoiceDetailTaxes.Add(purchaseInvoiceDetailTax);
				}
			}

			await _purchaseInvoiceDetailTaxService.SavePurchaseInvoiceDetailTaxes(purchaseInvoiceHeaderId, purchaseInvoiceDetailTaxes);
		}

		public async Task<ResponseDto> DeletePurchaseInvoice(int purchaseInvoiceHeaderId, int menuCode)
		{
			var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);
			if (purchaseInvoiceHeader == null) return new ResponseDto{ Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound)};

			await _costCenterJournalDetailService.DeleteCostCenterJournalDetails(purchaseInvoiceHeaderId, (short)menuCode);
			await _menuNoteService.DeleteMenuNotes(menuCode, purchaseInvoiceHeaderId);
			await _purchaseInvoiceDetailTaxService.DeletePurchaseInvoiceDetailTaxes(purchaseInvoiceHeaderId);
			await _purchaseInvoiceDetailService.DeletePurchaseInvoiceDetails(purchaseInvoiceHeaderId);
			await _purchaseInvoiceExpenseService.DeletePurchaseInvoiceExpenses(purchaseInvoiceHeaderId);

			var purchaseInvoiceResult = await _purchaseInvoiceHeaderService.DeletePurchaseInvoiceHeader(purchaseInvoiceHeaderId, menuCode);
			if (purchaseInvoiceResult.Success == false)
			{
				return purchaseInvoiceResult;
			}

			var journalResult = await _journalService.DeleteJournal(purchaseInvoiceHeader.JournalHeaderId);
			if (journalResult.Success == false)
			{
				return journalResult;
			}

			return purchaseInvoiceResult;
		}

		public async Task<decimal> GetLastPurchasePrice(int itemId, int itemPackageId)
		{
			return await _purchaseInvoiceDetailService.GetAll().Where(x => x.ItemId == itemId && x.ItemPackageId == itemPackageId).OrderByDescending(x => x.PurchaseInvoiceDetailId).Select(x => x.PurchasePrice).FirstOrDefaultAsync();
		}

        public async Task<List<LastPurchasePriceDto>> GetMultipleLastPurchasePrices(List<int> itemIds)
        {
            var invoices = await _purchaseInvoiceDetailService.GetAll().GroupBy(x => new { x.ItemId, x.ItemPackageId }).Select(x => new LastPurchasePriceDto
            {
                ItemId = x.Key.ItemId,
                ItemPackageId = x.Key.ItemPackageId,
                PurchasePrice = x.OrderByDescending(y => y.PurchaseInvoiceHeaderId).Select(x => x.PurchasePrice).FirstOrDefault()
            }).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			return invoices;
        }

        public async Task<List<QuantityPriceDto>> GetLatestInvoicesBasedOnQuantity(int currentPurchaseInvoiceHeaderId,int itemId, decimal itemStockQuantity)
		{
			var invoiceData =
			   await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x=>x.PurchaseInvoiceHeaderId != currentPurchaseInvoiceHeaderId)
					  from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId && x.ItemId == itemId)
					  orderby purchaseInvoiceHeader.DocumentDate descending, purchaseInvoiceDetail.PurchaseInvoiceDetailId descending
					  select new ItemQuantityPriceDto
					  {
						  ItemId = purchaseInvoiceDetail.ItemId,
						  Quantity = (purchaseInvoiceDetail.Quantity + purchaseInvoiceDetail.BonusQuantity) * purchaseInvoiceDetail.Packing,
						  Price = purchaseInvoiceDetail.PurchasePrice / purchaseInvoiceDetail.Packing
					  }).Take(1000).ToListAsync();

			var returnModelList = new List<QuantityPriceDto>();

			decimal itemQuantity = 0;
			foreach (var invoice in invoiceData)
			{
				if ((itemQuantity + invoice.Quantity) > itemStockQuantity)
				{
					var returnModel = new QuantityPriceDto() { Price = invoice.Price, Quantity =  (itemStockQuantity - itemQuantity) };
					returnModelList.Add(returnModel);
					break;

				}
				else if ((itemQuantity + invoice.Quantity) == itemStockQuantity)
				{
					var returnModel = new QuantityPriceDto() { Price = invoice.Price, Quantity = invoice.Quantity };
					returnModelList.Add(returnModel); 
					break;
				}
				else
				{
					var returnModel = new QuantityPriceDto() { Price = invoice.Price, Quantity = invoice.Quantity };
					itemQuantity += invoice.Quantity;
					returnModelList.Add(returnModel);
				}
			}

			return returnModelList;
		}
	}
}
