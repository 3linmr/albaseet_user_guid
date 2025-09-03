using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Basics
{
    public class ShippingStatus : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int ShippingStatusId { get; set; }

        [Column(Order = 2)]
        public int ShippingStatusCode { get; set; }

        [Required, StringLength(200)]
        [Column(Order = 3)]
        public string? ShippingStatusNameAr { get; set; }

        [Required, StringLength(200)]
        [Column(Order = 4)]
        public string? ShippingStatusNameEn { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }

        [Column(Order = 6)]
        public short MenuCode { get; set; }

		[Column(Order = 7)]
        public int StatusOrder { get; set; }

        [Column(Order = 8)]
        public bool IsActive { get; set; }

        [StringLength(2000)]
        [Column(Order = 9)]
        public string? InActiveReasons { get; set; }

		[ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        
        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }
	}
}
