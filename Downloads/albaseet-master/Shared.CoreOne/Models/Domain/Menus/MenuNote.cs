using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Menus
{
    public class MenuNote : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int MenuNoteId { get; set; }

        [Column(Order = 2)]
        public short MenuCode { get; set; }

        [Column(Order = 3)]
        public int MenuNoteIdentifierId { get; set; }

        [Column(Order = 4)]
        public int ReferenceId { get; set; }

        [Column(Order = 5)]
        [Required, StringLength(2000)]
        public string? NoteValue { get; set; }

        [Column(Order = 6)]
        [StringLength(2000)]
        public string? Note { get; set; }

        [Column(Order = 7)]
        public bool ShowInReports { get; set; }

        [Column(Order = 8)]
        public bool ShowOnPrint { get; set; }

        [Column(Order = 9)]
        public bool ShowOnSelection { get; set; }



        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }

        [ForeignKey(nameof(MenuNoteIdentifierId))]
        public MenuNoteIdentifier? MenuNoteIdentifier { get; set; }
    }
}
