using Invitee.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class ImageCostViewModel : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 10000)]
        [DisplayName("Image Count")]
        public long ImageCount { get; set; }
        [DataType(DataType.Currency)]
        [Required]
        [Range(1,int.MaxValue)]
        [DisplayName("Image Cost")]
        public double Cost { get; set; }
        [Range(0,100)]
        [DisplayName("Offer(Percentage)")]
        public int OfferInPercentage { get; set; }
    }
}