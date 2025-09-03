using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Taxes
{
    public class Tax : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TaxId { get; set; }

        [Column(Order = 2)]
        public int TaxCode { get; set; }

		[Column(Order = 3)]
		[Required, StringLength(100)]
		public string? TaxNameAr { get; set; }

		[Column(Order = 4)]
		[Required, StringLength(100)]
		public string? TaxNameEn { get; set; }

		[Column(Order = 5)]
		public int CompanyId { get; set; }

		[Column(Order = 6)]
        public byte TaxTypeId { get; set; }

        [Column(Order = 7)]
        public int? DrAccount { get; set; }

        [Column(Order = 8)]
        public int? CrAccount { get; set; }

        [Column(Order = 9)]
		public bool IsVatTax { get; set; }

        [Column(Order = 10)]
        public bool TaxAfterVatInclusive { get; set; } //احتساب الضريبة علي المبلغ شامل الضرائب (Value + VAT)


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

		[ForeignKey(nameof(TaxTypeId))]
        public TaxType? TaxType { get; set; }

        [ForeignKey(nameof(DrAccount))]
        public Account? DebitAccount { get; set; }

        [ForeignKey(nameof(CrAccount))]
        public Account? CreditAccount { get; set; }
	}
}
