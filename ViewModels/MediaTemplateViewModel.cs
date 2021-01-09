using Invitee.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class MediaTemplateViewModel : BaseEntity
    {
        public MediaTemplateViewModel()
        {
            this.CategoryList = new List<Category>();
            this.CostingList = new List<Costing>();
            this.SlideTexts = new HashSet<SlideText>();
        }
        [Key]
        public int Id { get; set; }
        public bool IsFree { get; set; }
        
        public string VideoFilePath { get; set; }
        
        public string VideoThumbnail { get; set; }
        [Required]
        [DisplayName("Template Name")]
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        [DisplayName("Template Cost")]
        public double NormalCost { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        public int CategoryId { get; set; }
        [AutoMapper.IgnoreMap]
        public virtual Category Category { get; set; }
        public bool IsAlbumTemplate { get; set; }
        public virtual ICollection<SlideText> SlideTexts { get; set; }
        [AutoMapper.IgnoreMap]
        public string SlideTextInput { get; set; }
        public IList<Category> CategoryList { get; set; }
        public HttpPostedFileBase VideoFile { get; set; }
        public HttpPostedFileBase VideoThumbnailFile { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Costing> Costings { get; set; }
        public IList<Costing> CostingList { get; set; }
        public int[] CostingIds { get; set; }
        [Range(0,100)]
        public int OfferPercentage { get; set; }
        public int ImageCount { get; set; }
    }
}