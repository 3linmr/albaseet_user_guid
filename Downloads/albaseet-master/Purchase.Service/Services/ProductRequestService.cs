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
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Models.StaticData;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Items;

namespace Purchases.Service.Services
{
    public class ProductRequestService : IProductRequestService
    {
        private readonly IProductRequestHeaderService _productRequestHeaderService;
        private readonly IProductRequestDetailService _productRequestDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

        public ProductRequestService(IProductRequestHeaderService productRequestHeaderService, IProductRequestDetailService productRequestDetailService, IMenuNoteService menuNoteService, IItemNoteValidationService itemNoteValidationService)
        {
            _productRequestHeaderService = productRequestHeaderService;
            _productRequestDetailService = productRequestDetailService;
            _menuNoteService = menuNoteService;
            _itemNoteValidationService = itemNoteValidationService;
        }

        public List<RequestChangesDto> GetProductRequestRequestChanges(ProductRequestDto oldItem, ProductRequestDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.ProductRequestHeader, newItem.ProductRequestHeader);
            requestChanges.AddRange(items);

            if (oldItem.ProductRequestDetails.Any() && newItem.ProductRequestDetails.Any())
            {
                var oldCount = oldItem.ProductRequestDetails.Count;
                var newCount = newItem.ProductRequestDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ProductRequestDetails[i], newItem.ProductRequestDetails[i]);
                            requestChanges.AddRange(changes);
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

            return requestChanges;
        }

        public async Task<ProductRequestDto> GetProductRequest(int productRequestHeaderId)
        {
            var header = await _productRequestHeaderService.GetProductRequestHeaderById(productRequestHeaderId);
            var detail = await _productRequestDetailService.GetProductRequestDetails(productRequestHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ProductRequest, productRequestHeaderId).ToListAsync();
            return new ProductRequestDto() { ProductRequestHeader = header, ProductRequestDetails = detail, MenuNotes = menuNotes };
        }


		public async Task<ResponseDto> SaveProductRequest(ProductRequestDto productRequest, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(productRequest.ProductRequestDetails);
            if (productRequest.ProductRequestHeader != null)
            {
                var itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(productRequest.ProductRequestDetails, x => x.ItemId, x => x.ItemNote);
                if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

                var result = await _productRequestHeaderService.SaveProductRequestHeader(productRequest.ProductRequestHeader, hasApprove, approved, requestId);
                if (result.Success)
                {
                    await _productRequestDetailService.SaveProductRequestDetails(result.Id, productRequest.ProductRequestDetails);
                    if (productRequest.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(productRequest.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

		private void TrimDetailStrings(List<ProductRequestDetailDto> productRequestDetails)
		{
			foreach (var productRequestDetail in productRequestDetails)
			{
				productRequestDetail.ItemNote = string.IsNullOrWhiteSpace(productRequestDetail.ItemNote) ? null : productRequestDetail.ItemNote.Trim();
			}
		}

        public async Task<ResponseDto> DeleteProductRequest(int productRequestHeaderId)
        {
            await _menuNoteService.DeleteMenuNotes(MenuCodeData.ProductRequest, productRequestHeaderId);
            await _productRequestDetailService.DeleteProductRequestDetails(productRequestHeaderId);
            var result = await _productRequestHeaderService.DeleteProductRequestHeader(productRequestHeaderId);
            return result;
        }
    }
}
