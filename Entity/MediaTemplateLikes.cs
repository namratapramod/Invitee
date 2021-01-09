using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class MediaTemplateLike : LikeEntityBase
    {
        public int MediaTemplateId { get; set; }
        public virtual MediaTemplate MediaTemplate { get; set; }
    }
}