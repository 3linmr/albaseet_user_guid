using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Notifications
{
    public class NotificationHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NotificationHeaderId { get; set; }

        [Column(Order = 2)]
        public int NotificationTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column(Order = 3)]
        public string? Subject { get; set; }

        [Required]
        [StringLength(500)]
        [Column(Order = 4)]
        public string? Body { get; set; }

        [StringLength(50)]
        [Column(Order = 5)]
        public string? FromUserName { get; set; }

        [Column(Order = 6)]
        public DateTime EntryDate { get; set; }

        [Column(Order = 7)]
        public bool IsHighPriority { get; set; }

        [Column(Order = 8)]
        public bool SendLater { get; set; }

        [Column(Order = 9)]
        public DateTime NotifyTime { get; set; }

        [Column(Order = 10)]
        public int? ReferenceId { get; set; }



        [ForeignKey(nameof(NotificationTypeId))]
        public NotificationType? NotificationType { get; set; }
    }
}
