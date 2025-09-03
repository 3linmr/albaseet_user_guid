using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;

namespace Shared.CoreOne.Models.Domain.Accounts
{
    public class AccountLedger : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte AccountLedgerId { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 2)]
        public string? AccountLedgerNameAr { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 3)]
        public string? AccountLedgerNameEn { get; set; }
    }
}
