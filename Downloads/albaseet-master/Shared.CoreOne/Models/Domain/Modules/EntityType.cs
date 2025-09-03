using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
	public class EntityType : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public byte EntityTypeId { get; set; }

		[Required, StringLength(100)]
		[Column(Order = 2)]
		public string? EntityTypeNameAr { get; set; }

		[Required, StringLength(100)]
		[Column(Order = 3)]
		public string? EntityTypeNameEn { get; set; }

		[Column(Order = 4)]
		public byte Order { get; set; }
	}
}
