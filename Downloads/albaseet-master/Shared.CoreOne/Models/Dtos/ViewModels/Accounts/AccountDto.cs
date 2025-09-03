using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Accounts
{
	public class AccountDto
	{
		public int AccountId { get; set; }
		public string? AccountCode { get; set; }
		public int CompanyId { get; set; }
		public string? AccountNameAr { get; set; }
		public string? AccountNameEn { get; set; }
		public byte AccountCategoryId { get; set; }
		public byte? AccountTypeId { get; set; }
		public bool IsMainAccount { get; set; }
		public int? MainAccountId { get; set; }
		public string? MainAccountCode { get; set; }
		public string? MainAccountName { get; set; }
		public byte AccountLevel { get; set; }
		public int Order { get; set; }
		public bool IsLastLevel { get; set; }
		public short CurrencyId { get; set; }
		public string? CurrencyName { get; set; }
		public bool HasCostCenter { get; set; }
		public int? CostCenterId { get; set; }
		public string? CostCenterName { get; set; }
		public bool IsPrivate { get; set; }
		public bool IsActive { get; set; }
		public bool IsCreatedAutomatically { get; set; }
		public string? InActiveReasons { get; set; }
		public bool HasRemarks { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public bool IsNonEditable { get; set; }
		public string? NotesAr { get; set; }
		public string? NotesEn { get; set; }
		public int? ArchiveHeaderId { get; set; }
        public int? InternalReferenceAccountId { get; set; } //Like Reference On Fixed Assets(Mirroring)

		public bool CreateNewClient { get; set; }
		public bool CreateNewSupplier { get; set; }
		public bool CreateNewBank { get; set; }
		public int? ClientId { get; set; }
		public string? ClientName { get; set; }
		public int? SupplierId { get; set; }
		public string? SupplierName { get; set; }
		public int? BankId { get; set; }
		public string? BankName { get; set; }
		public bool HasChildren { get; set; }
	}

	public class AccountTreeDto
	{
		public int AccountId { get; set; }
		public string? AccountCode { get; set; }
		public byte AccountCategoryId { get; set; }
		public byte? AccountTypeId { get; set; }
		public string? AccountCategoryName { get; set; }
		public string? AccountName { get; set; }
		public string? AccountNameAr { get; set; }
		public string? AccountNameEn { get; set; }
		public bool IsMainAccount { get; set; }
		public int MainAccountId { get; set; }
		public string? MainAccountCode { get; set; }
		public byte AccountLevel { get; set; }
		public short CurrencyId { get; set; }
		public int Order { get; set; }
		public bool IsLastLevel { get; set; }
		public bool IsCreatedAutomatically { get; set; }
	}

	public class AccountSimpleTreeDto
	{
		public int AccountId { get; set; }
		public int? MainAccountId { get; set; }
		public string? AccountCode { get; set; }
		public byte AccountLevel { get; set; }
		public string? AccountName { get; set; }
		public List<AccountSimpleTreeDto> List { get; set; } = new List<AccountSimpleTreeDto>();
	}

	public class AccountAutoCompleteDto
	{
		public int AccountId { get; set; }
		public int CurrencyId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountName { get; set; }
		public byte AccountLevel { get; set; }
		public byte? AccountTypeId { get; set; }
	}

	public class AccountNameDto
	{
		public int AccountId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountName { get; set; }
		public int CurrencyId { get; set; }
	}

	public class AccountTreeVm
	{
		public int AccountId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountNameAr { get; set; }
		public string? AccountNameEn { get; set; }
		public bool IsMainAccount { get; set; }
		public int? MainAccountId { get; set; }
		public byte AccountLevel { get; set; }
		public bool IsLastLevel { get; set; }

		public List<AccountTreeVm>? Children { get; set; }
	}

	public class AccountTaxDto
	{
		public int? AccountId { get; set; }
		public short CurrencyId { get; set; }
		public byte TaxTypeId { get; set; }
		public decimal TaxPercent { get; set; }
		public decimal DebitValue { get; set; }
		public decimal DebitValueEvaluated { get; set; }
		public decimal CreditValue { get; set; }
		public decimal CreditValueEvaluated { get; set; }
	}

	public class FixedAssetAccountDto
	{
		public int AccountId { get; set; }

        public int? MainAccountId { get; set; }

		public string? FixedAssetNameAr { get; set; }

		public string? FixedAssetNameEn { get; set; }

		public int CompanyId { get; set; }
        public bool? IsMainAccount { get; set; }
    }

	public class FixedAssetAccountReturnDto
	{
		public int AccountId { get; set; }

		public int DepreciationAccountId { get; set; }

		public int CumulativeDepreciationAccountId { get; set; }
		public required ResponseDto Result { get; set; }
	}
}
