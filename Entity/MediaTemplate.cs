using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class MediaTemplate : BaseEntity, ISoftDelete
    {
        public MediaTemplate()
        {
            this.Costings = new HashSet<Costing>();
        }
        [Key]
        public int Id { get; set; }
        public bool IsFree { get; set; }
        public double NormalCost { get; set; }
        public string VideoFilePath { get; set; }
        public string VideoThumbnail { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public bool IsAlbumTemplate { get; set; }
        public virtual ICollection<SlideText> SlideTexts {get; set;}
        public virtual ICollection<Costing> Costings { get; set; }
        public virtual ICollection<MediaTemplateLike> MediaTemplateLikes { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public int OfferPercentage { get; set; }
        public int ImageCount { get; set; }
    }
}