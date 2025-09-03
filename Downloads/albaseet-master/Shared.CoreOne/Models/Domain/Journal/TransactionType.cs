using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Journal
{
    public class TransactionType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte TransactionTypeId { get; set; }

        [Required, StringLength(10)]
        [Column(Order = 2)]
        public string? TransactionTypeNameAr { get; set; }

        [Required, StringLength(10)]
        [Column(Order = 3)]
        public string? TransactionTypeNameEn { get; set; }
    }
}
