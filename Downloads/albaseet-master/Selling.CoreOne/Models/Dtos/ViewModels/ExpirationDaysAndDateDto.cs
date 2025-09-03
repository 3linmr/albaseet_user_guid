using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Models.Dtos.ViewModels
{
	public class ExpirationDaysAndDateDto
	{
		public int ValidInDays { get; set; }
		public DateTime ValidUntil { get; set; }
	}
}
