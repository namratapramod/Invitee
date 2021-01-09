using Invitee.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class OfferBannerViewModel : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MMM-dd}")]
        public DateTime OfferStartDate { get; set; } = DateTime.Now;
        public DateTime OfferEndDate { get; set; }

        [DisplayName("Offer Banner Image")]
        public HttpPostedFileBase File { get; set; }
        public string OfferText { get; set; }
        public bool IsImage { get; set; }
    }
}