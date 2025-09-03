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
using static System.Runtime.InteropServices.JavaScript.JSType;
using Shared.Service.Services.Modules;
using Shared.CoreOne.Contracts.Journal;
using System.Globalization;
using System.Reflection.PortableExecutable;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Purchases.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Extensions;

namespace Purchases.Service.Services
{
	public class PurchaseInvoiceReturnService : IPurchaseInvoiceReturnService
	{
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IPurchaseInvoiceReturnDetailTaxService _purchaseInvoiceReturnDetailTaxService;
		private readonly IJournalService _journalService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IItemTaxService _itemTaxService;
		private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;

		public PurchaseInvoiceReturnService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailTaxService purchaseInvoiceReturnDetailTaxService, IJournalService journalService, IMenuNoteService menuNoteService, IGenericMessageService genericMessageService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, ICostCenterJournalDetailService costCenterJournalDetailService)
		{
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_purchaseInvoiceReturnDetailTaxService = purchaseInvoiceReturnDetailTaxService;
			_journalService = journalService;
			_menuNoteService = menuNoteService;
			_genericMessageService = genericMessageService;
			_purchaseInvoiceDetailService = purchaseInvoiceDetailService;
			_itemBarCodeService = itemBarCodeService;
			_itemTaxService = itemTaxService;
			_costCenterJournalDetailService = costCenterJournalDetailService;
		}

		public List<RequestChangesDto> GetPurchaseInvoiceReturnRequestChanges(PurchaseInvoiceReturnDto oldItem, PurchaseInvoiceReturnDto newItem)
		{
			var requestChanges = new List<RequestChangesDto>();
			var items = CompareLogic.GetDifferences(oldItem.PurchaseInvoiceReturnHeader, newItem.PurchaseInvoiceReturnHeader);
			requestChanges.AddRange(items);

			if (oldItem.PurchaseInvoiceReturnDetails.Any() && newItem.PurchaseInvoiceReturnDetails.Any())
			{
				var oldCount = oldItem.PurchaseInvoiceReturnDetails.Count;
				var newCount = newItem.PurchaseInvoiceReturnDetails.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.PurchaseInvoiceReturnDetails[i], newItem.PurchaseInvoiceReturnDetails[i]);
							requestChanges.AddRange(changes);

							var detailTaxChanges = GetPurchaseInvoiceReturnDetailTaxesRequestChanges(oldItem.PurchaseInvoiceReturnDetails[i], newItem.PurchaseInvoiceReturnDetails[i]);
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

            if (oldItem.Journal != null && newItem.Journal != null)
            {
                var journalChanges = _journalService.GetJournalEntryRequestChanges(oldItem.Journal, newItem.Journal);
				requestChanges.AddRange(journalChanges);
            }

            requestChanges.RemoveAll(x => x.ColumnName == "PurchaseInvoiceReturnDetailTaxes" || x.ColumnName == "ItemTaxData");

			return requestChanges;
		}

		private static List<RequestChangesDto> GetPurchaseInvoiceReturnDetailTaxesRequestChanges(PurchaseInvoiceReturnDetailDto oldItem, PurchaseInvoiceReturnDetailDto newItem)
		{
			var requestChanges = new List<RequestChangesDto>();

			if (oldItem.PurchaseInvoiceReturnDetailTaxes.Any() && newItem.PurchaseInvoiceReturnDetailTaxes.Any())
			{
				var oldCount = oldItem.PurchaseInvoiceReturnDetailTaxes.Count;
				var newCount = newItem.PurchaseInvoiceReturnDetailTaxes.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.PurchaseInvoiceReturnDetailTaxes[i], newItem.PurchaseInvoiceReturnDetailTaxes[i]);
							requestChanges.AddRange(changes);

							index++;
							break;
						}
					}
				}
			}

			return requestChanges;
		}

		public async Task<PurchaseInvoiceReturnDto> GetPurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId)
		{
			var header = await _purchaseInvoiceReturnHeaderService.GetPurchaseInvoiceReturnHeaderById(purchaseInvoiceReturnHeaderId);
			if (header == null) return new PurchaseInvoiceReturnDto();

			var details = await GetPurchaseInvoiceReturnDetailsCalculated(purchaseInvoiceReturnHeaderId, header);
			var menuNotes = await _menuNoteService.GetMenuNotes(purchaseInvoiceReturnHeaderId, PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(header)).ToListAsync();
			var purchaseInvoiceReturnDetailTaxes = await _purchaseInvoiceReturnDetailTaxService.GetPurchaseInvoiceReturnDetailTaxes(purchaseInvoiceReturnHeaderId).ToListAsync();
			var journal = await _journalService.GetJournal(header.JournalHeaderId);

			foreach (var detail in details)
			{
				detail.PurchaseInvoiceReturnDetailTaxes = purchaseInvoiceReturnDetailTaxes.Where(x => x.PurchaseInvoiceReturnDetailId == detail.PurchaseInvoiceReturnDetailId).ToList();
			}

			return new PurchaseInvoiceReturnDto() { PurchaseInvoiceReturnHeader = header, PurchaseInvoiceReturnDetails = details, Journal = journal, MenuNotes = menuNotes };
		}

		public async Task<List<PurchaseInvoiceReturnDetailDto>> GetPurchaseInvoiceReturnDetailsCalculated(int purchaseInvoiceReturnHeaderId, PurchaseInvoiceReturnHeaderDto? purchaseInvoiceReturnHeader = null, List<PurchaseInvoiceReturnDetailDto>? purchaseInvoiceReturnDetails = null)
		{
			purchaseInvoiceReturnHeader ??= await _purchaseInvoiceReturnHeaderService.GetPurchaseInvoiceReturnHeaderById(purchaseInvoiceReturnHeaderId);
			var purchaseInvoiceHeaderId = purchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId;
			var isDirectInvoice = purchaseInvoiceReturnHeader!.IsDirectInvoice;

			purchaseInvoiceReturnDetails ??= await _purchaseInvoiceReturnDetailService.GetPurchaseInvoiceReturnDetailsAsQueryable(purchaseInvoiceReturnHeaderId).ToListAsync();
			var itemIds = purchaseInvoiceReturnDetails.Select(x => x.ItemId).ToList();

			var purchaseInvoiceDetailsGrouped = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGrouped(purchaseInvoiceHeaderId);

			purchaseInvoiceReturnDetails = (
				from purchaseInvoiceReturnDetail in purchaseInvoiceReturnDetails
				from purchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId && x.ItemPackageId == purchaseInvoiceReturnDetail.ItemPackageId && x.CostCenterId == purchaseInvoiceReturnDetail.CostCenterId && x.BarCode == purchaseInvoiceReturnDetail.BarCode && x.PurchasePrice == purchaseInvoiceReturnDetail.PurchasePrice).DefaultIfEmpty()
				select new PurchaseInvoiceReturnDetailDto
				{
					PurchaseInvoiceReturnDetailId = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId,
					PurchaseInvoiceReturnHeaderId = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnHeaderId,
					CostCenterId = purchaseInvoiceReturnDetail.CostCenterId,
					CostCenterName = purchaseInvoiceReturnDetail.CostCenterName,
					ItemId = purchaseInvoiceReturnDetail.ItemId,
					ItemCode = purchaseInvoiceReturnDetail.ItemCode,
					ItemName = purchaseInvoiceReturnDetail.ItemName,
					TaxTypeId = purchaseInvoiceReturnDetail.TaxTypeId,
					ItemTypeId = purchaseInvoiceReturnDetail.ItemTypeId,
					ItemPackageId = purchaseInvoiceReturnDetail.ItemPackageId,
					ItemPackageName = purchaseInvoiceReturnDetail.ItemPackageName,
					IsItemVatInclusive = purchaseInvoiceReturnDetail.IsItemVatInclusive,
					BarCode = purchaseInvoiceReturnDetail.BarCode,
					Packing = purchaseInvoiceReturnDetail.Packing,
					ExpireDate = purchaseInvoiceReturnDetail.ExpireDate,
					BatchNumber = purchaseInvoiceReturnDetail.BatchNumber,
					Quantity = purchaseInvoiceReturnDetail.Quantity,
					BonusQuantity = purchaseInvoiceReturnDetail.BonusQuantity,
					PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0,
					PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0,
					AvailableQuantity = isDirectInvoice? (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0) : 0,
					AvailableBonusQuantity = isDirectInvoice? (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0) : 0,
					PurchasePrice = purchaseInvoiceReturnDetail.PurchasePrice,
					TotalValue = purchaseInvoiceReturnDetail.TotalValue,
					ItemDiscountPercent = purchaseInvoiceReturnDetail.ItemDiscountPercent,
					ItemDiscountValue = purchaseInvoiceReturnDetail.ItemDiscountValue,
					TotalValueAfterDiscount = purchaseInvoiceReturnDetail.TotalValueAfterDiscount,
					HeaderDiscountValue = purchaseInvoiceReturnDetail.HeaderDiscountValue,
					GrossValue = purchaseInvoiceReturnDetail.GrossValue,
					VatPercent = purchaseInvoiceReturnDetail.VatPercent,
					VatValue = purchaseInvoiceReturnDetail.VatValue,
					SubNetValue = purchaseInvoiceReturnDetail.SubNetValue,
					OtherTaxValue = purchaseInvoiceReturnDetail.OtherTaxValue,
					NetValue = purchaseInvoiceReturnDetail.NetValue,
					Notes = purchaseInvoiceReturnDetail.Notes,
					ItemNote = purchaseInvoiceReturnDetail.ItemNote,
					ConsumerPrice = purchaseInvoiceReturnDetail.ConsumerPrice,
					CostPrice = purchaseInvoiceReturnDetail.CostPrice,
					CostPackage = purchaseInvoiceReturnDetail.CostPackage,
					LastPurchasePrice = purchaseInvoiceReturnDetail.LastPurchasePrice,

					PurchaseInvoiceReturnDetailTaxes = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailTaxes,

					CreatedAt = purchaseInvoiceReturnDetail.CreatedAt,
					IpAddressCreated = purchaseInvoiceReturnDetail.IpAddressCreated,
					UserNameCreated = purchaseInvoiceReturnDetail.UserNameCreated,
				}).ToList();


			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
			var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

			foreach (var purchaseInvoiceReturnDetail in purchaseInvoiceReturnDetails)
			{
				purchaseInvoiceReturnDetail.Packages = packages.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				purchaseInvoiceReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId).ToList();
				purchaseInvoiceReturnDetail.Taxes = purchaseInvoiceReturnDetail.ItemTaxData.ToJson();
			}

			return purchaseInvoiceReturnDetails;
		}

		public async Task<ResponseDto> SavePurchaseInvoiceReturn(PurchaseInvoiceReturnDto purchaseInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null)
		{
			if (purchaseInvoiceReturn.PurchaseInvoiceReturnHeader != null)
			{
				var menuCode = (short)PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader);
				if (purchaseInvoiceReturn.Journal != null)
				{
					purchaseInvoiceReturn.Journal.JournalHeader!.MenuCode = menuCode;
					purchaseInvoiceReturn.Journal.JournalHeader!.MenuReferenceId = await _purchaseInvoiceReturnHeaderService.GetNextId();
					var journalResult = await _journalService.SaveJournal(purchaseInvoiceReturn.Journal, hasApprove, approved, requestId);

					if (journalResult.Success)
					{
						purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.JournalHeaderId = journalResult.Id;
					}
					else
					{
						return journalResult;
					}
				}

				var result = await _purchaseInvoiceReturnHeaderService.SavePurchaseInvoiceReturnHeader(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader, hasApprove, approved, requestId, documentReference);
				if (result.Success)
				{
					var modifiedPurchaseInvoiceReturnDetails = await _purchaseInvoiceReturnDetailService.SavePurchaseInvoiceReturnDetails(result.Id, purchaseInvoiceReturn.PurchaseInvoiceReturnDetails);
					await SavePurchaseInvoiceReturnDetailTaxes(result.Id, modifiedPurchaseInvoiceReturnDetails);
					await UpdateCostCenterJournalDetails(result.Id, modifiedPurchaseInvoiceReturnDetails, menuCode);
					await _purchaseInvoiceReturnDetailService.DeletePurchaseInvoiceReturnDetailList(modifiedPurchaseInvoiceReturnDetails, result.Id);

					if (purchaseInvoiceReturn.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(purchaseInvoiceReturn.MenuNotes, result.Id);
					}
				}

				return result;
			}
			return new ResponseDto { Message = "Header should not be null"};
		}

		private async Task UpdateCostCenterJournalDetails(int purchaseInvoiceReturnHeaderId, List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails, short menuCode)
		{
			await _costCenterJournalDetailService.UpdateCostCenterJournalDetailsBasedOnInvoiceDetails(
				invoiceHeaderId: purchaseInvoiceReturnHeaderId, 
				invoiceDetails: purchaseInvoiceReturnDetails,
				detailIdSelector: x => x.PurchaseInvoiceReturnDetailId,
				itemIdSelector: x => x.ItemId,
				costCenterIdSelector: x => x.CostCenterId,
				creditValueSelector: x => x.NetValue,
				debitValueSelector: x => 0,
				remarksSelector: x => x.Notes,
				menuCode: menuCode
			);
		}


		private async Task SavePurchaseInvoiceReturnDetailTaxes(int purchaseInvoiceReturnHeaderId, List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails)
		{
			List<PurchaseInvoiceReturnDetailTaxDto> purchaseInvoiceReturnDetailTaxes = new List<PurchaseInvoiceReturnDetailTaxDto>();

			foreach (var purchaseInvoiceReturnDetail in purchaseInvoiceReturnDetails)
			{
				foreach (var purchaseInvoiceReturnDetailTax in purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailTaxes)
				{
					purchaseInvoiceReturnDetailTax.PurchaseInvoiceReturnDetailId = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId;
					purchaseInvoiceReturnDetailTaxes.Add(purchaseInvoiceReturnDetailTax);
				}
			}

			await _purchaseInvoiceReturnDetailTaxService.SavePurchaseInvoiceReturnDetailTaxes(purchaseInvoiceReturnHeaderId, purchaseInvoiceReturnDetailTaxes);
		}

		public async Task<ResponseDto> DeletePurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId, int menuCode)
		{
			var purchaseInvoiceReturnHeader = await _purchaseInvoiceReturnHeaderService.GetPurchaseInvoiceReturnHeaderById(purchaseInvoiceReturnHeaderId);
			if (purchaseInvoiceReturnHeader == null) return new ResponseDto{ Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound)};

			await _costCenterJournalDetailService.DeleteCostCenterJournalDetails(purchaseInvoiceReturnHeaderId, (short)menuCode);
			await _menuNoteService.DeleteMenuNotes(menuCode, purchaseInvoiceReturnHeaderId);
			await _purchaseInvoiceReturnDetailTaxService.DeletePurchaseInvoiceReturnDetailTaxes(purchaseInvoiceReturnHeaderId);
			await _purchaseInvoiceReturnDetailService.DeletePurchaseInvoiceReturnDetails(purchaseInvoiceReturnHeaderId);

			var purchaseInvoiceReturnResult = await _purchaseInvoiceReturnHeaderService.DeletePurchaseInvoiceReturnHeader(purchaseInvoiceReturnHeaderId, menuCode);
			if (purchaseInvoiceReturnResult.Success == false)
			{
				return purchaseInvoiceReturnResult;
			}

			var journalResult = await _journalService.DeleteJournal(purchaseInvoiceReturnHeader.JournalHeaderId);
			if (journalResult.Success == false)
			{
				return journalResult;
			}

			return purchaseInvoiceReturnResult;
		}
	}
}
