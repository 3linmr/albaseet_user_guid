using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Menus
{
	public class MenuNoteIdentifier : BaseObject
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(Order = 1)] 
		public int MenuNoteIdentifierId { get; set; }

		[Column(Order = 2)]
		public int MenuNoteIdentifierCode { get; set; }

		[Column(Order = 3)]
		[Required, StringLength(200)] 
		public string? MenuNoteIdentifierNameAr { get; set; }

		[Column(Order = 4)]
		[Required, StringLength(200)] 
		public string? MenuNoteIdentifierNameEn { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }

        [Column(Order = 6)]
		public byte ColumnIdentifierId { get; set; }

		[Column(Order = 7)]
		public short? MenuCode { get; set; }
		



        [ForeignKey(nameof(ColumnIdentifierId))]
		public ColumnIdentifier? ColumnIdentifier { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [ForeignKey(nameof(MenuCode))]
		public Menu? Menu { get; set; }
	}
}
