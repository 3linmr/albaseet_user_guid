using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
    public class DistrictDto
    {
        public int DistrictId { get; set; }

        public string? DistrictNameAr { get; set; }

        public string? DistrictNameEn { get; set; }

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

    public class DistrictDropDownDto
    {
        public int DistrictId { get; set; }
        public string? DistrictName { get; set; }
    }

}
