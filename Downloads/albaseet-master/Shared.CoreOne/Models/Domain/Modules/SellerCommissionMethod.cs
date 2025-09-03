using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
	public class SellerCommissionMethod : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public short SellerCommissionMethodId { get; set; }

		[Column(Order = 2)]
		public short SellerCommissionMethodCode { get; set; }

		[Column(Order = 3)]
		[StringLength(100)] 
		public string? SellerCommissionMethodNameAr { get; set; }

		[Column(Order = 4)]
		[StringLength(100)] 
		public string? SellerCommissionMethodNameEn { get; set; }

		[Column(Order = 5)]
		public byte SellerCommissionTypeId { get; set; }

        [Column(Order = 6)]
        public int CompanyId { get; set; }

		[Column(Order = 7)]
		public bool IsActive { get; set; }

		[StringLength(500)]
		[Column(Order = 8)]
		public string? InActiveReasons { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

		[ForeignKey(nameof(SellerCommissionTypeId))]
		public SellerCommissionType? SellerCommissionType { get; set; }
	}
}
