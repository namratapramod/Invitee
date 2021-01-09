using System.ComponentModel.DataAnnotations;

namespace Invitee.Entity
{
    public class CashfreePayment : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public double OrderAmount { get; set; }
        public string Currency { get; set; }
        public bool Status { get; set; }
        public string CfToken { get; set; }
    }
}