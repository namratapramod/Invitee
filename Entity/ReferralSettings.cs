using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class ReferralSettings : BaseEntity
    {
        public int Id { get; set; }
        [Range(2, 50)]
        public int ReferralCountForFreeVideo { get; set; }
    }
}