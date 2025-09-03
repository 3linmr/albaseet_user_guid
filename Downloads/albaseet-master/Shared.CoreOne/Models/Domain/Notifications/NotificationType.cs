using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Notifications
{
    public class NotificationType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int NotificationTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column(Order = 2)]
        public string? NotificationTypeNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? NotificationTypeNameEn { get; set; }

        [Column(Order = 4)]
        public short AutomatedNotifyBeforeDays { get; set; }

        [Column(Order = 5)]
        public short NotifyAfterDays { get; set; }

        [Column(Order = 6)]
        public bool IsHighPriority { get; set; }
    }
}
