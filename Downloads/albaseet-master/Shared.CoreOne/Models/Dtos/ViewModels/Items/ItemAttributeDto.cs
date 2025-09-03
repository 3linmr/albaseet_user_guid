using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
	public class ItemAttributeDto
	{
		public int ItemAttributeId { get; set; }
		public int ItemAttributeTypeId { get; set; }
		public string? ItemAttributeTypeName { get; set; }
		public int ItemId { get; set; }
		public string? ItemAttributeNameAr { get; set; }
		public string? ItemAttributeNameEn { get; set; }
	}
}
