using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;

namespace Shared.CoreOne.Models.Domain.Settings
{
    public class ApplicationFlagHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ApplicationFlagHeaderId { get; set; }

        [Column(Order = 2)]
        public int ApplicationFlagTabId { get; set; }

		[Column(Order = 3)]
        [StringLength(200)]
        public string? ApplicationFlagHeaderNameAr { get; set; }

        [Column(Order = 4)]
        [StringLength(200)]
        public string? ApplicationFlagHeaderNameEn { get; set; }

        [Column(Order = 5)]
        public short Order { get; set; }
      

        [ForeignKey(nameof(ApplicationFlagTabId))]
        public ApplicationFlagTab? ApplicationFlagTab { get; set; }
	}
}
