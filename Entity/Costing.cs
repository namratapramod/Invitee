using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class Costing : BaseEntity
    {
        public Costing()
        {
            this.MediaTemplates = new HashSet<MediaTemplate>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public double CostToAdd { get; set; }
        public int OfferPercentage { get; set; }
        public int DeliveryHours { get; set; }
        public virtual ICollection<MediaTemplate>  MediaTemplates { get; set; }
    }
}