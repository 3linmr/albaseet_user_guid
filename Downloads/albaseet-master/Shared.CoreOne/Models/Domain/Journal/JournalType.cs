using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Models;

namespace Shared.CoreOne.Models.Domain.Journal
{
    public class JournalType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte JournalTypeId { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 2)]
        public string? JournalTypeNameAr { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 3)]
        public string? JournalTypeNameEn { get; set; }
    }
}
