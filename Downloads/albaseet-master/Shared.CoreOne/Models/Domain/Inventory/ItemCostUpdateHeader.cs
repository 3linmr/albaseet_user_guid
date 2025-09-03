using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Inventory
{
    public class ItemCostUpdateHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemCostUpdateHeaderId { get; set; }

        [Column(Order = 2)]
        public DateTime EntryDate { get; set; }

        [Column(Order = 3)]
        public int StoreId { get; set; }


        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }
    }
}
