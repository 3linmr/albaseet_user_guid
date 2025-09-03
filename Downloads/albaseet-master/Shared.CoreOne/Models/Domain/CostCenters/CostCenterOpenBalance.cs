using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Journal;

namespace Shared.CoreOne.Models.Domain.CostCenters
{
    public class CostCenterOpenBalance : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OpenBalanceId { get; set; }
        
        [Column(Order = 2)]
        public int CostCenterId { get; set; }
        
        [Column(Order = 3)]
        public int Serial { get; set; }
        
        [Column(Order = 4)]
        public byte TransactionTypeId { get; set; }
        
        [Column(TypeName = "decimal(30,15)", Order = 5)]
        public decimal TransactionValue { get; set; }



        [ForeignKey(nameof(CostCenterId))]
        public CostCenter? CostCenter { get; set; }

        [ForeignKey(nameof(TransactionTypeId))]
        public TransactionType? TransactionType { get; set; }
    }
}
