namespace Invitee.Models.Payment
{
    public class PaymentRequest
    {
        public string orderId { get; set; }
        public string orderAmount { get; set; }
        public string orderCurrency { get; set; }
    }
}