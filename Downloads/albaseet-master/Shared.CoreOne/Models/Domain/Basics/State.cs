using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Basics
{
    public class State : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int StateId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 2)]
        public string? StateNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? StateNameEn { get; set; }

        [Column(Order = 4)]
        public int CountryId { get; set; }


        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; }

    }
}
