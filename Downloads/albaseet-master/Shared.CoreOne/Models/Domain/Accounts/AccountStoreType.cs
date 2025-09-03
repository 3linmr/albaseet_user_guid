using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Accounts
{
    public class AccountStoreType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountStoreTypeId { get; set; }

        [Column(Order = 2)]
        [Required, StringLength(20)]
        public string? AccountStoreTypeNameAr { get; set; }

        [Column(Order = 3)]
        [Required, StringLength(20)]
        public string? AccountStoreTypeNameEn { get; set; }
    }
}
