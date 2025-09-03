using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Items
{
    public class ItemAttributeType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemAttributeTypeId { get; set; }

        [Column(Order = 2)]
        public int ItemAttributeTypeCode { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? ItemAttributeTypeNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? ItemAttributeTypeNameEn { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}
