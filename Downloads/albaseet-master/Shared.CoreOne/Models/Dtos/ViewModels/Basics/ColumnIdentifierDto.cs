using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
	public class ColumnIdentifierDto
	{
		public byte ColumnIdentifierId { get; set; }

		public string? ColumnIdentifierNameAr { get; set; }

		public string? ColumnIdentifierNameEn { get; set; }
	}

	public class ColumnIdentifierDropDownDto
	{
		public byte ColumnIdentifierId { get; set; }

		public string? ColumnIdentifierName{ get; set; }
	}
}
