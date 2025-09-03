using Shared.CoreOne.Models.Domain.Basics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
    public class CityDto
    {
        public int CityId { get; set; }

        public string? CityNameAr { get; set; }

        public string? CityNameEn { get; set; }
        public int CountryId { get; set; }

        public string? CountryNameAr { get; set; }

        public string? CountryNameEn { get; set; }

        public int StateId { get; set; }

        public string? StateNameAr { get; set; }

        public string? StateNameEn { get; set; }
    }

    public class CityDropDownDto
    {
        public int CityId { get; set; }
        public string? CityName { get; set; }
    }
}
