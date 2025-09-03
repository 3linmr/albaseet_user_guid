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
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Menus;

namespace Sales.Service.Services
{
    public class ClientQuotationApprovalService : IClientQuotationApprovalService
    {
        private readonly IClientQuotationApprovalHeaderService _clientQuotationApprovalHeaderService;
        private readonly IClientQuotationApprovalDetailService _clientQuotationApprovalDetailService;
        private readonly IClientQuotationHeaderService _clientQuotationHeaderService;
        private readonly IClientQuotationDetailService _clientQuotationDetailService;
        private readonly IClientQuotationDetailTaxService _clientQuotationDetailTaxService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IClientQuotationApprovalDetailTaxService _clientQuotationApprovalDetailTaxService;
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
        private readonly IGenericMessageService _genericMessageService;
        private readonly ISalesMessageService _salesMessageService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

        public ClientQuotationApprovalService(IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IClientQuotationApprovalDetailService clientQuotationApprovalDetailService, IClientQuotationHeaderService clientQuotationHeaderService, IClientQuotationDetailService clientQuotationDetailService, IClientQuotationDetailTaxService clientQuotationDetailTaxService, IMenuNoteService menuNoteService, IClientQuotationApprovalDetailTaxService clientQuotationApprovalDetailTaxService, IHttpContextAccessor httpContextAccessor, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, IItemCostService itemCostService, IItemPackingService itemPackingService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceService salesInvoiceService, ITaxPercentService taxPercentService, ITaxService taxService, IGenericMessageService genericMessageService, ISalesMessageService salesMessageService, IItemNoteValidationService itemNoteValidationService)
        {
            _clientQuotationApprovalHeaderService = clientQuotationApprovalHeaderService;
            _clientQuotationApprovalDetailService = clientQuotationApprovalDetailService;
            _clientQuotationDetailService = clientQuotationDetailService;
            _clientQuotationDetailTaxService = clientQuotationDetailTaxService;
            _clientQuotationHeaderService = clientQuotationHeaderService;
            _menuNoteService = menuNoteService;
            _clientQuotationApprovalDetailTaxService = clientQuotationApprovalDetailTaxService;
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
            _genericMessageService = genericMessageService;
            _salesMessageService = salesMessageService;
            _itemNoteValidationService = itemNoteValidationService;
        }
        public List<RequestChangesDto> GetClientQuotationApprovalRequestChanges(ClientQuotationApprovalDto oldItem, ClientQuotationApprovalDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.ClientQuotationApprovalHeader, newItem.ClientQuotationApprovalHeader);
            requestChanges.AddRange(items);

            if (oldItem.ClientQuotationApprovalDetails.Any() && newItem.ClientQuotationApprovalDetails.Any())
            {
                var oldCount = oldItem.ClientQuotationApprovalDetails.Count;
                var newCount = newItem.ClientQuotationApprovalDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ClientQuotationApprovalDetails[i], newItem.ClientQuotationApprovalDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetClientQuotationApprovalDetailTaxesRequestChanges(oldItem.ClientQuotationApprovalDetails[i], newItem.ClientQuotationApprovalDetails[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "ClientQuotationApprovalDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetClientQuotationApprovalDetailTaxesRequestChanges(ClientQuotationApprovalDetailDto oldItem, ClientQuotationApprovalDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.ClientQuotationApprovalDetailTaxes.Any() && newItem.ClientQuotationApprovalDetailTaxes.Any())
            {
                var oldCount = oldItem.ClientQuotationApprovalDetailTaxes.Count;
                var newCount = newItem.ClientQuotationApprovalDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ClientQuotationApprovalDetailTaxes[i], newItem.ClientQuotationApprovalDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<ClientQuotationApprovalDto> GetClientQuotationApproval(int clientQuotationApprovalHeaderId)
        {
            var header = await _clientQuotationApprovalHeaderService.GetClientQuotationApprovalHeaderById(clientQuotationApprovalHeaderId);
            var details = await _clientQuotationApprovalDetailService.GetClientQuotationApprovalDetails(clientQuotationApprovalHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ClientQuotationApproval, clientQuotationApprovalHeaderId).ToListAsync();
            var clientQuotationApprovalDetailTaxes = await _clientQuotationApprovalDetailTaxService.GetClientQuotationApprovalDetailTaxes(clientQuotationApprovalHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.ClientQuotationApprovalDetailTaxes = clientQuotationApprovalDetailTaxes.Where(x => x.ClientQuotationApprovalDetailId == detail.ClientQuotationApprovalDetailId).ToList();
            }

            return new ClientQuotationApprovalDto() { ClientQuotationApprovalHeader = header, ClientQuotationApprovalDetails = details, MenuNotes = menuNotes };
        }

        public async Task<ClientQuotationApprovalDto> GetClientQuotationApprovalFromClientQuotation(int clientQuotationHeaderId)
        {
            var clientQuotationHeader = await _clientQuotationHeaderService.GetClientQuotationHeaderById(clientQuotationHeaderId);
            
            if (clientQuotationHeader == null)
            {
                return new ClientQuotationApprovalDto();
            }

            var clientQuotationApprovalDetails = await GetClientQuotationApprovalDetailFromClientQuotation(clientQuotationHeaderId, clientQuotationHeader.StoreId);
            var clientQuotationApprovalHeader = GetClientQuotationApprovalHeaderFromClientQuotation(clientQuotationHeader, clientQuotationApprovalDetails);

            return new ClientQuotationApprovalDto { ClientQuotationApprovalHeader = clientQuotationApprovalHeader, ClientQuotationApprovalDetails = clientQuotationApprovalDetails};
        }

        private async Task<List<ClientQuotationApprovalDetailDto>> GetClientQuotationApprovalDetailFromClientQuotation(int clientQuotationHeaderId, int storeId)
        {
            decimal vatPercent = await _taxPercentService.GetVatTaxByStoreId(storeId, DateHelper.GetDateTimeNow());
            bool isVatInclusive = await _itemService.IsItemsVatInclusive(storeId);

            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var clientQuotationApprovalDetails =
             await (from clientQuotationDetail in _clientQuotationDetailService.GetAll().Where(x => x.ClientQuotationHeaderId == clientQuotationHeaderId).OrderBy(x => x.ClientQuotationDetailId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == clientQuotationDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == clientQuotationDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == clientQuotationDetail.CostCenterId).DefaultIfEmpty()
                    from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == clientQuotationDetail.ItemId && x.StoreId == storeId).DefaultIfEmpty()
                    from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == clientQuotationDetail.ItemId && x.FromPackageId == clientQuotationDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                    from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.ItemId == clientQuotationDetail.ItemId && x.ItemPackageId == clientQuotationDetail.ItemPackageId).OrderByDescending(x => x.SalesInvoiceDetailId).Take(1).DefaultIfEmpty()
                    select new ClientQuotationApprovalDetailDto
                    {
                        ClientQuotationApprovalDetailId = clientQuotationDetail.ClientQuotationDetailId,  // <-- used to join with detail taxes
						CostCenterId = clientQuotationDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = clientQuotationDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = clientQuotationDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = clientQuotationDetail.IsItemVatInclusive,
                        BarCode = clientQuotationDetail.BarCode,
                        Packing = itemPacking.Packing,
                        Quantity = clientQuotationDetail.Quantity,
                        SellingPrice = clientQuotationDetail.SellingPrice,
                        TotalValue = clientQuotationDetail.TotalValue,
                        ItemDiscountPercent = clientQuotationDetail.ItemDiscountPercent,
                        ItemDiscountValue = clientQuotationDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = clientQuotationDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = clientQuotationDetail.HeaderDiscountValue,
                        GrossValue = clientQuotationDetail.GrossValue,
                        VatPercent = clientQuotationDetail.VatPercent,
                        VatValue = clientQuotationDetail.VatValue,
                        SubNetValue = clientQuotationDetail.SubNetValue,
                        OtherTaxValue = clientQuotationDetail.OtherTaxValue,
                        NetValue = clientQuotationDetail.NetValue,
                        Notes = clientQuotationDetail.Notes,
                        ItemNote = clientQuotationDetail.ItemNote,
                        ConsumerPrice = item.ConsumerPrice,
                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                        CostValue = itemCost != null ? itemCost.CostPrice * itemPacking.Packing * clientQuotationDetail.Quantity : 0,
                        LastSalesPrice = salesInvoiceDetail != null ? salesInvoiceDetail.SellingPrice : 0
                    }).ToListAsync();


            var itemIds = clientQuotationApprovalDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
			var clientQuotationDetailTaxes = await _clientQuotationDetailTaxService.GetClientQuotationDetailTaxes(clientQuotationHeaderId).ToListAsync();

			int newId = -1;
            int newSubId = -1;
            foreach (var clientQuotationApprovalDetail in clientQuotationApprovalDetails)
            {

                clientQuotationApprovalDetail.Packages = packages.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                clientQuotationApprovalDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId).ToList();
                clientQuotationApprovalDetail.Taxes = clientQuotationApprovalDetail.ItemTaxData.ToJson();

                clientQuotationApprovalDetail.ClientQuotationApprovalDetailTaxes = (
                        from itemTax in clientQuotationDetailTaxes.Where(x => x.ClientQuotationDetailId == clientQuotationApprovalDetail.ClientQuotationApprovalDetailId)
						select new ClientQuotationApprovalDetailTaxDto
                        {
                            TaxId = itemTax.TaxId,
                            CreditAccountId = (int)itemTax.CreditAccountId!,
                            TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                            TaxPercent = itemTax.TaxPercent,
                            TaxValue = itemTax.TaxValue
                        }
                    ).ToList();

                clientQuotationApprovalDetail.ClientQuotationApprovalDetailId = newId;
                clientQuotationApprovalDetail.ClientQuotationApprovalDetailTaxes.ForEach(y =>
                {
                    y.ClientQuotationApprovalDetailId = newId;
                    y.ClientQuotationApprovalDetailTaxId = newSubId--;
                });
                newId--;
            }

            return clientQuotationApprovalDetails;
        }

        private ClientQuotationApprovalHeaderDto GetClientQuotationApprovalHeaderFromClientQuotation(ClientQuotationHeaderDto clientQuotationHeader, List<ClientQuotationApprovalDetailDto> clientQuotationApprovalDetails)
        {
            var totalCostValueFromDetail = clientQuotationApprovalDetails.Sum(x => x.CostValue);

            var clientQuotationApprovalHeader = new ClientQuotationApprovalHeaderDto
            {
                ClientQuotationApprovalHeaderId = 0,
                ClientQuotationHeaderId = clientQuotationHeader.ClientQuotationHeaderId,
                ClientId = clientQuotationHeader.ClientId,
                ClientCode = clientQuotationHeader.ClientCode,
                ClientName = clientQuotationHeader.ClientName,
                SellerId = clientQuotationHeader.SellerId,
                SellerCode = clientQuotationHeader.SellerCode,
                SellerName = clientQuotationHeader.SellerName,
                StoreId = clientQuotationHeader.StoreId,
                StoreName = clientQuotationHeader.StoreName,
                TaxTypeId = clientQuotationHeader.TaxTypeId,
                DocumentDate = clientQuotationHeader.DocumentDate,
                Reference = clientQuotationHeader.Reference,
                TotalValue = clientQuotationHeader.TotalValue,
                DiscountPercent = clientQuotationHeader.DiscountPercent,
                DiscountValue = clientQuotationHeader.DiscountValue,
                TotalItemDiscount = clientQuotationHeader.TotalItemDiscount,
                GrossValue = clientQuotationHeader.GrossValue,
                VatValue = clientQuotationHeader.VatValue,
                SubNetValue = clientQuotationHeader.SubNetValue,
                OtherTaxValue = clientQuotationHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = clientQuotationHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = clientQuotationHeader.AdditionalDiscountValue,
                NetValue = clientQuotationHeader.NetValue,
                TotalCostValue = totalCostValueFromDetail,
                RemarksAr = clientQuotationHeader.RemarksAr,
                RemarksEn = clientQuotationHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = clientQuotationHeader.ArchiveHeaderId,
            };

            return clientQuotationApprovalHeader;
        }

        public async Task<ResponseDto> SaveClientQuotationApproval(ClientQuotationApprovalDto clientQuotationApproval, bool hasApprove, bool approved, int? requestId)
        {
            if (clientQuotationApproval.ClientQuotationApprovalHeader != null)
			{
				var itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(clientQuotationApproval.ClientQuotationApprovalDetails, x => x.ItemId, x => x.ItemNote);
				if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

				if (!clientQuotationApproval.ClientQuotationApprovalDetails.Any()) return new ResponseDto { Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.DetailIsEmpty) };

                var clientQuotationStillValid = await CheckClientQuotationIsStillValid(clientQuotationApproval.ClientQuotationApprovalHeader.ClientQuotationHeaderId, clientQuotationApproval.ClientQuotationApprovalHeader.ClientQuotationApprovalHeaderId == 0);
                if (clientQuotationStillValid.Success == false) return clientQuotationStillValid;

                if (clientQuotationApproval.ClientQuotationApprovalHeader.ClientQuotationApprovalHeaderId == 0)
                {
                    await UpdateModelPrices(clientQuotationApproval);
                }

                var result = await _clientQuotationApprovalHeaderService.SaveClientQuotationApprovalHeader(clientQuotationApproval.ClientQuotationApprovalHeader, hasApprove, approved, requestId);
                if (result.Success)
                {
                    var modifiedClientQuotationApprovalDetails = await _clientQuotationApprovalDetailService.SaveClientQuotationApprovalDetails(result.Id, clientQuotationApproval.ClientQuotationApprovalDetails);
                    await SaveClientQuotationApprovalDetailTaxes(result.Id, modifiedClientQuotationApprovalDetails);
                    await _clientQuotationApprovalDetailService.DeleteClientQuotationApprovalDetailList(modifiedClientQuotationApprovalDetails, result.Id);
                    
                    if (clientQuotationApproval.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(clientQuotationApproval.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        private async Task<ResponseDto> CheckClientQuotationIsStillValid(int? clientQuotationHeaderId, bool isCreate)
        {
            if (clientQuotationHeaderId != null && isCreate)
            {
                var validUntil = (await _clientQuotationHeaderService.GetClientQuotationHeaderById((int)clientQuotationHeaderId))?.ValidUntilDate;
                if (validUntil == null) { return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.NotFound) }; }

                if (((DateTime)validUntil).Date < DateHelper.GetDateTimeNow().Date)
                {
                    return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(MenuCodeData.ClientQuotation, SalesMessageData.NoLongerValid) };
                }
            }
            return new ResponseDto { Success = true };
        }

        private async Task UpdateModelPrices(ClientQuotationApprovalDto clientQuotationApproval)
        {
            await UpdateDetailPrices(clientQuotationApproval.ClientQuotationApprovalDetails, clientQuotationApproval.ClientQuotationApprovalHeader!.StoreId);
            clientQuotationApproval.ClientQuotationApprovalHeader!.TotalCostValue = clientQuotationApproval.ClientQuotationApprovalDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<ClientQuotationApprovalDetailDto> clientQuotationApprovalDetails, int storeId)
        {
            var itemIds = clientQuotationApprovalDetails.Select(x => x.ItemId).ToList();

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

            foreach (var clientQuotationApprovalDetail in clientQuotationApprovalDetails)
            {
                var packing = packings.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.FromPackageId == clientQuotationApprovalDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                clientQuotationApprovalDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                clientQuotationApprovalDetail.CostPrice = itemCosts.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                clientQuotationApprovalDetail.CostPackage = clientQuotationApprovalDetail.CostPrice * packing;
                clientQuotationApprovalDetail.CostValue = clientQuotationApprovalDetail.CostPackage * clientQuotationApprovalDetail.Quantity;
                clientQuotationApprovalDetail.LastSalesPrice = lastSellingPrices.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.ItemPackageId == clientQuotationApprovalDetail.ItemPackageId).Select(x => x.SellingPrice).FirstOrDefault(0);
            }
        }

        private async Task SaveClientQuotationApprovalDetailTaxes(int clientQuotationApprovalHeaderId, List<ClientQuotationApprovalDetailDto> clientQuotationApprovalDetails)
        {
            List<ClientQuotationApprovalDetailTaxDto> clientQuotationApprovalDetailTaxes = new List<ClientQuotationApprovalDetailTaxDto>();

            foreach (var clientQuotationApprovalDetail in clientQuotationApprovalDetails)
            {
                foreach (var clientQuotationApprovalDetailTax in clientQuotationApprovalDetail.ClientQuotationApprovalDetailTaxes)
                {
                    clientQuotationApprovalDetailTax.ClientQuotationApprovalDetailId = clientQuotationApprovalDetail.ClientQuotationApprovalDetailId;
                    clientQuotationApprovalDetailTaxes.Add(clientQuotationApprovalDetailTax);
                }
            }

            await _clientQuotationApprovalDetailTaxService.SaveClientQuotationApprovalDetailTaxes(clientQuotationApprovalHeaderId, clientQuotationApprovalDetailTaxes);
        }

        public async Task<ResponseDto> DeleteClientQuotationApproval(int clientQuotationApprovalHeaderId)
        {
            await _menuNoteService.DeleteMenuNotes(MenuCodeData.ClientQuotationApproval, clientQuotationApprovalHeaderId);
            await _clientQuotationApprovalDetailTaxService.DeleteClientQuotationApprovalDetailTaxes(clientQuotationApprovalHeaderId);
            await _clientQuotationApprovalDetailService.DeleteClientQuotationApprovalDetails(clientQuotationApprovalHeaderId);
            var result = await _clientQuotationApprovalHeaderService.DeleteClientQuotationApprovalHeader(clientQuotationApprovalHeaderId);
            return result;
        }
    }
}
