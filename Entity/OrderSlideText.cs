using System.ComponentModel.DataAnnotations;

namespace Invitee.Entity
{
    public class OrderSlideText
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int SlideTextId { get; set; }
        public virtual SlideText SlideText { get; set; }
        public string NewSlideText { get; set; }
    }
}