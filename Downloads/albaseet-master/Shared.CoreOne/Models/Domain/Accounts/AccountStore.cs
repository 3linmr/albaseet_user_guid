using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Accounts
{
    public class AccountStore : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountStoreId { get; set; }

        [Column(Order = 2)]
        public int AccountStoreTypeId { get; set; }

        [Column(Order = 3)]
        public int StoreId { get; set; }

        [Column(Order = 4)]
        public int AccountId { get; set; }


        [ForeignKey(nameof(AccountStoreTypeId))]
        public AccountStoreType? AccountStoreType { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Account? Account { get; set; }

    }
}
