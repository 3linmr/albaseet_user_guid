using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Menus
{
	public class MenuNoteIdentifierDto
	{
		public int MenuNoteIdentifierId { get; set; }

		public int MenuNoteIdentifierCode { get; set; }

		public string? MenuNoteIdentifierNameAr { get; set; }

		public string? MenuNoteIdentifierNameEn { get; set; }

		public byte ColumnIdentifierId { get; set; }

		public short? MenuCode { get; set; }

		public string? MenuCodeName { get; set; }

		public string? ColumnIdentifierName { get; set; }

        public int CompanyId { get; set; }
    }

	public class MenuNoteIdentifierDropDownDto
	{
		public int MenuNoteIdentifierId { get; set; }
		public byte ColumnIdentifierId { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuNoteIdentifierName { get; set; }
	}
}
