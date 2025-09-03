using Sales.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.Dtos.ViewModels;
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
using Sales.CoreOne.Models.Domain;
using Shared.Helper.Extensions;
using Shared.Service.Services.Items;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.Service.Services.Inventory;
using Shared.CoreOne.Models.Domain.Items;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;

namespace Sales.Service.Services
{
    public class ProformaInvoiceService : IProformaInvoiceService
    {
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IStockOutQuantityService _stockOutQuantityService;
        private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
        private readonly IStockOutHeaderService _stockOutHeaderService;
        private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
        private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
        private readonly IClientQuotationApprovalHeaderService _clientQuotationApprovalHeaderService;
        private readonly IClientQuotationApprovalDetailService _clientQuotationApprovalDetailService;
        private readonly IClientQuotationApprovalDetailTaxService _clientQuotationApprovalDetailTaxService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IProformaInvoiceDetailTaxService _proformaInvoiceDetailTaxService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemService _itemService;
        private readonly ICostCenterService _costCenterService;
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemPackingService _itemPackingService;
        private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly ITaxService _taxService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly IZeroStockValidationService _zeroStockValidationService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

        public ProformaInvoiceService(ISalesInvoiceHeaderService salesInvoiceHeaderService, IStockOutQuantityService stockOutQuantityService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutHeaderService stockOutHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IProformaInvoiceDetailService proformaInvoiceDetailService, IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IClientQuotationApprovalDetailService clientQuotationApprovalDetailService, IClientQuotationApprovalDetailTaxService clientQuotationApprovalDetailTaxService, IMenuNoteService menuNoteService, IProformaInvoiceDetailTaxService proformaInvoiceDetailTaxService, IHttpContextAccessor httpContextAccessor, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, IItemCostService itemCostService, IItemPackingService itemPackingService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceService salesInvoiceService, ITaxPercentService taxPercentService, ITaxService taxService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IGenericMessageService genericMessageService, IItemCurrentBalanceService itemCurrentBalanceService, IZeroStockValidationService zeroStockValidationService, IItemNoteValidationService itemNoteValidationService)
        {
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _stockOutQuantityService = stockOutQuantityService;
            _stockOutReturnHeaderService = stockOutReturnHeaderService;
            _stockOutHeaderService = stockOutHeaderService;
            _proformaInvoiceHeaderService = proformaInvoiceHeaderService;
            _proformaInvoiceDetailService = proformaInvoiceDetailService;
            _clientQuotationApprovalDetailService = clientQuotationApprovalDetailService;
            _clientQuotationApprovalDetailTaxService = clientQuotationApprovalDetailTaxService;
            _clientQuotationApprovalHeaderService = clientQuotationApprovalHeaderService;
            _menuNoteService = menuNoteService;
            _proformaInvoiceDetailTaxService = proformaInvoiceDetailTaxService;
            _httpContextAccessor = httpContextAccessor;
            _itemService = itemService;
            _costCenterService = costCenterService;
            _itemPackageService = itemPackageService;
            _itemBarCodeService = itemBarCodeService;
            _itemTaxService = itemTaxService;
            _itemCostService = itemCostService;
            _itemPackingService = itemPackingService;
            _salesInvoiceDetailService = salesInvoiceDetailService;
            _salesInvoiceService = salesInvoiceService;
            _taxPercentService = taxPercentService;
            _taxService = taxService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _genericMessageService = genericMessageService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _zeroStockValidationService = zeroStockValidationService;
            _itemNoteValidationService = itemNoteValidationService;
        }

        public List<RequestChangesDto> GetProformaInvoiceRequestChanges(ProformaInvoiceDto oldItem, ProformaInvoiceDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.ProformaInvoiceHeader, newItem.ProformaInvoiceHeader);
            requestChanges.AddRange(items);

            if (oldItem.ProformaInvoiceDetails.Any() && newItem.ProformaInvoiceDetails.Any())
            {
                var oldCount = oldItem.ProformaInvoiceDetails.Count;
                var newCount = newItem.ProformaInvoiceDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ProformaInvoiceDetails[i], newItem.ProformaInvoiceDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetProformaInvoiceDetailTaxesRequestChanges(oldItem.ProformaInvoiceDetails[i], newItem.ProformaInvoiceDetails[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "ProformaInvoiceDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetProformaInvoiceDetailTaxesRequestChanges(ProformaInvoiceDetailDto oldItem, ProformaInvoiceDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.ProformaInvoiceDetailTaxes.Any() && newItem.ProformaInvoiceDetailTaxes.Any())
            {
                var oldCount = oldItem.ProformaInvoiceDetailTaxes.Count;
                var newCount = newItem.ProformaInvoiceDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ProformaInvoiceDetailTaxes[i], newItem.ProformaInvoiceDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task ModifyProformaInvoiceDetailsWithStoreIdAndAvaialbleBalance(int storeId, List<ProformaInvoiceDetailDto> details)
        {
            var itemIds = details.Select(x => x.ItemId).ToList();
            var itemCurrentBalancesLookup = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, false, true).Where(x => x.StoreId == storeId && itemIds.Contains(x.ItemId)).ToDictionaryAsync(
                x => (x.ItemId, x.ItemPackageId), x => x.AvailableBalance);

            foreach (var detail in details)
            {
                detail.StoreId = storeId;
                detail.AvailableBalance = itemCurrentBalancesLookup.GetValueOrDefault((detail.ItemId, detail.ItemPackageId), 0);
            }
        }

        public async Task<ProformaInvoiceDto> GetProformaInvoice(int proformaInvoiceHeaderId)
        {
            var header = await _proformaInvoiceHeaderService.GetProformaInvoiceHeaderById(proformaInvoiceHeaderId);
            if (header == null) { return new ProformaInvoiceDto(); }

            var details = await _proformaInvoiceDetailService.GetProformaInvoiceDetails(proformaInvoiceHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ProformaInvoice, proformaInvoiceHeaderId).ToListAsync();
            var proformaInvoiceDetailTaxes = await _proformaInvoiceDetailTaxService.GetProformaInvoiceDetailTaxes(proformaInvoiceHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.ProformaInvoiceDetailTaxes = proformaInvoiceDetailTaxes.Where(x => x.ProformaInvoiceDetailId == detail.ProformaInvoiceDetailId).ToList();
            }

            await ModifyProformaInvoiceDetailsWithStoreIdAndAvaialbleBalance(header.StoreId, details);
            return new ProformaInvoiceDto() { ProformaInvoiceHeader = header, ProformaInvoiceDetails = details, MenuNotes = menuNotes };
        }

		public IQueryable<ProformaInvoiceHeaderDto> GetProformaInvoiceHeadersByStoreIdAndMenuCode(int storeId, int? clientId, int menuCode, int proformaInvoiceHeaderId)
		{
            clientId ??= 0;
            if (proformaInvoiceHeaderId != 0)
            {
                return _proformaInvoiceHeaderService.GetProformaInvoiceHeaders().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId);
            }
			else if (menuCode == MenuCodeData.StockOutFromProformaInvoice)
			{
				var hasAnyDirectInvoice = _salesInvoiceHeaderService.GetAll().Where(x => x.IsDirectInvoice).Select(x => x.ProformaInvoiceHeaderId);

				return from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetProformaInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsBlocked == false && !hasAnyDirectInvoice.Contains(x.ProformaInvoiceHeaderId))
					   from availableQuantity in _stockOutQuantityService.GetOverallQuantityAvailableFromProformaInvoices().Where(x => x.ParentId == proformaInvoiceHeader.ProformaInvoiceHeaderId && x.Quantity > 0)
					   select proformaInvoiceHeader;
			}
			else if (menuCode == MenuCodeData.SalesInvoiceInterim)
			{
				var hasAnyDirectInvoice = _salesInvoiceHeaderService.GetAll().Where(x => x.IsDirectInvoice).Select(x => x.ProformaInvoiceHeaderId);

				return from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetProformaInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsEnded == false && x.IsBlocked == false && !hasAnyDirectInvoice.Contains(x.ProformaInvoiceHeaderId))
                       from unInvoicedQuantity in _stockOutQuantityService.GetOverallUnInvoicedQuantityFromProformaInvoices().Where(x => x.ParentId == proformaInvoiceHeader.ProformaInvoiceHeaderId && x.Quantity > 0)
                       select proformaInvoiceHeader;
			}
			else
			{
				return Enumerable.Empty<ProformaInvoiceHeaderDto>().AsQueryable();
			}
		}

		public async Task<ProformaInvoiceDto> GetProformaInvoiceFromClientQuotationApproval(int clientQuotationApprovalHeaderId)
        {
            var clientQuotationApprovalHeader = await _clientQuotationApprovalHeaderService.GetClientQuotationApprovalHeaderById(clientQuotationApprovalHeaderId);
            
            if (clientQuotationApprovalHeader == null)
            {
                return new ProformaInvoiceDto();
            }

            var proformaInvoiceDetails = await GetProformaInvoiceDetailFromClientQuotationApproval(clientQuotationApprovalHeaderId, clientQuotationApprovalHeader.StoreId, clientQuotationApprovalHeader.DiscountPercent);
            var proformaInvoiceHeader = GetProformaInvoiceHeaderFromClientQuotationApproval(proformaInvoiceDetails, clientQuotationApprovalHeader);

            return new ProformaInvoiceDto { ProformaInvoiceHeader = proformaInvoiceHeader, ProformaInvoiceDetails = proformaInvoiceDetails};
        }

        private async Task<List<ProformaInvoiceDetailDto>> GetProformaInvoiceDetailFromClientQuotationApproval(int clientQuotationApprovalHeaderId, int storeId, decimal headerDiscountPercent)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var proformaInvoiceDetails =
             await (from clientQuotationApprovalDetail in _clientQuotationApprovalDetailService.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == clientQuotationApprovalHeaderId).OrderBy(x => x.ClientQuotationApprovalDetailId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == clientQuotationApprovalDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == clientQuotationApprovalDetail.CostCenterId).DefaultIfEmpty()
                    from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.StoreId == storeId).DefaultIfEmpty()
                    from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.FromPackageId == clientQuotationApprovalDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                    from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.ItemPackageId == clientQuotationApprovalDetail.ItemPackageId).OrderByDescending(x => x.SalesInvoiceDetailId).Take(1).DefaultIfEmpty()
                    select new ProformaInvoiceDetailDto
                    {
                        ProformaInvoiceDetailId = clientQuotationApprovalDetail.ClientQuotationApprovalDetailId, // <-- used to join with detail taxes
                        CostCenterId = clientQuotationApprovalDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = clientQuotationApprovalDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        ItemTypeId = item.ItemTypeId,
                        TaxTypeId = item.TaxTypeId,
                        ItemPackageId = clientQuotationApprovalDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = clientQuotationApprovalDetail.IsItemVatInclusive,
                        BarCode = clientQuotationApprovalDetail.BarCode,
                        Packing = itemPacking.Packing,
                        Quantity = clientQuotationApprovalDetail.Quantity,
                        SellingPrice = clientQuotationApprovalDetail.SellingPrice,
                        TotalValue = clientQuotationApprovalDetail.TotalValue,
                        ItemDiscountPercent = clientQuotationApprovalDetail.ItemDiscountPercent,
                        ItemDiscountValue = clientQuotationApprovalDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = clientQuotationApprovalDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = clientQuotationApprovalDetail.HeaderDiscountValue,
                        GrossValue = clientQuotationApprovalDetail.GrossValue,
                        VatPercent = clientQuotationApprovalDetail.VatPercent,
                        VatValue = clientQuotationApprovalDetail.VatValue,
                        SubNetValue = clientQuotationApprovalDetail.SubNetValue,
                        OtherTaxValue = clientQuotationApprovalDetail.OtherTaxValue,
                        NetValue = clientQuotationApprovalDetail.NetValue,
                        Notes = clientQuotationApprovalDetail.Notes,
                        ItemNote = clientQuotationApprovalDetail.ItemNote,
                        ConsumerPrice = item.ConsumerPrice,
                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                        CostValue = itemCost != null ? itemCost.CostPrice * itemPacking.Packing * clientQuotationApprovalDetail.Quantity : 0,
                        LastSalesPrice = salesInvoiceDetail != null ? salesInvoiceDetail.SellingPrice : 0
                    }).ToListAsync();

            await ModifyProformaInvoiceDetailsWithStoreIdAndAvaialbleBalance(storeId, proformaInvoiceDetails);
            await CalculateOtherTaxesAndPackages(clientQuotationApprovalHeaderId, proformaInvoiceDetails);
            SerializeProformaInvoiceDetails(proformaInvoiceDetails);

            return proformaInvoiceDetails;
        }

        private async Task CalculateOtherTaxesAndPackages(int clientQuotationApprovalHeaderId, List<ProformaInvoiceDetailDto> proformaInvoiceDetails)
        {
            var itemIds = proformaInvoiceDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var clientQuotationApprovalDetailTaxes = await _clientQuotationApprovalDetailTaxService.GetClientQuotationApprovalDetailTaxes(clientQuotationApprovalHeaderId).ToListAsync();

            foreach(var proformaInvoiceDetail in proformaInvoiceDetails)
            {
                proformaInvoiceDetail.Packages = packages.Where(x => x.ItemId == proformaInvoiceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                proformaInvoiceDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == proformaInvoiceDetail.ItemId).ToList();
                proformaInvoiceDetail.Taxes = proformaInvoiceDetail.ItemTaxData.ToJson();

                proformaInvoiceDetail.ProformaInvoiceDetailTaxes = (
                        from itemTax in clientQuotationApprovalDetailTaxes.Where(x => x.ClientQuotationApprovalDetailId == proformaInvoiceDetail.ProformaInvoiceDetailId)
                        select new ProformaInvoiceDetailTaxDto
                        {
                            TaxId = itemTax.TaxId,
                            CreditAccountId = itemTax.CreditAccountId,
                            TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                            TaxPercent = itemTax.TaxPercent,
                            TaxValue = itemTax.TaxValue
                        }
                    ).ToList();
            }
        }

        private void SerializeProformaInvoiceDetails(List<ProformaInvoiceDetailDto> proformaInvoiceDetails)
        {
            int newId = -1;
            int newSubId = -1;
            foreach(var proformaInvoiceDetail in proformaInvoiceDetails)
            {
                proformaInvoiceDetail.ProformaInvoiceDetailId = newId;

                proformaInvoiceDetail.ProformaInvoiceDetailTaxes.ForEach(y =>
                {
                    y.ProformaInvoiceDetailId = newId;
                    y.ProformaInvoiceDetailTaxId = newSubId--;
                });

                newId--;
            }
        }

        private ProformaInvoiceHeaderDto GetProformaInvoiceHeaderFromClientQuotationApproval(List<ProformaInvoiceDetailDto> proformaInvoiceDetails, ClientQuotationApprovalHeaderDto clientQuotationApprovalHeader)
        {
            var totalCostValueFromDetail = proformaInvoiceDetails.Sum(x => x.CostValue);

            var proformaInvoiceHeader = new ProformaInvoiceHeaderDto
            {
                ProformaInvoiceHeaderId = 0,
                ClientQuotationApprovalHeaderId = clientQuotationApprovalHeader.ClientQuotationApprovalHeaderId,
                DocumentDate = clientQuotationApprovalHeader.DocumentDate,
                ClientId = clientQuotationApprovalHeader.ClientId,
                ClientCode = clientQuotationApprovalHeader.ClientCode,
                ClientName = clientQuotationApprovalHeader.ClientName,
                SellerId = clientQuotationApprovalHeader.SellerId,
                SellerCode = clientQuotationApprovalHeader.SellerCode,
                SellerName = clientQuotationApprovalHeader.SellerName,
                StoreId = clientQuotationApprovalHeader.StoreId,
                StoreName = clientQuotationApprovalHeader.StoreName,
                Reference = clientQuotationApprovalHeader.Reference,
                CreditPayment = false,
                TaxTypeId = TaxTypeData.Taxable,
                TotalValue = clientQuotationApprovalHeader.TotalValue,
                DiscountPercent = clientQuotationApprovalHeader.DiscountPercent,
                DiscountValue = clientQuotationApprovalHeader.DiscountValue,
                TotalItemDiscount = clientQuotationApprovalHeader.TotalItemDiscount,
                GrossValue = clientQuotationApprovalHeader.GrossValue,
                VatValue = clientQuotationApprovalHeader.VatValue,
                SubNetValue = clientQuotationApprovalHeader.SubNetValue,
                OtherTaxValue = clientQuotationApprovalHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = clientQuotationApprovalHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = clientQuotationApprovalHeader.AdditionalDiscountValue,
                NetValue = clientQuotationApprovalHeader.NetValue,
                TotalCostValue = totalCostValueFromDetail,
                RemarksAr = clientQuotationApprovalHeader.RemarksAr,
                RemarksEn = clientQuotationApprovalHeader.RemarksEn,
                IsClosed = false,
                IsCancelled = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = clientQuotationApprovalHeader.ArchiveHeaderId,
            };

            return proformaInvoiceHeader;
        }

        public async Task<ResponseDto> UpdateBlocked(int proformaInvoiceHeaderId, bool isBlocked)
        {
            await _stockOutHeaderService.UpdateAllStockOutsBlockedFromProformaInvoice(proformaInvoiceHeaderId, isBlocked);
            await _stockOutReturnHeaderService.UpdateAllStockOutReturnsBlockedFromProformaInvoice(proformaInvoiceHeaderId, isBlocked);
            await _proformaInvoiceHeaderService.UpdateBlocked(proformaInvoiceHeaderId, isBlocked);
            await _salesInvoiceHeaderService.UpdateAllSalesInvoicesBlockedFromProformaInvoice(proformaInvoiceHeaderId, isBlocked);
            await _salesInvoiceReturnHeaderService.UpdateAllSalesInvoiceReturnsBlockedFromProformaInvoice(proformaInvoiceHeaderId, isBlocked);

            return new ResponseDto { Id = proformaInvoiceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, isBlocked ? GenericMessageData.StoppedDealingOnSuccessfully : GenericMessageData.OpenedToDealingOnSuccessfully) };
        }

        public async Task<ResponseDto> SaveProformaInvoice(ProformaInvoiceDto proformaInvoice, bool hasApprove, bool approved, int? requestId, bool shouldValidate = true, string? documentReference = null, int documentStatusId = DocumentStatusData.ProformaInvoiceCreated, bool shouldInitializeFlags = false)
        {
            if (proformaInvoice.ProformaInvoiceHeader != null)
            {
                if (shouldValidate)
                {
                    var validationResult = await CheckProformaInvoiceIsValidForSave(proformaInvoice);
                    if (validationResult.Success == false) return validationResult;
                }

                if (proformaInvoice.ProformaInvoiceHeader.ProformaInvoiceHeaderId == 0)
                {
                    await UpdateModelPrices(proformaInvoice);
                }

                var result = await _proformaInvoiceHeaderService.SaveProformaInvoiceHeader(proformaInvoice.ProformaInvoiceHeader, hasApprove, approved, requestId, shouldValidate, documentReference, documentStatusId, shouldInitializeFlags);
                if (result.Success)
                {
                    var modifiedProformaInvoiceDetails = await _proformaInvoiceDetailService.SaveProformaInvoiceDetails(result.Id, proformaInvoice.ProformaInvoiceDetails);
                    await SaveProformaInvoiceDetailTaxes(result.Id, modifiedProformaInvoiceDetails);
                    await _proformaInvoiceDetailService.DeleteProformaInvoiceDetailList(modifiedProformaInvoiceDetails, result.Id);
                    
                    if (proformaInvoice.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(proformaInvoice.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        public async Task<ResponseDto> CheckProformaInvoiceIsValidForSave(ProformaInvoiceDto proformaInvoice)
        {
			if (!proformaInvoice.ProformaInvoiceDetails.Any()) return new ResponseDto { Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.DetailIsEmpty) };

			var zeroStockResult = await CheckProformaInvoiceZeroStock(proformaInvoice.ProformaInvoiceHeader!.StoreId, proformaInvoice.ProformaInvoiceDetails);
			if (zeroStockResult.Success == false) return zeroStockResult;

			var itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(proformaInvoice.ProformaInvoiceDetails, x => x.ItemId, x => x.ItemNote);
			if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

			return new ResponseDto { Success = true };
		}

        //no need to do zero stock validation when deleting proforma
        private async Task<ResponseDto> CheckProformaInvoiceZeroStock(int storeId, List<ProformaInvoiceDetailDto> proformaDetails)
        {
            return await _zeroStockValidationService.ValidateZeroStock(
                    storeId: storeId,
                    newDetails: proformaDetails,
                    oldDetails: [], //proforma invoice does not affect the actual available balance
                    detailKeySelector: x => (x.ItemId, x.ItemPackageId),
                    itemIdSelector: x => x.ItemId,
                    quantitySelector: x => x.Quantity + x.BonusQuantity,
                    availableBalanceKeySelector: x => (x.ItemId, x.ItemPackageId),
                    isGrouped: true,
                    menuCode: MenuCodeData.ProformaInvoice,
                    settingMenuCode: MenuCodeData.ProformaInvoice,
                    isSave: true
                );
        }

        private async Task UpdateModelPrices(ProformaInvoiceDto proformaInvoice)
        {
            await UpdateDetailPrices(proformaInvoice.ProformaInvoiceDetails, proformaInvoice.ProformaInvoiceHeader!.StoreId);
            proformaInvoice.ProformaInvoiceHeader!.TotalCostValue = proformaInvoice.ProformaInvoiceDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<ProformaInvoiceDetailDto> proformaInvoiceDetails, int storeId)
        {
            var itemIds = proformaInvoiceDetails.Select(x => x.ItemId).ToList();

            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId) && x.StoreId == storeId).Select(x => new { x.StoreId, x.ItemId, x.CostPrice }).ToListAsync();
            var consumerPrices = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).Select(x => new { x.ItemId, x.ConsumerPrice }).ToListAsync();
            var lastSellingPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds);

            var packings = await (
                        from item in _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId))
                        from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == item.ItemId && x.ToPackageId == item.SingularPackageId)
                        select new
                        {
                            item.ItemId,
                            itemPacking.FromPackageId,
                            itemPacking.Packing
                        }
                    ).ToListAsync();

            foreach (var proformaInvoiceDetail in proformaInvoiceDetails)
            {
                var packing = packings.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.FromPackageId == proformaInvoiceDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                proformaInvoiceDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == proformaInvoiceDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                proformaInvoiceDetail.CostPrice = itemCosts.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                proformaInvoiceDetail.CostPackage = proformaInvoiceDetail.CostPrice * packing;
                proformaInvoiceDetail.CostValue = proformaInvoiceDetail.CostPackage * (proformaInvoiceDetail.Quantity + proformaInvoiceDetail.BonusQuantity);
                proformaInvoiceDetail.LastSalesPrice = lastSellingPrices.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.ItemPackageId == proformaInvoiceDetail.ItemPackageId).Select(x => x.SellingPrice).FirstOrDefault(0);
            }
        }

        private async Task SaveProformaInvoiceDetailTaxes(int proformaInvoiceHeaderId, List<ProformaInvoiceDetailDto> proformaInvoiceDetails)
        {
            List<ProformaInvoiceDetailTaxDto> proformaInvoiceDetailTaxes = new List<ProformaInvoiceDetailTaxDto>();

            foreach (var proformaInvoiceDetail in proformaInvoiceDetails)
            {
                foreach (var proformaInvoiceDetailTax in proformaInvoiceDetail.ProformaInvoiceDetailTaxes)
                {
                    proformaInvoiceDetailTax.ProformaInvoiceDetailId = proformaInvoiceDetail.ProformaInvoiceDetailId;
                    proformaInvoiceDetailTaxes.Add(proformaInvoiceDetailTax);
                }
            }

            await _proformaInvoiceDetailTaxService.SaveProformaInvoiceDetailTaxes(proformaInvoiceHeaderId, proformaInvoiceDetailTaxes);
        }

        public async Task<ResponseDto> DeleteProformaInvoice(int proformaInvoiceHeaderId)
        {
            var validationResult = await CheckProformaInvoiceIsValidForDelete(proformaInvoiceHeaderId);
            if (validationResult.Success == false) return validationResult;

            await _menuNoteService.DeleteMenuNotes(MenuCodeData.ProformaInvoice, proformaInvoiceHeaderId);
            await _proformaInvoiceDetailTaxService.DeleteProformaInvoiceDetailTaxes(proformaInvoiceHeaderId);
            await _proformaInvoiceDetailService.DeleteProformaInvoiceDetails(proformaInvoiceHeaderId);
            var result = await _proformaInvoiceHeaderService.DeleteProformaInvoiceHeader(proformaInvoiceHeaderId);
            return result;
        }

        public async Task<ResponseDto> CheckProformaInvoiceIsValidForDelete(int proformaInvoiceHeaderId)
        {
            var proformaInvoiceHeader = await _proformaInvoiceHeaderService.GetProformaInvoiceHeaderById(proformaInvoiceHeaderId);

			if (proformaInvoiceHeader!.IsBlocked)
			{
				return new ResponseDto() { Id = proformaInvoiceHeader.ProformaInvoiceHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			return new ResponseDto { Success = true };
        }
    }
}
