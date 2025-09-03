using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Menus
{
    public class Menu : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short MenuCode { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 2)]
        public string? MenuNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? MenuNameEn { get; set; }

        [StringLength(100)]
        [Column(Order = 4)]
		public string? MenuUrl { get; set; }

        [Column(Order = 5)]
        public bool HasApprove { get; set; }

        [Column(Order = 6)]
        public bool HasNotes { get; set; }

		[Column(Order = 7)]
        public bool HasEncoding { get; set; }

        [Column(Order = 8)]
        public bool IsFavorite { get; set; }

        [Column(Order = 9)]
        public bool IsReport { get; set; }
	}
}
