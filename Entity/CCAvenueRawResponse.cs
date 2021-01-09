using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class CCAvenueRawResponse : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int order_id { get; set; }
        public long tracking_id { get; set; }
        public long bank_ref_no { get; set; }
        public string order_status { get; set; }
        public string failure_messages { get; set; }
        public string payment_mode { get; set; }
        public string card_name { get; set; }
        public string status_code { get; set; }
        public string status_message { get; set; }
        public string currency { get; set; }
        public double amount { get; set; }
        public string billing_name { get; set; }
        public string billing_address { get; set; }
        public string billing_city { get; set; }
        public string billing_state { get; set; }
        public string billing_zip { get; set; }
        public string billing_country { get; set; }
        public string billing_tel { get; set; }
        public string billing_email { get; set; }
        public string delivery_name { get; set; }
        public string delivery_address { get; set; }
        public string delivery_city { get; set; }
        public string delivery_state { get; set; }
        public string delivery_zip { get; set; }
        public string delivery_country { get; set; }
        public string delivery_tel { get; set; }
        public string merchant_param1 { get; set; }
        public string merchant_param2 { get; set; }
        public string merchant_param3 { get; set; }
        public string merchant_param4 { get; set; }
        public string merchant_param5 { get; set; }
        public string vault { get; set; }
        public string offer_type { get; set; }
        public string offer_code { get; set; }
        public double discount_value { get; set; }
        public double mer_amount { get; set; }
        public string eci_value { get; set; }
        public string retry { get; set; }
        public int response_code { get; set; }
        public string billing_notes { get; set; }
        public string trans_date { get; set; }
        public string bin_country { get; set; }
    }
}