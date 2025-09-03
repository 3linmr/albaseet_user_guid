using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;

namespace Accounting.CoreOne.Models.Domain
{
    public class PaymentVoucherInvoice : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PaymentVoucherInvoiceId { get; set; }

        [Column(Order = 2)]
        public int PaymentVoucherHeaderId { get; set; }

        [Column(Order = 3)]
        public int InvoiceId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 4)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime InvoiceDate { get; set; }

        [Column(Order = 5, TypeName = "decimal(30,15)")]
        public decimal InvoiceTotalValue { get; set; }

        [Column(Order = 6, TypeName = "decimal(30,15)")]
        public decimal InvoicePaidValue { get; set; }

        [Column(Order = 7, TypeName = "decimal(30,15)")]
        public decimal InvoiceDueValue { get; set; }

        [Column(Order = 8, TypeName = "decimal(30,15)")]
        public decimal InvoiceInstallmentValue { get; set; }


        [ForeignKey(nameof(PaymentVoucherHeaderId))]
        public PaymentVoucherHeader? PaymentVoucherHeader { get; set; }
    }
}
