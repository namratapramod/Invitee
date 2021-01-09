using Invitee.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class CostingViewModel : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        
        [Required(ErrorMessage = "The Costing Name field is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0,10000)]
        [Required(ErrorMessage = "The Cost to be added field is required")]
        public double CostToAdd { get; set; }
        [Range(0, 100)]
        public int OfferPercentage { get; set; }
        [Range(12,720)]
        [Required(ErrorMessage = "The Delivery (Hours) field is required")]
        public int DeliveryHours { get; set; }
    }
}