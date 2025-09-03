using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.FixedAssets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets
{
    public class FixedAssetDto
    {
        public int FixedAssetId { get; set; }
        public string? FixedAssetCode { get; set; }
        public string? FixedAssetName { get; set; }
        public string? FixedAssetNameAr { get; set; }
        public string? FixedAssetNameEn { get; set; }
        public int CompanyId { get; set; }
        public bool IsMainFixedAsset { get; set; }
        public int? MainFixedAssetId { get; set; }
        public string? MainFixedAssetCode { get; set; }
        public string? MainFixedAssetName { get; set; }
        public byte FixedAssetLevel { get; set; }
        public int Order { get; set; }
        public bool IsLastLevel { get; set; }
        public int? AccountId { get; set; }
        public short CurrencyId { get; set; }
        public short DepreciationCurrencyId { get; set; }
        public short CumulativeCurrencyId { get; set; }   
            
        public int? DepreciationAccountId { get; set; }
        public int? CumulativeDepreciationAccountId { get; set; }
        public decimal AnnualDepreciationPercent { get; set; }
        public bool IsPrivate { get; set; }
        public bool? IsActive { get; set; }
        public string? InActiveReasons { get; set; }
        public string? InActiveReasonsAr { get; set; }
        public string? InActiveReasonsEn { get; set; }
        public bool HasRemarks { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public bool IsNonEditable { get; set; }
        public string? NotesAr { get; set; }
        public string? NotesEn { get; set; }
        public int? ArchiveHeaderId { get; set; }
        public bool HasChildren { get; set; }

    }
    public class FixedAssetTreeDto
    {
        public int FixedAssetId { get; set; }
        public string? FixedAssetCode { get; set; }
        public string? FixedAssetNameAr { get; set; }
        public string? FixedAssetNameEn { get; set; }
        public bool IsMainFixedAsset { get; set; }
        public int MainFixedAssetId { get; set; }
        public string? MainFixedAssetCode { get; set; }
        public byte FixedAssetLevel { get; set; }
        public int Order { get; set; }
        public bool IsLastLevel { get; set; }
    }
    public class FixedAssetAutoCompleteDto
    {
        public int FixedAssetId { get; set; }
        public string? FixedAssetCode { get; set; }
        public string? FixedAssetName { get; set; }
        public byte FixedAssetLevel { get; set; }
    }
    public class FixedAssetTreeVm
	{
		public int FixedAssetId { get; set; }
		public string? FixedAssetCode { get; set; }
		public string? FixedAssetNameAr { get; set; }
		public string? FixedAssetNameEn { get; set; }
		public bool IsMainFixedAsset { get; set; }
		public int? MainFixedAssetId { get; set; }
		public byte FixedAssetLevel { get; set; }
		public bool IsLastLevel { get; set; }

		public List<FixedAssetTreeVm>? Children { get; set; }
	}
    public class FixedAssetSimpleTreeDto
    {
        public int FixedAssetId { get; set; }
        public int? MainFixedAssetId { get; set; }
        public string? FixedAssetCode { get; set; }
        public byte FixedAssetLevel { get; set; }
        public string? FixedAssetName { get; set; }
        public List<FixedAssetSimpleTreeDto> List { get; set; } = new List<FixedAssetSimpleTreeDto>();
    }
    public class FixedAssetDropDownDto
    {
        public int FixedAssetId { get; set; }
        public string? FixedAssetName { get; set; }
    }
}
