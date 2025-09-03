using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Accounting.CoreOne.Models.Dtos.ViewModels
{
    public class FixedAssetVoucherDto
    {
        public FixedAssetVoucherHeaderDto? FixedAssetVoucherHeader { get; set; }
        public List<FixedAssetVoucherDetailDto>? FixedAssetVoucherDetails { get; set; } = new List<FixedAssetVoucherDetailDto>();
        public List<FixedAssetJournalVoucherDto>? FixedAssetJournalVouchers { get; set; } = new List<FixedAssetJournalVoucherDto>();
        public List<CostCenterJournalDetailDto>? CostCenterJournalDetails { get; set; } = new List<CostCenterJournalDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
        public ResponseDto? Response { get; set; }
    }

    public class FixedAssetVoucherHeaderDto
    {
        public int? FixedAssetVoucherHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int? DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public byte? FixedAssetVoucherTypeId { get; set; }
        public string? DocumentReference { get; set; }
        public int? StoreId { get; set; }
        public DateTime? EntryDate { get; set; } = DateHelper.GetDateTimeNow();
        public DateTime? DocumentDate { get; set; }
        public string? PeerReference { get; set; }
        public string? FixedAssetReference { get; set; }
        public int? SellerId { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public int? JournalHeaderId { get; set; }
        public int? ArchiveHeaderId { get; set; }
        public string? StoreName { get; set; }
        public int? StoreCurrencyId { get; set; }
        public string? SellerName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? IpAddressCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public string? UserNameModified { get; set; }
    }
    public class FixedAssetVoucherDetailDto
    {
        public int FixedAssetVoucherDetailId { get; set; }
        public int FixedAssetVoucherHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int? DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public int FixedAssetId { get; set; }
        public string? FixedAssetName { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public byte? FixedAssetVoucherTypeId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }
        public DateTime? DocumentDate { get; set; }
        public decimal? DetailValue { get; set; }
        public int? JournalHeaderId { get; set; }
        public int? StoreId { get; set; }
        public List<FixedAssetVoucherDetailPaymentDto>? FixedAssetVoucherDetailPayments { get; set; }
    }
    public class FixedAssetJournalVoucherDto
    {
        public int? FixedAssetVoucherDetailId { get; set; }
        public int? FixedAssetVoucherHeaderId { get; set; }
        public int? FixedAssetId { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int? AccountId { get; set; }
        public short? CurrencyId { get; set; }
        public decimal? CurrencyRate { get; set; }
        public decimal? CreditValue { get; set; }
        public decimal? DebitValue { get; set; }
        public decimal? CreditValueAccount { get; set; }
        public decimal? DebitValueAccount { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
    }

    public class FixedAssetAdditionDto
    {
        public int? FixedAssetVoucherHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int? DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public int? FixedAssetId { get; set; }
        public string? FixedAssetName { get; set; }
        public DateTime? AdditionDate { get; set; }
        public int? StoreId { get; set; }
        public decimal? AdditionValue { get; set; }
        public List<FixedAssetVoucherDetailPaymentDto>? FixedAssetVoucherDetailPayments { get; set; }
    }
    public class FixedAssetDepreciationDto
    {
        public int? FixedAssetVoucherHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int? DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public DateTime? DepreciationDate { get; set; }
        public int? StoreId { get; set; }
        public decimal? DepreciationValue { get; set; }
    }

    public class FixedAssetExclusionDto
    {
        public int? FixedAssetVoucherHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int? DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public int? FixedAssetId { get; set; }
        public string? FixedAssetName { get; set; }
        public DateTime? ExclusionDate { get; set; }
        public int? StoreId { get; set; }
        public int? SalesAccountId { get; set; }
        public decimal? ExclusionToValue { get; set; }
        public List<FixedAssetVoucherDetailPaymentDto>? FixedAssetVoucherDetailPayments { get; set; }
    }
    public class FixedAssetVoucherDetailResponseDto
    {
        public ResponseDto? Response { get; set; }
        public List<FixedAssetVoucherDetailDto>? FixedAssetVoucherDetails { get; set; }
    }    
}
