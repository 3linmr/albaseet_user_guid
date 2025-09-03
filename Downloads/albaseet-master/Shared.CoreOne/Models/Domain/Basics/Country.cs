using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Basics
{
    public class Country : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int CountryId { get; set; }

        [Required,StringLength(100)]
        [Column(Order = 2)]
        public string? CountryNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? CountryNameEn { get; set; }

        [Column(Order = 4)]
        public short? CurrencyId { get; set; }

        [Column(Order = 5)]
        [StringLength(10)]
        public string? CountryCode { get; set; }

        [StringLength(10)]
        [Column(Order = 6)]
        public string? PhoneCode { get; set; }


		[ForeignKey(nameof(CurrencyId))]
        public Currency? Currency { get; set; }
    }
}
