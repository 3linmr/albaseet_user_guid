using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Accounts
{
    public class AccountCategory : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte AccountCategoryId { get; set; }

		[Column(Order = 2)]
		public byte AccountLedgerId { get; set; }

		[Required, StringLength(50)]
        [Column(Order = 3)]
        public string? AccountCategoryNameAr { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 4)]
        public string? AccountCategoryNameEn { get; set; }

        [ForeignKey(nameof(AccountLedgerId))]
        public AccountLedger? AccountLedger { get; set; }
    }
}
