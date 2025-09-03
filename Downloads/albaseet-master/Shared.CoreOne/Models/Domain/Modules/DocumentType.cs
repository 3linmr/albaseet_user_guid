using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class DocumentType : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)] 
        public short DocumentTypeId { get; set; }

        [Required, StringLength(50)]
        public string? DocumentTypeNameAr { get; set; }

        [Required, StringLength(50)]
        public string? DocumentTypeNameEn { get; set; }
    }
}
