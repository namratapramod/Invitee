using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class Delivery : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string DeliveryFile { get; set; }
        public string DeliveryThumbnailFile { get; set; }
        public string ComplementaryFile { get; set; }
        public string ComplementaryThumbnailFile { get; set; }
        public string DeliveryFileUrl { get; set; }
        public string ComplementaryFileUrl { get; set; }
        public string DownloadUrl { get; set; }
        public int DownloadedCount { get; set; }
        [DefaultValue(false)]
        public bool IsPublic { get; set; }
        public virtual ICollection<DeliveryLike> DeliveryLikes { get; set; }
        public string Comment { get; set; }
        [Range(1,5)]
        public int? Rating { get; set; }
    }
}