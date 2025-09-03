using Accounting.CoreOne.Models.Dtos.ViewModels;
using Accounting.CoreOne.Contracts;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Contracts.Modules;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Models.StaticData;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Service.Services
{
    public class FixedAssetMovementService : IFixedAssetMovementService
    {
        private readonly IFixedAssetMovementHeaderService _fixedAssetMovementHeaderService;
        private readonly IFixedAssetMovementDetailService _fixedAssetMovementDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IStringLocalizer<FixedAssetMovementService> _localizer;

        public FixedAssetMovementService(
            IFixedAssetMovementHeaderService fixedAssetMovementHeaderService, 
            IFixedAssetMovementDetailService fixedAssetMovementDetailService,
            IMenuNoteService menuNoteService,
            IStringLocalizer<FixedAssetMovementService> localizer)
        {
            _fixedAssetMovementHeaderService = fixedAssetMovementHeaderService;
            _fixedAssetMovementDetailService = fixedAssetMovementDetailService;
            _menuNoteService = menuNoteService;
            _localizer = localizer;
        }
        public async Task<FixedAssetMovementDto> GetFixedAssetMovement(int fixedAssetMovementHeaderId)
        {
            var header = await _fixedAssetMovementHeaderService.GetFixedAssetMovementHeaderById(fixedAssetMovementHeaderId);
            var detail = await _fixedAssetMovementDetailService.GetFixedAssetMovementDetail(fixedAssetMovementHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.FixedAssetMovement, fixedAssetMovementHeaderId).ToListAsync();
            return new FixedAssetMovementDto() { FixedAssetMovementHeader = header, FixedAssetMovementDetails = detail, MenuNotes = menuNotes };
        }

        public async Task<ResponseDto> SaveFixedAssetMovement(FixedAssetMovementDto fixedAssetMovement, bool hasApprove, bool approved, int? requestId)
        {
            if (fixedAssetMovement.FixedAssetMovementHeader != null)
            {
                var saveHeader = await _fixedAssetMovementHeaderService.SaveFixedAssetMovementHeader(fixedAssetMovement.FixedAssetMovementHeader, hasApprove, approved, requestId);
                if (saveHeader.Success)
                {
                    var saveDetail = await _fixedAssetMovementDetailService.SaveFixedAssetMovementDetail(fixedAssetMovement.FixedAssetMovementDetails!, saveHeader.Id);
                    if (saveDetail.Response!.Success)
                    {
                        if (fixedAssetMovement.MenuNotes != null)
                        {
                            await _menuNoteService.SaveMenuNotes(fixedAssetMovement.MenuNotes, saveHeader.Id);
                        }
                    }
                }
                return saveHeader;
            }
            return new ResponseDto() { Success = false, Message = _localizer["FixedAssetMovementHeaderIsNULL"] };
        }

        public async Task<ResponseDto> DeleteFixedAssetMovement(int fixedAssetMovementHeaderId)
        {
            var fixedAssetHeader = await _fixedAssetMovementHeaderService.GetFixedAssetMovementHeaderById(fixedAssetMovementHeaderId);
            await _fixedAssetMovementDetailService.DeleteFixedAssetMovementDetail(fixedAssetMovementHeaderId);
            var result = await _fixedAssetMovementHeaderService.DeleteFixedAssetMovementHeader(fixedAssetMovementHeaderId);
            return result;
        }

        public List<RequestChangesDto> GetFixedAssetMovementRequestChanges(FixedAssetMovementDto oldItem, FixedAssetMovementDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.FixedAssetMovementHeader, newItem.FixedAssetMovementHeader);
            requestChanges.AddRange(items);

            if (oldItem.FixedAssetMovementDetails!.Count != 0 && newItem.FixedAssetMovementDetails!.Count != 0)
            {
                var oldCount = oldItem.FixedAssetMovementDetails!.Count(x => x.FixedAssetId > 0);
                var newCount = newItem.FixedAssetMovementDetails!.Count(x => x.FixedAssetId > 0);
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount;)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.FixedAssetMovementDetails!.Where(x => x.FixedAssetId > 0).ToList()[i], newItem.FixedAssetMovementDetails!.Where(x => x.FixedAssetId > 0).ToList()[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }
            if (oldItem.MenuNotes != null && oldItem.MenuNotes.Count != 0 && newItem.MenuNotes != null && newItem.MenuNotes.Count != 0)
            {
                var oldCount = oldItem.MenuNotes.Count;
                var newCount = newItem.MenuNotes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount;)
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
    }
}
