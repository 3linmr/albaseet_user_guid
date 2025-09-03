using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Basics
{
    public class City : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int CityId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 2)]
        public string? CityNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? CityNameEn { get; set; }

        [Column(Order = 4)]
        public int StateId { get; set; }


        [ForeignKey(nameof(StateId))]
        public State? State { get; set; }
    }
}
