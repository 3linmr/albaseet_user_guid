using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Models.Dtos.ViewModels
{
    public class FixedAssetMovementDto
    {
        public FixedAssetMovementHeaderDto? FixedAssetMovementHeader { get; set; }
        public List<FixedAssetMovementDetailDto>? FixedAssetMovementDetails { get; set; } = new List<FixedAssetMovementDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
        public ResponseDto? Response { get; set; }
    }
    public class FixedAssetMovementDetailResponseDto
    {
        public ResponseDto? Response { get; set; }
        public List<FixedAssetMovementDetailDto>? FixedAssetMovementDetails { get; set; }
    }
    
}
