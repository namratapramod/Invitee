using AutoMapper.Configuration.Conventions;
using Invitee.Entity.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class Order : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; }
        [Required]
        public int MediaTemplateId { get; set; }
        public virtual MediaTemplate MediaTemplate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int? CostingId { get; set; }
        public virtual Costing Costing { get; set; }
        public virtual ICollection<OrderImage> OrderImages { get; set; }
        public virtual ICollection<OrderSlideText> OrderSlideTexts { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }
        public DateTime? ProgressStartedAt { get; set; }
        public string AudioFilePath { get; set; }
        public virtual ICollection<MediaFilter> MediaFilters { get; set; }
        public string Note { get; set; }
        [DefaultValue(false)]
        public bool IsFreeOrder { get; set; }
        public string OrderCostingJson { get; set; }
        public string Reason { get; set; }
    }
}