using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Basics
{
	public class ColumnIdentifier : BaseObject
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(Order = 1)] 
		public byte ColumnIdentifierId { get; set; }

		[Column(Order = 2)]
		[Required, StringLength(100)] 
		public string? ColumnIdentifierNameAr { get; set; }

		[Column(Order = 3)]
		[Required, StringLength(100)] 
		public string? ColumnIdentifierNameEn { get; set; }
	}
}
