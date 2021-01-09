using Invitee.Entity;
using Invitee.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class OrderViewModel : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int MediaTemplateId { get; set; }
        public virtual MediaTemplate MediaTemplate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int? CostingId { get; set; }
        public virtual Costing Costing { get; set; }
        public virtual ICollection<OrderImage> OrderImages { get; set; }
        public virtual ICollection<OrderSlideText> OrderSlideTexts { get; set; }
        public DateTime? ProgressStartedAt { get; set; }
        public string Note { get; set; }
        public List<int> MediaFilterIds { get; set; }
        public string AudioFilePath { get; set; }
        public virtual ICollection<MediaFilter> MediaFilters { get; set; }
        public bool IsFreeOrder { get; set; }
        public string Reason { get; set; }
    }
}