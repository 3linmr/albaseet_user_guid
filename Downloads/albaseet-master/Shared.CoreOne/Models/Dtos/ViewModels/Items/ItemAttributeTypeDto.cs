using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
	public class ItemAttributeTypeDto
	{
		public int ItemAttributeTypeId { get; set; }
		public int ItemAttributeTypeCode { get; set; }
		public string? ItemAttributeTypeNameAr { get; set; }
		public string? ItemAttributeTypeNameEn { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}

	public class ItemAttributeTypeDropDownDto
	{
		public int ItemAttributeTypeId { get; set; }
		public string? ItemAttributeTypeName { get; set; }
	}
}
