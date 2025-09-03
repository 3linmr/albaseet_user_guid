using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shared.CoreOne.Models.Domain.Approval;

namespace Shared.CoreOne.Models
{
    public class BaseObject
    {
        [Column(Order = 200)]
        public DateTime? CreatedAt { get; set; }

        [Column(Order = 201)]
        public DateTime? ModifiedAt { get; set; }

        [StringLength(50)]
        [Column(Order = 202)]
        public string? UserNameCreated { get; set; }

        [StringLength(50)]
        [Column(Order = 203)]
        public string? UserNameModified { get; set; }

        [StringLength(50)]
        [Column(Order = 204)]
        public string? IpAddressCreated { get; set; }

        [StringLength(50)]
        [Column(Order = 205)]
        public string? IpAddressModified { get; set; }

        [Column(Order = 206)]
        public bool? Hide { get; set; }
    }

}
