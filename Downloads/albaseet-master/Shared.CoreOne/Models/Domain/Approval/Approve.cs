using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Approval
{
    public class Approve : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int ApproveId { get; set; }

        [Column(Order = 2)]
        public int ApproveCode { get; set; }

        [Column(Order = 3)]
        public int ApplicationId { get; set; }

        [Column(Order = 4)]
        public short MenuCode { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 6)]
        public string? ApproveNameAr { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 7)]
        public string? ApproveNameEn { get; set; }

        [Column(Order = 8)]
        public bool OnAdd { get; set; }

        [Column(Order = 9)]
        public bool OnEdit { get; set; }

        [Column(Order = 10)]
        public bool OnDelete { get; set; }

        [Column(Order = 11)]
        public bool IsStopped { get; set; }



        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}