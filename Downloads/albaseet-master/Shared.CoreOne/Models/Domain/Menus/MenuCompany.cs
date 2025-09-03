using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Menus
{
    public class MenuCompany : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MenuCompanyId { get; set; }

        [Column(Order = 2)]
        public short MenuCode { get; set; }

        [Column(Order = 3)]
        public int CompanyId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? MenuNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 5)]
        public string? MenuNameEn { get; set; }

        [Column(Order = 6)]
        public bool IsFavorite { get; set; }


        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }    
        
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}
