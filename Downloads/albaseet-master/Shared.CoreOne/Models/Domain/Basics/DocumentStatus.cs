using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Basics
{
    public class DocumentStatus : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int DocumentStatusId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 2)]
        public string? DocumentStatusNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? DocumentStatusNameEn { get; set; }
    }
}
