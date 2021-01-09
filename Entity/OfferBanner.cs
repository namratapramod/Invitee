using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class OfferBanner : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public DateTime OfferStartDate { get; set; } = DateTime.Now;
        public DateTime? OfferEndDate { get; set; }
        public string OfferText { get; set; }
        public bool IsImage { get; set; }
    }
}