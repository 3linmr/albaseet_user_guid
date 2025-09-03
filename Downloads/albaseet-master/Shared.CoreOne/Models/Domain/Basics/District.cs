using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Basics
{
    public class District : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int DistrictId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 2)]
        public string? DistrictNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? DistrictNameEn { get; set; }

        [Column(Order = 4)]
        public int CityId { get; set; }


        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }
    }
}
