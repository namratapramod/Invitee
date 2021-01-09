using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class DeliveryLike : LikeEntityBase
    {
        public Guid DeliveryId { get; set; }
        public virtual Delivery Delivery { get; set; }
    }
}