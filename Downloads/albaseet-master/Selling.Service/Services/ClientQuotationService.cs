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
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
    public class ClientQuotationService : IClientQuotationService
    {
        private readonly IClientQuotationHeaderService _clientQuotationHeaderService;
        private readonly IClientQuotationDetailService _clientQuotationDetailService;
        private readonly IClientPriceRequestHeaderService _clientPriceRequestHeaderService;
        private readonly IClientPriceRequestDetailService _clientPriceRequestDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IClientQuotationDetailTaxService _clientQuotationDetailTaxService;
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
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

        public ClientQuotationService(IClientQuotationHeaderService clientQuotationHeaderService, IClientQuotationDetailService clientQuotationDetailService, IClientPriceRequestHeaderService clientPriceRequestHeaderService, IClientPriceRequestDetailService clientPriceRequestDetailService, IMenuNoteService menuNoteService, IClientQuotationDetailTaxService clientQuotationDetailTaxService, IHttpContextAccessor httpContextAccessor, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, IItemCostService itemCostService, IItemPackingService itemPackingService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceService salesInvoiceService, ITaxPercentService taxPercentService, ITaxService taxService, IGenericMessageService genericMessageService, IApplicationSettingService applicationSettingService, IItemNoteValidationService itemNoteValidationService)
        {
            _clientQuotationHeaderService = clientQuotationHeaderService;
            _clientQuotationDetailService = clientQuotationDetailService;
            _clientPriceRequestDetailService = clientPriceRequestDetailService;
            _clientPriceRequestHeaderService = clientPriceRequestHeaderService;
            _menuNoteService = menuNoteService;
            _clientQuotationDetailTaxService = clientQuotationDetailTaxService;
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
            _applicationSettingService = applicationSettingService;
            _itemNoteValidationService = itemNoteValidationService;
        }
        public List<RequestChangesDto> GetClientQuotationRequestChanges(ClientQuotationDto oldItem, ClientQuotationDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.ClientQuotationHeader, newItem.ClientQuotationHeader);
            requestChanges.AddRange(items);

            if (oldItem.ClientQuotationDetails.Any() && newItem.ClientQuotationDetails.Any())
            {
                var oldCount = oldItem.ClientQuotationDetails.Count;
                var newCount = newItem.ClientQuotationDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ClientQuotationDetails[i], newItem.ClientQuotationDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetClientQuotationDetailTaxesRequestChanges(oldItem.ClientQuotationDetails[i], newItem.ClientQuotationDetails[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "ClientQuotationDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetClientQuotationDetailTaxesRequestChanges(ClientQuotationDetailDto oldItem, ClientQuotationDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.ClientQuotationDetailTaxes.Any() && newItem.ClientQuotationDetailTaxes.Any())
            {
                var oldCount = oldItem.ClientQuotationDetailTaxes.Count;
                var newCount = newItem.ClientQuotationDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ClientQuotationDetailTaxes[i], newItem.ClientQuotationDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<ClientQuotationDto> GetClientQuotation(int clientQuotationHeaderId)
        {
            var header = await _clientQuotationHeaderService.GetClientQuotationHeaderById(clientQuotationHeaderId);
            var details = await _clientQuotationDetailService.GetClientQuotationDetails(clientQuotationHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ClientQuotation, clientQuotationHeaderId).ToListAsync();
            var clientQuotationDetailTaxes = await _clientQuotationDetailTaxService.GetClientQuotationDetailTaxes(clientQuotationHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.ClientQuotationDetailTaxes = clientQuotationDetailTaxes.Where(x => x.ClientQuotationDetailId == detail.ClientQuotationDetailId).ToList();
            }

            return new ClientQuotationDto() { ClientQuotationHeader = header, ClientQuotationDetails = details, MenuNotes = menuNotes };
        }

        public async Task<ClientQuotationDto> GetClientQuotationFromClientPriceRequest(int clientPriceRequestHeaderId)
        {
            var clientPriceRequestHeader = await _clientPriceRequestHeaderService.GetClientPriceRequestHeaderById(clientPriceRequestHeaderId);
            
            if (clientPriceRequestHeader == null)
            {
                return new ClientQuotationDto();
            }

            var clientQuotationDetails = await GetClientQuotationDetailFromClientPriceRequest(clientPriceRequestHeaderId, clientPriceRequestHeader.StoreId);
            var clientQuotationHeader = GetClientQuotationHeaderFromClientPriceRequest(clientPriceRequestHeader, clientQuotationDetails);

            return new ClientQuotationDto { ClientQuotationHeader = clientQuotationHeader, ClientQuotationDetails = clientQuotationDetails};
        }

        private async Task<List<ClientQuotationDetailDto>> GetClientQuotationDetailFromClientPriceRequest(int clientPriceRequestHeaderId, int storeId)
        {
            decimal vatPercent = await _taxPercentService.GetVatTaxByStoreId(storeId, DateHelper.GetDateTimeNow());
            bool isVatInclusive = await _itemService.IsItemsVatInclusive(storeId);

            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var clientQuotationDetails =
             await (from clientPriceRequestDetail in _clientPriceRequestDetailService.GetAll().Where(x => x.ClientPriceRequestHeaderId == clientPriceRequestHeaderId).OrderBy(x => x.ClientPriceRequestDetailId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == clientPriceRequestDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == clientPriceRequestDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == clientPriceRequestDetail.CostCenterId).DefaultIfEmpty()
                    from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == clientPriceRequestDetail.ItemId && x.StoreId == storeId).DefaultIfEmpty()
                    from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == clientPriceRequestDetail.ItemId && x.FromPackageId == clientPriceRequestDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                    from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.ItemId == clientPriceRequestDetail.ItemId && x.ItemPackageId == clientPriceRequestDetail.ItemPackageId).OrderByDescending(x => x.SalesInvoiceDetailId).Take(1).DefaultIfEmpty()
                    select new ClientQuotationDetailDto
                    {
                        CostCenterId = clientPriceRequestDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = clientPriceRequestDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = clientPriceRequestDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = isVatInclusive,
                        BarCode = clientPriceRequestDetail.BarCode,
                        Packing = itemPacking.Packing,
                        Quantity = clientPriceRequestDetail.Quantity,
                        SellingPrice = clientPriceRequestDetail.ConsumerPrice,
                        TotalValue = CalculateDetailValue.TotalValue(clientPriceRequestDetail.Quantity, clientPriceRequestDetail.ConsumerPrice, isVatInclusive, item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0),
                        ItemDiscountPercent = 0,
                        ItemDiscountValue = 0,
                        TotalValueAfterDiscount = CalculateDetailValue.TotalValueAfterDiscount(clientPriceRequestDetail.Quantity, clientPriceRequestDetail.ConsumerPrice, 0, isVatInclusive, item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0),
                        HeaderDiscountValue = 0,
                        GrossValue = CalculateDetailValue.GrossValue(clientPriceRequestDetail.Quantity, clientPriceRequestDetail.ConsumerPrice, 0, 0, isVatInclusive, item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0),
                        VatPercent = item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0,
                        VatValue = CalculateDetailValue.VatValue(clientPriceRequestDetail.Quantity, clientPriceRequestDetail.ConsumerPrice, 0, item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0, 0, isVatInclusive),
                        SubNetValue = CalculateDetailValue.SubNetValue(clientPriceRequestDetail.Quantity, clientPriceRequestDetail.ConsumerPrice, 0, item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0, 0, isVatInclusive),
                        Notes = clientPriceRequestDetail.Notes,
                        ItemNote = clientPriceRequestDetail.ItemNote,
                        ConsumerPrice = item.ConsumerPrice,
                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                        CostValue = itemCost != null ? itemCost.CostPrice * itemPacking.Packing * clientPriceRequestDetail.Quantity : 0,
                        LastSalesPrice = salesInvoiceDetail != null ? salesInvoiceDetail.SellingPrice : 0
                    }).ToListAsync();


            var itemIds = clientQuotationDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            int newId = -1;
            int newSubId = -1;
            foreach (var clientQuotationDetail in clientQuotationDetails)
            {

                clientQuotationDetail.Packages = packages.Where(x => x.ItemId == clientQuotationDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                clientQuotationDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == clientQuotationDetail.ItemId).ToList();
                clientQuotationDetail.Taxes = clientQuotationDetail.ItemTaxData.ToJson();

                clientQuotationDetail.ClientQuotationDetailTaxes = (
                        from itemTax in clientQuotationDetail.ItemTaxData
                        select new ClientQuotationDetailTaxDto
                        {
                            TaxId = itemTax.TaxId,
                            CreditAccountId = (int)itemTax.CreditAccountId!,
                            TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                            TaxPercent = clientQuotationDetail.TaxTypeId == TaxTypeData.Taxable ? itemTax.TaxPercent : 0,
                            TaxValue = CalculateDetailValue.TaxValue(clientQuotationDetail.Quantity, clientQuotationDetail.SellingPrice, clientQuotationDetail.ItemDiscountPercent, clientQuotationDetail.VatPercent, clientQuotationDetail.TaxTypeId == TaxTypeData.Taxable ? itemTax.TaxPercent : 0, itemTax.TaxAfterVatInclusive, 0, isVatInclusive)
                        }
                    ).ToList();

                clientQuotationDetail.OtherTaxValue = clientQuotationDetail.ClientQuotationDetailTaxes.Sum(x => x.TaxValue);
                clientQuotationDetail.NetValue = CalculateDetailValue.NetValue(clientQuotationDetail.Quantity, clientQuotationDetail.SellingPrice, clientQuotationDetail.ItemDiscountPercent, clientQuotationDetail.VatPercent, clientQuotationDetail.OtherTaxValue, 0, isVatInclusive);

                clientQuotationDetail.ClientQuotationDetailId = newId;
                clientQuotationDetail.ClientQuotationDetailTaxes.ForEach(y =>
                {
                    y.ClientQuotationDetailId = newId;
                    y.ClientQuotationDetailTaxId = newSubId--;
                });
                newId--;
            }

            return clientQuotationDetails;
        }

        private ClientQuotationHeaderDto GetClientQuotationHeaderFromClientPriceRequest(ClientPriceRequestHeaderDto clientPriceRequestHeader, List<ClientQuotationDetailDto> clientQuotationDetails)
        {
            var totalValueFromDetail = clientQuotationDetails.Sum(x => x.TotalValue);
            var totalItemDiscount = clientQuotationDetails.Sum(x => x.ItemDiscountValue);
            var grossValueFromDetail = clientQuotationDetails.Sum(x => x.GrossValue);
            var vatValueFromDetail = clientQuotationDetails.Sum(x => x.VatValue);
            var subNetValueFromDetail = clientQuotationDetails.Sum(x => x.SubNetValue);
            var otherTaxValueFromDetail = clientQuotationDetails.Sum(x => x.OtherTaxValue);
            var netValueFromDetail = clientQuotationDetails.Sum(x => x.NetValue);
            var totalCostValueFromDetail = clientQuotationDetails.Sum(x => x.CostValue);

            var clientQuotationHeader = new ClientQuotationHeaderDto
            {
                ClientQuotationHeaderId = 0,
                ClientPriceRequestHeaderId = clientPriceRequestHeader.ClientPriceRequestHeaderId,
                ClientId = clientPriceRequestHeader.ClientId,
                ClientCode = clientPriceRequestHeader.ClientCode,
                ClientName = clientPriceRequestHeader.ClientName,
                SellerId = clientPriceRequestHeader.SellerId,
                SellerCode = clientPriceRequestHeader.SellerCode,
                SellerName = clientPriceRequestHeader.SellerName,
                StoreId = clientPriceRequestHeader.StoreId,
                StoreName = clientPriceRequestHeader.StoreName,
                TaxTypeId = TaxTypeData.Taxable,
                DocumentDate = clientPriceRequestHeader.DocumentDate,
                Reference = clientPriceRequestHeader.Reference,
                TotalValue = totalValueFromDetail,
                DiscountPercent = 0,
                DiscountValue = 0,
                TotalItemDiscount = totalItemDiscount,
                GrossValue = grossValueFromDetail,
                VatValue = vatValueFromDetail,
                SubNetValue = subNetValueFromDetail,
                OtherTaxValue = otherTaxValueFromDetail,
                NetValueBeforeAdditionalDiscount = netValueFromDetail,
                AdditionalDiscountValue = 0,
                NetValue = CalculateHeaderValue.NetValue(netValueFromDetail, 0),
                TotalCostValue = totalCostValueFromDetail,
                ValidInDays = 0,
                ValidUntilDate = DateHelper.GetDateTimeNow(),
                RemarksAr = clientPriceRequestHeader.RemarksAr,
                RemarksEn = clientPriceRequestHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = clientPriceRequestHeader.ArchiveHeaderId,
            };

            return clientQuotationHeader;
        }

		public async Task<ExpirationDaysAndDateDto> GetDefaultValidDate(int storeId)
        {
            var validDurationSetting = await _applicationSettingService.GetApplicationSettingValueByStoreId(storeId, ApplicationSettingDetailData.DaysToQuotationIsValid);

            var inDays = int.Parse(validDurationSetting!);
            var inDate = inDays != 0 ? DateHelper.GetDateTimeNow().AddDays(inDays) : DateHelper.GetDateTimeNow().AddYears(1);

            return new ExpirationDaysAndDateDto
            {
                ValidInDays = inDays,
                ValidUntil = inDate
            };
		}


		public async Task<ResponseDto> SaveClientQuotation(ClientQuotationDto clientQuotation, bool hasApprove, bool approved, int? requestId)
        {
            if (clientQuotation.ClientQuotationHeader != null)
			{
				var itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(clientQuotation.ClientQuotationDetails, x => x.ItemId, x => x.ItemNote);
				if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

				if (!clientQuotation.ClientQuotationDetails.Any()) return new ResponseDto { Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.DetailIsEmpty) };

                if (clientQuotation.ClientQuotationHeader.ClientQuotationHeaderId == 0)
                {
                    await UpdateModelPrices(clientQuotation);
                }

                var result = await _clientQuotationHeaderService.SaveClientQuotationHeader(clientQuotation.ClientQuotationHeader, hasApprove, approved, requestId);
                if (result.Success)
                {
                    var modifiedClientQuotationDetails = await _clientQuotationDetailService.SaveClientQuotationDetails(result.Id, clientQuotation.ClientQuotationDetails);
                    await SaveClientQuotationDetailTaxes(result.Id, modifiedClientQuotationDetails);
                    await _clientQuotationDetailService.DeleteClientQuotationDetailList(modifiedClientQuotationDetails, result.Id);
                    
                    if (clientQuotation.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(clientQuotation.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        private async Task UpdateModelPrices(ClientQuotationDto clientQuotation)
        {
            await UpdateDetailPrices(clientQuotation.ClientQuotationDetails, clientQuotation.ClientQuotationHeader!.StoreId);
            clientQuotation.ClientQuotationHeader!.TotalCostValue = clientQuotation.ClientQuotationDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<ClientQuotationDetailDto> clientQuotationDetails, int storeId)
        {
            var itemIds = clientQuotationDetails.Select(x => x.ItemId).ToList();

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

            foreach (var clientQuotationDetail in clientQuotationDetails)
            {
                var packing = packings.Where(x => x.ItemId == clientQuotationDetail.ItemId && x.FromPackageId == clientQuotationDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                clientQuotationDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == clientQuotationDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                clientQuotationDetail.CostPrice = itemCosts.Where(x => x.ItemId == clientQuotationDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                clientQuotationDetail.CostPackage = clientQuotationDetail.CostPrice * packing;
                clientQuotationDetail.CostValue = clientQuotationDetail.CostPackage * clientQuotationDetail.Quantity;
                clientQuotationDetail.LastSalesPrice = lastSellingPrices.Where(x => x.ItemId == clientQuotationDetail.ItemId && x.ItemPackageId == clientQuotationDetail.ItemPackageId).Select(x => x.SellingPrice).FirstOrDefault(0);
            }
        }

        private async Task SaveClientQuotationDetailTaxes(int clientQuotationHeaderId, List<ClientQuotationDetailDto> clientQuotationDetails)
        {
            List<ClientQuotationDetailTaxDto> clientQuotationDetailTaxes = new List<ClientQuotationDetailTaxDto>();

            foreach (var clientQuotationDetail in clientQuotationDetails)
            {
                foreach (var clientQuotationDetailTax in clientQuotationDetail.ClientQuotationDetailTaxes)
                {
                    clientQuotationDetailTax.ClientQuotationDetailId = clientQuotationDetail.ClientQuotationDetailId;
                    clientQuotationDetailTaxes.Add(clientQuotationDetailTax);
                }
            }

            await _clientQuotationDetailTaxService.SaveClientQuotationDetailTaxes(clientQuotationHeaderId, clientQuotationDetailTaxes);
        }

        public async Task<ResponseDto> DeleteClientQuotation(int clientQuotationHeaderId)
        {
            await _menuNoteService.DeleteMenuNotes(MenuCodeData.ClientQuotation, clientQuotationHeaderId);
            await _clientQuotationDetailTaxService.DeleteClientQuotationDetailTaxes(clientQuotationHeaderId);
            await _clientQuotationDetailService.DeleteClientQuotationDetails(clientQuotationHeaderId);
            var result = await _clientQuotationHeaderService.DeleteClientQuotationHeader(clientQuotationHeaderId);
            return result;
        }
    }
}
