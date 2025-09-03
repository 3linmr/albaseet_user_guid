using Shared.CoreOne.Models.Domain.Menus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class MenuNoteDto
	{
		public int MenuNoteId { get; set; }
		public int GridSerial { get; set; } = 0;

		public short MenuCode { get; set; }

		public int ReferenceId { get; set; }

		public string? NoteValue { get; set; }
		public string? NoteValueReadable { get; set; }

		public string? Note { get; set; }

		public int MenuNoteIdentifierId { get; set; }
		public byte ColumnIdentifierId { get; set; }

		public string? MenuNoteIdentifierName { get; set; }

		public string? ColumnIdentifierName { get; set; }

		public bool ShowInReports { get; set; }

        public bool ShowOnPrint { get; set; }

		public bool ShowOnSelection { get; set; }
	}
}
