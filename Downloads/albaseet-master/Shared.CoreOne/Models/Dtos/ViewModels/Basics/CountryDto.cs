using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
    public class CountryDto
    {
        public int CountryId { get; set; }

        public string? CountryNameAr { get; set; }

        public string? CountryNameEn { get; set; }

        public short? CurrencyId { get; set; }
        public string? CurrencyName { get; set; }

        public string? CountryCode { get; set; }

        public string? PhoneCode { get; set; }

	}

	public class CountryDropDownDto
    {
        public int CountryId { get; set; }

        public string? CountryName { get; set; }

        public string? CountryCode { get; set; }

        public string? PhoneCode { get; set; }
	}
}
