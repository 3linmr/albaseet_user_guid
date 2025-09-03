using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Archive
{
    public class ArchiveDetail : BaseObject 
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArchiveDetailId { get; set; }

        [Column(Order = 2)]
        public int ArchiveHeaderId { get; set; }

        [StringLength(100)]
        [Column(Order = 3)]
        public string? RemarksAr { get; set; }

        [StringLength(100)]
        [Column(Order = 4)]
        public string? RemarksEn { get; set; }

        [Required]
        [StringLength(100)]
        [Column(Order = 5)]
        public string? FileName { get; set; }

        [Required]
        [StringLength(10)]
        [Column(Order = 6)]
        public string? FileExtension { get; set; }

        [Required]
        [StringLength(20)]
        [Column(Order = 7)]
        public string? FileType { get; set; }

        [Required]
        [StringLength(300)]
        [Column(Order = 8)]
        public string? FileUrl { get; set; }


        [Column(Order = 9)]
        public int? ReferenceId { get; set; }




        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
