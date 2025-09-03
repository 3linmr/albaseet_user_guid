using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Archive;

namespace Inventory.CoreOne.Models.Domain
{
    public class ItemImportExcelHistory : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int Id { get; set; }

        [Column(Order = 2)]
        public int ItemCount { get; set; }

        [Column(Order = 3)]
        public int? ArchiveHeaderId { get; set; }



        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
