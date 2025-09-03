using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class PaymentType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public byte PaymentTypeId { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 2)] 
        public string? PaymentTypeCode { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 3)] 
        public string? PaymentTypeNameAr { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 4)]
        public string? PaymentTypeNameEn { get; set; }
    }
}
