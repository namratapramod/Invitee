using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class PaymentConfig
    {
        [Key]
        public int Id { get; set; }
        public string AccessCode { get; set; }
        public string WorkingKey { get; set; }
        public string MerchantId { get; set; }
        public string RSAKeyUrl { get; set; }
        public bool InUse { get; set; }
    }
}