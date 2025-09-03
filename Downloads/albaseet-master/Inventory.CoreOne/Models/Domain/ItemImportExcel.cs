using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;

namespace Inventory.CoreOne.Models.Domain
{
    public class ItemImportExcel : Item
    {
        [Column(Order = 0)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemImportExcelId { get; set; }

        public int ItemImportExcelHistoryId { get; set; }
        


        [ForeignKey(nameof(ItemImportExcelHistoryId))]
        public ItemImportExcelHistory? ItemImportExcelHistory { get; set; }
    }
}
