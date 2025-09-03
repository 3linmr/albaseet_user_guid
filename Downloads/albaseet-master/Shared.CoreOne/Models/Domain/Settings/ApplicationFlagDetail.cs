using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Settings
{
    public class ApplicationFlagDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ApplicationFlagDetailId { get; set; }

        [Column(Order = 2)]
        public int ApplicationFlagHeaderId { get; set; }

        [Column(Order = 3)]
        public byte ApplicationFlagTypeId { get; set; }

        [Column(Order = 4)]
        [StringLength(200)]
        public string? FlagNameAr { get; set; }

        [Column(Order = 5)]
        [StringLength(200)]
        public string? FlagNameEn { get; set; }

		[Column(Order = 6)]
        [StringLength(500)]
        public string? FlagValue { get; set; }

        [Column(Order = 7)]
        public short Order { get; set; }



        [ForeignKey(nameof(ApplicationFlagHeaderId))]
        public ApplicationFlagHeader? CompanyFlagHeader { get; set; }

        [ForeignKey(nameof(ApplicationFlagTypeId))]
        public ApplicationFlagType? CompanyFlagType { get; set; }
    }
}
