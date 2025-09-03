using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Journal
{
	public class EntityTypeDto
	{
		public int EntityTypeId { get; set; }
		public string? EntityTypeName { get; set; }
		public byte Order { get; set; }
	}
}
