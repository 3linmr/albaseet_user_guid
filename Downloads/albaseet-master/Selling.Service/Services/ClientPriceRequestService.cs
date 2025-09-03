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
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Models.StaticData;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Items;

namespace Sales.Service.Services
{
    public class ClientPriceRequestService : IClientPriceRequestService
    {
        private readonly IClientPriceRequestHeaderService _clientPriceRequestHeaderService;
        private readonly IClientPriceRequestDetailService _clientPriceRequestDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

        public ClientPriceRequestService(IClientPriceRequestHeaderService clientPriceRequestHeaderService, IClientPriceRequestDetailService clientPriceRequestDetailService, IMenuNoteService menuNoteService, IGenericMessageService genericMessageService, IItemNoteValidationService itemNoteValidationService)
        {
            _clientPriceRequestHeaderService = clientPriceRequestHeaderService;
            _clientPriceRequestDetailService = clientPriceRequestDetailService;
            _menuNoteService = menuNoteService;
            _genericMessageService = genericMessageService;
            _itemNoteValidationService = itemNoteValidationService;
        }

        public List<RequestChangesDto> GetClientPriceRequestRequestChanges(ClientPriceRequestDto oldItem, ClientPriceRequestDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.ClientPriceRequestHeader, newItem.ClientPriceRequestHeader);
            requestChanges.AddRange(items);

            if (oldItem.ClientPriceRequestDetails.Any() && newItem.ClientPriceRequestDetails.Any())
            {
                var oldCount = oldItem.ClientPriceRequestDetails.Count;
                var newCount = newItem.ClientPriceRequestDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ClientPriceRequestDetails[i], newItem.ClientPriceRequestDetails[i]);
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

        public async Task<ClientPriceRequestDto> GetClientPriceRequest(int clientPriceRequestHeaderId)
        {
            var header = await _clientPriceRequestHeaderService.GetClientPriceRequestHeaderById(clientPriceRequestHeaderId);
            var detail = await _clientPriceRequestDetailService.GetClientPriceRequestDetails(clientPriceRequestHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ClientPriceRequest, clientPriceRequestHeaderId).ToListAsync();
            return new ClientPriceRequestDto() { ClientPriceRequestHeader = header, ClientPriceRequestDetails = detail, MenuNotes = menuNotes };
        }

        public async Task<ResponseDto> SaveClientPriceRequest(ClientPriceRequestDto clientPriceRequest, bool hasApprove, bool approved, int? requestId)
        {
            if (clientPriceRequest.ClientPriceRequestHeader != null)
			{
				var itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(clientPriceRequest.ClientPriceRequestDetails, x => x.ItemId, x => x.ItemNote);
				if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

				if (!clientPriceRequest.ClientPriceRequestDetails.Any()) return new ResponseDto { Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.DetailIsEmpty) };

                var result = await _clientPriceRequestHeaderService.SaveClientPriceRequestHeader(clientPriceRequest.ClientPriceRequestHeader, hasApprove, approved, requestId);
                if (result.Success)
                {
                    await _clientPriceRequestDetailService.SaveClientPriceRequestDetails(result.Id, clientPriceRequest.ClientPriceRequestDetails);
                    if (clientPriceRequest.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(clientPriceRequest.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        public async Task<ResponseDto> DeleteClientPriceRequest(int clientPriceRequestHeaderId)
        {
            await _menuNoteService.DeleteMenuNotes(MenuCodeData.ClientPriceRequest, clientPriceRequestHeaderId);
            await _clientPriceRequestDetailService.DeleteClientPriceRequestDetails(clientPriceRequestHeaderId);
            var result = await _clientPriceRequestHeaderService.DeleteClientPriceRequestHeader(clientPriceRequestHeaderId);
            return result;
        }
    }
}
