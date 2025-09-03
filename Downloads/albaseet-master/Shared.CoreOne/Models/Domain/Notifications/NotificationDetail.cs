using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Notifications
{
    public class NotificationDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NotificationDetailId { get; set; }

        [Column(Order = 2)]
        public int NotificationHeaderId { get; set; }

        [Required]
        [StringLength(50)]
        [Column(Order = 3)]
        public string? ToUserName { get; set; }

        [Column(Order = 4)]
        public bool IsRead { get; set; }

        [Column(Order = 5)]
        public DateTime? ReadTime { get; set; }



        [ForeignKey(nameof(NotificationHeaderId))]
        public NotificationHeader? NotificationHeader { get; set; }
    }
}
