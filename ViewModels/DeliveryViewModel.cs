using Invitee.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class DeliveryViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string DeliveryFile { get; set; }
        public string DeliveryThumbnailFile { get; set; }
        public string DeliveryFileUrl { get; set; }
        public string DownloadUrl { get; set; }
        public int DownloadedCount { get; set; }
        public string ComplementaryFile { get; set; }
        public string ComplementaryThumbnailFile { get; set; }
        public string ComplementaryFileUrl { get; set; }
        public HttpPostedFileBase DFile { get; set; }
        public HttpPostedFileBase CFile { get; set; }
        public HttpPostedFileBase CThumbnailFile { get; set; }
        public HttpPostedFileBase DThumbnailFile { get; set; }
    }
}