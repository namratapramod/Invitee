using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class ImageCost : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1,10000)]
        public long ImageCount { get; set; }
        [DataType(DataType.Currency)]
        [Required]
        public double Cost { get; set; }
        public int OfferInPercentage { get; set; }
    }
}