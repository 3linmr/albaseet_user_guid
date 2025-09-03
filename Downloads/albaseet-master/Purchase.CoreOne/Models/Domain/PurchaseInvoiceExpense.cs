using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Basics;

namespace Purchases.CoreOne.Models.Domain
{
    public class PurchaseInvoiceExpense : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int PurchaseInvoiceExpenseId { get; set; }

        [Column(Order = 2)]
        public int PurchaseInvoiceHeaderId { get; set; }

        [Column(Order = 3)]
        public int InvoiceExpenseTypeId { get; set; }

        [Column(Order = 4, TypeName = "decimal(30,15)")]
        public decimal ExpenseValue { get; set; }

        [StringLength(500)]
        [Column(Order = 5)]
        public string? RemarksAr { get; set; }

        [StringLength(500)]
        [Column(Order = 6)]
        public string? RemarksEn { get; set; }


		[ForeignKey(nameof(PurchaseInvoiceHeaderId))]
        public PurchaseInvoiceHeader? PurchaseInvoiceHeader { get; set; }		
        
        [ForeignKey(nameof(InvoiceExpenseTypeId))]
        public InvoiceExpenseType? InvoiceExpenseType { get; set; }
	}
}
