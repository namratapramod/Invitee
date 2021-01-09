using Invitee.Entity;
using Invitee.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public partial class OrderResponseModel
    {
        public int Id { get; set; }
        public string AudioFilePath { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public int TemplateId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public  IEnumerable<object> OrderImages { get; set; }
        public IEnumerable<object> OrderSlideTextInfo { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime? ProgressStartedAt { get; set; }
        public IEnumerable<object> DeliveryInformation { get; set; }
        public IEnumerable<object> Filters { get; set; }
        public string Note { get; set; }
        public CostingDetails CostingDetails { get; set; }
        public bool IsAlbumTemplate { get; internal set; }
        public bool IsFreeOrder { get; set; }
        public string OrderCostingJson { get; internal set; }
    }

    public partial class CostingDetails
    {
        public object Costing { get; set; }
        public object ImageCosting { get; set; }
        public double? GrandTotal { get; set; }
    }
}